using Rokys.Audit.DTOs.Responses.Reports;
using System.Drawing;
using ClosedXML.Excel;

namespace Rokys.Audit.Services.Services.ReportUtils
{
    public class ReportExcelGenerator
    {
        public static string GenerateExcelReport(List<PeriodAuditItemReportResponseDto> reports)
        {
            using var workbook = new XLWorkbook();

            // Crear hoja principal
            var worksheet = workbook.Worksheets.Add("Reporte de Auditorias");

            // Configurar encabezados
            SetupHeaders(worksheet);

            // Llenar datos
            FillData(worksheet, reports);

            // Aplicar formato
            ApplyFormatting(worksheet, reports.Count);

            // Configurar columnas
            ConfigureColumns(worksheet);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var result = stream.ToArray();
            return Convert.ToBase64String(result);
        }
        private static void SetupHeaders(IXLWorksheet worksheet)
        {
            var headers = new[]
            {
                "Empresa", "Tienda", "Auditor", "Supervisor", "Gerente de Operaciones",
                "Ranking", "Resultado del mes", "Nivel de Riesgo", "Veces Auditado", "Estado"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }
        }

        private static void FillData(IXLWorksheet worksheet, List<PeriodAuditItemReportResponseDto> reports)
        {
            int row = 2;
            foreach (var report in reports) 
            {
                    worksheet.Cell(row, 1).Value = report.EnterpriseName;
                    worksheet.Cell(row, 2).Value = report.StoreName;
                    worksheet.Cell(row, 3).Value = report.ResponsibleAuditorName;
                    worksheet.Cell(row, 4).Value = report.SupervisorName;
                    worksheet.Cell(row, 5).Value = report.OperationManagerName;
                    worksheet.Cell(row, 6).Value = report.Ranking;
                    worksheet.Cell(row, 7).Value = report.MothlyScore;
                    worksheet.Cell(row, 8).Value = report.LevelRisk;
                    worksheet.Cell(row, 9).Value = report.AuditedQuantityPerStore;
                    worksheet.Cell(row, 10).Value = report.AuditStatus.Name ?? "";
                    if (!string.IsNullOrEmpty(report.AuditStatus?.ColorCode))
                    {
                        try
                        {
                            var statusColor = ColorTranslator.FromHtml(report.AuditStatus.ColorCode);
                            worksheet.Cell(row, 10).Style.Fill.BackgroundColor = XLColor.FromColor(statusColor);

                            // Texto blanco si el color de fondo es oscuro
                            if (IsColorDark(statusColor))
                            {
                                worksheet.Cell(row, 10).Style.Font.FontColor = XLColor.White;
                            }
                        }
                        catch
                        {
                            // Ignorar si el color no es válido
                        }
                    }
                    row++;
                }
        }

        private static void ApplyFormatting(IXLWorksheet worksheet, int recordCount)
        {
            var dataRange = worksheet.Range(1, 1, recordCount + 1, 10);

            // Formato general
            dataRange.Style.Font.FontName = "Arial";
            dataRange.Style.Font.FontSize = 10;
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Formato de encabezados
            var headerRange = worksheet.Range(1, 1, 1, 10);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(84, 130, 53);
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        private static void ConfigureColumns(IXLWorksheet worksheet)
        {
            // Ajustar ancho de columnas
            worksheet.Column(1).Width = 15;  // Empresa
            worksheet.Column(2).Width = 30;  // Tienda
            worksheet.Column(3).Width = 25;  // Auditor
            worksheet.Column(4).Width = 18;  // Supervisor
            worksheet.Column(5).Width = 18;  // Jefe Regional
            worksheet.Column(6).Width = 18;  // Ranking
            worksheet.Column(7).Width = 18;  // Resultado del mes
            worksheet.Column(8).Width = 20;  // Nivel de Riesgo
            worksheet.Column(9).Width = 15;  // Nivel de Riesgo
            worksheet.Column(10).Width = 15;  // Estado

            // Congelar fila de encabezados
            worksheet.SheetView.FreezeRows(1);

            // Filtros automáticos
            var tableRange = worksheet.Range(1, 1, worksheet.LastRowUsed().RowNumber(), 10);
            tableRange.SetAutoFilter();
        }

        private static bool IsColorDark(Color color)
        {
            // Algoritmo para determinar si un color es oscuro
            var luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            return luminance < 0.5;
        }
    }
}
