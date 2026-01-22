using Rokys.Audit.DTOs.Responses.Reports;
using System.Drawing;
using ClosedXML.Excel;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Services.Services.ReportUtils
{
    public class AuditDetailedReportExcelGenerator
    {
        public static string GenerateExcelReport(List<PeriodAudit> items)
        {
            using var workbook = new XLWorkbook();

            // Obtener todos los puntos auditables únicos (ScaleGroups) de las auditorías
            var allScaleGroups = items
                .SelectMany(a => a.PeriodAuditGroupResults ?? new List<PeriodAuditGroupResult>())
                .SelectMany(gr => gr.PeriodAuditScaleResults ?? new List<PeriodAuditScaleResult>())
                .Where(sr => sr.ScaleGroup != null && sr.ScaleGroup.Group != null)
                .GroupBy(sr => sr.ScaleGroup!.Code)
                .Select(g => g.First())
                .Select(sr => new { 
                    sr.ScaleGroup!.ScaleGroupId, 
                    sr.ScaleGroup.Code, 
                    sr.ScaleGroup.Name, 
                    sr.ScaleGroup.SortOrder,
                    GroupSortOrder = sr.ScaleGroup.Group!.SortOrder,
                    GroupName = sr.ScaleGroup.Group.Name
                })
                .OrderBy(sg => sg.GroupSortOrder)
                .ThenBy(sg => sg.GroupName)
                .ThenBy(sg => sg.SortOrder)
                .ThenBy(sg => sg.Name)
                .ToList();

            // Crear una hoja por cada punto auditable (ScaleGroup)
            foreach (var scaleGroup in allScaleGroups)
            {
                // Filtrar auditorías que tienen este punto auditable
                var auditsForScaleGroup = items
                    .Where(a => a.PeriodAuditGroupResults != null && 
                                a.PeriodAuditGroupResults.Any(gr => 
                                    gr.PeriodAuditScaleResults != null &&
                                    gr.PeriodAuditScaleResults.Any(sr => sr.ScaleGroup != null && sr.ScaleGroup.Code == scaleGroup.Code)))
                    .ToList();

                if (auditsForScaleGroup.Any())
                {
                    // Crear nombre de hoja: "Código - Nombre"
                    var sheetName = SanitizeSheetName($"{scaleGroup.Code} - {scaleGroup.Name}");
                    var worksheet = workbook.Worksheets.Add(sheetName);

                    // Configurar encabezados
                    SetupHeaders(worksheet);

                    // TODO: Llenar datos del punto auditable (temporalmente deshabilitado para pruebas)
                    // FillScaleGroupData(worksheet, auditsForScaleGroup, scaleGroup.Code);

                    // Aplicar formato solo a encabezados
                    ApplyFormatting(worksheet, 0);

                    // Configurar columnas
                    ConfigureColumns(worksheet);
                }
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var result = stream.ToArray();
            return Convert.ToBase64String(result);
        }

        private static string SanitizeSheetName(string name)
        {
            // Excel no permite ciertos caracteres en nombres de hojas: : \ / ? * [ ]
            var sanitized = name
                .Replace(":", "-")
                .Replace("\\", "-")
                .Replace("/", "-")
                .Replace("?", "")
                .Replace("*", "")
                .Replace("[", "(")
                .Replace("]", ")");

            // Excel limita los nombres de hojas a 31 caracteres
            if (sanitized.Length > 31)
            {
                sanitized = sanitized.Substring(0, 31);
            }

            return sanitized;
        }

        private static void SetupHeaders(IXLWorksheet worksheet)
        {
            var headers = new[]
            {
                "Código Auditoría",
                "Fecha",
                "Empresa",
                "Tienda",
                "Código Tienda",
                "Punto Auditable",
                "Estado",
                "Puntuación Punto Auditable",
                "Puntuación Máxima",
                "Porcentaje (%)",
                "Escala",
                "Auditores",
                "Supervisores",
                "Gerentes de Operaciones"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }
        }

        private static void FillScaleGroupData(IXLWorksheet worksheet, List<PeriodAudit> audits, string scaleGroupCode)
        {
            int row = 2;
            foreach (var audit in audits)
            {
                // Buscar el resultado del punto auditable específico
                var scaleResult = audit.PeriodAuditGroupResults?
                    .SelectMany(gr => gr.PeriodAuditScaleResults ?? new List<PeriodAuditScaleResult>())
                    .FirstOrDefault(sr => sr.ScaleGroup != null && sr.ScaleGroup.Code == scaleGroupCode);

                if (scaleResult == null) continue;

                worksheet.Cell(row, 1).Value = audit.CorrelativeNumber;
                worksheet.Cell(row, 2).Value = audit.CreationDate.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cell(row, 3).Value = audit.Store?.Enterprise?.Name ?? string.Empty;
                worksheet.Cell(row, 4).Value = audit.Store?.Name ?? string.Empty;
                worksheet.Cell(row, 5).Value = audit.Store?.Code ?? string.Empty;
                
                // Nombre del punto auditable actual
                worksheet.Cell(row, 6).Value = scaleResult.ScaleGroup?.Name ?? string.Empty;

                worksheet.Cell(row, 7).Value = audit.AuditStatus?.Name ?? string.Empty;
                
                // Puntuaciones del punto auditable específico
                worksheet.Cell(row, 8).Value = scaleResult.ScoreValue;
                worksheet.Cell(row, 9).Value = scaleResult.AppliedWeighting;
                
                // Calcular porcentaje del punto auditable
                decimal scalePercentage = scaleResult.AppliedWeighting > 0 
                    ? (scaleResult.ScoreValue / scaleResult.AppliedWeighting) * 100 
                    : 0;
                worksheet.Cell(row, 10).Value = scalePercentage;
                
                worksheet.Cell(row, 11).Value = scaleResult.ScaleDescription ?? string.Empty;

                // Aplicar color de escala del punto auditable
                if (!string.IsNullOrEmpty(scaleResult.ScaleColor))
                {
                    try
                    {
                        var scaleColor = ColorTranslator.FromHtml(scaleResult.ScaleColor);
                        worksheet.Cell(row, 11).Style.Fill.BackgroundColor = XLColor.FromColor(scaleColor);
                        
                        if (IsColorDark(scaleColor))
                        {
                            worksheet.Cell(row, 11).Style.Font.FontColor = XLColor.White;
                        }
                    }
                    catch
                    {
                        // Ignorar si el color no es válido
                    }
                }

                // Auditores
                var auditors = audit.PeriodAuditParticipants?
                    .Where(p => p.RoleCodeSnapshot == "AUDITOR" && p.UserReference != null)
                    .Select(p => p.UserReference!.FullName)
                    .ToList() ?? new List<string>();
                worksheet.Cell(row, 12).Value = string.Join(", ", auditors);

                // Supervisores
                var supervisors = audit.PeriodAuditParticipants?
                    .Where(p => p.RoleCodeSnapshot == "SUPERVISOR" && p.UserReference != null)
                    .Select(p => p.UserReference!.FullName)
                    .ToList() ?? new List<string>();
                worksheet.Cell(row, 13).Value = string.Join(", ", supervisors);

                // Gerentes de Operaciones
                var managers = audit.PeriodAuditParticipants?
                    .Where(p => p.RoleCodeSnapshot == "OPERATION_MANAGER" && p.UserReference != null)
                    .Select(p => p.UserReference!.FullName)
                    .ToList() ?? new List<string>();
                worksheet.Cell(row, 14).Value = string.Join(", ", managers);

                row++;
            }
        }

        private static void ApplyFormatting(IXLWorksheet worksheet, int recordCount)
        {
            var dataRange = worksheet.Range(1, 1, recordCount + 1, 14);

            // Formato general
            dataRange.Style.Font.FontName = "Arial";
            dataRange.Style.Font.FontSize = 10;
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Formato de encabezados
            var headerRange = worksheet.Range(1, 1, 1, 14);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(84, 130, 53);
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // Formato de números
            if (recordCount > 0)
            {
                worksheet.Range(2, 8, recordCount + 1, 10).Style.NumberFormat.Format = "0.00";
            }
        }

        private static void ConfigureColumns(IXLWorksheet worksheet)
        {
            worksheet.Column(1).Width = 18;  // Código Auditoría
            worksheet.Column(2).Width = 16;  // Fecha
            worksheet.Column(3).Width = 20;  // Empresa
            worksheet.Column(4).Width = 30;  // Tienda
            worksheet.Column(5).Width = 15;  // Código Tienda
            worksheet.Column(6).Width = 30;  // Punto Auditable
            worksheet.Column(7).Width = 15;  // Estado
            worksheet.Column(8).Width = 18;  // Puntuación Punto Auditable
            worksheet.Column(9).Width = 20;  // Puntuación Máxima
            worksheet.Column(10).Width = 12; // Porcentaje
            worksheet.Column(11).Width = 20; // Escala
            worksheet.Column(12).Width = 40; // Auditores
            worksheet.Column(13).Width = 40; // Supervisores
            worksheet.Column(14).Width = 40; // Gerentes de Operaciones

            // Congelar fila de encabezados
            worksheet.SheetView.FreezeRows(1);

            // Filtros automáticos
            if (worksheet.LastRowUsed() != null)
            {
                var tableRange = worksheet.Range(1, 1, worksheet.LastRowUsed().RowNumber(), 14);
                tableRange.SetAutoFilter();
            }
        }

        private static bool IsColorDark(Color color)
        {
            var luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            return luminance < 0.5;
        }
    }
}
