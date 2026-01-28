using System.Drawing;
using ClosedXML.Excel;
using Rokys.Audit.Common.Constant;
using Rokys.Audit.Model.Tables;
using System.Text.Json;

namespace Rokys.Audit.Services.Services.ReportUtils
{
    public class AuditDetailedReportExcelGenerator
    {
        public static string GenerateExcelReport(List<PeriodAuditTableScaleTemplateResult> tables)
        {
            if (tables == null || tables.Count == 0)
            {
                throw new ArgumentException("No hay datos para generar el reporte.");
            }

            try
            {
                using var workbook = new XLWorkbook();

                // Agrupar las tablas por ScaleGroup Code (punto auditable)
                var tablesByScaleGroup = tables
                    .Where(t => t.PeriodAuditScaleResult?.ScaleGroup != null && t.IsActive)
                    .OrderBy(t => t.PeriodAuditScaleResult.PeriodAuditGroupResult?.SortOrder ?? int.MaxValue)
                    .ThenBy(t => t.PeriodAuditScaleResult?.SortOrder ?? int.MaxValue)
                    .GroupBy(t => t.PeriodAuditScaleResult!.ScaleGroup!.Code ?? "")
                    .ToList();

                if (tablesByScaleGroup.Count == 0)
                {
                    throw new InvalidOperationException("No se encontraron datos válidos para generar el reporte.");
                }

                // Crear una hoja por cada código de punto auditable (ScaleGroup Code)
                foreach (var scaleGroupTables in tablesByScaleGroup)
                {
                    var code = scaleGroupTables.Key;
                    var name = scaleGroupTables.First().PeriodAuditScaleResult?.ScaleGroup?.Name ?? "";
                    
                    // Crear nombre de hoja usando solo el código y nombre del grupo de escala
                    var sheetName = SanitizeSheetName(code + (name != ""? $" - {name}": ""));
                    var worksheet = workbook.Worksheets.Add(sheetName);

                    // Llenar datos del punto auditable
                    FillScaleGroupData(worksheet, scaleGroupTables.ToList());
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var result = stream.ToArray();
                return Convert.ToBase64String(result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generando el archivo Excel: {ex.Message}", ex);
            }
        }

        private static string SanitizeSheetName(string name)
        {
            var sanitized = name
                .Replace(":", "-")
                .Replace("\\", "-")
                .Replace("/", "-")
                .Replace("?", "")
                .Replace("*", "")
                .Replace("[", "(")
                .Replace("]", ")");

            if (sanitized.Length > 31)
            {
                sanitized = sanitized.Substring(0, 31);
            }

            return sanitized;
        }

        private static void FillScaleGroupData(IXLWorksheet worksheet, List<PeriodAuditTableScaleTemplateResult> tables)
        {
            int currentRow = 1;

            // Agrupar por PeriodAuditTableScaleTemplateResult
            var groupedTables = tables
                .GroupBy(t => t.Code)
                .OrderBy(g => g.Key)
                .ToList();

            foreach (var group in groupedTables)
            {
                var table = group.First();
                var tableFields = table.PeriodAuditFieldValues?
                    .Where(f => f.IsActive)
                    .OrderBy(f => f.SortOrder)
                    .ToList() ?? new List<PeriodAuditFieldValues>();

                // Encabezados específicos para cada tabla
                worksheet.Cell(currentRow, 1).Value = table.Name;
                worksheet.Range(currentRow, 1, currentRow, 4 + tableFields.Count).Merge();
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(220, 230, 241);
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = "Empresa";
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(220, 230, 241);
                worksheet.Cell(currentRow, 2).Value = "Tienda";
                worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(220, 230, 241);
                worksheet.Cell(currentRow, 3).Value = "Auditoría";
                worksheet.Cell(currentRow, 3).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 3).Style.Fill.BackgroundColor = XLColor.FromArgb(220, 230, 241);
                worksheet.Cell(currentRow, 4).Value = "Estado auditoría";
                worksheet.Cell(currentRow, 4).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(220, 230, 241);

                int col = 5;

                foreach (var field in tableFields)
                {
                    worksheet.Cell(currentRow, col).Value = field.FieldName;
                    worksheet.Cell(currentRow, col).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, col).Style.Fill.BackgroundColor = XLColor.FromArgb(220, 230, 241);
                    col++;
                }

                currentRow++;

                // if orientation is horizontal, we need to handle differently
                if (table.Orientation != null && table.Orientation.Equals(TableOrientationCode.Horizontal, StringComparison.OrdinalIgnoreCase))
                {
                    HandleHorizontalOrientation(group, worksheet, ref currentRow, ref col);
                }
                else
                {
                    HandleVerticalOrientation(group, worksheet, ref currentRow, ref col);
                }
                currentRow++; // Fila extra entre registros
            }
        }

        private static void HandleHorizontalOrientation(
            IGrouping<string?, PeriodAuditTableScaleTemplateResult> group,
            IXLWorksheet worksheet,
            ref int currentRow,
            ref int col)
        {
            foreach (var item in group)
            {
                // 1️⃣ Tomamos un field solo para saber cuántas filas existen
                var firstFieldJson = item.PeriodAuditFieldValues?
                    .FirstOrDefault()?
                    .TableDataHorizontal ?? "[]";

                var firstFieldData = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(firstFieldJson);

                if (firstFieldData == null || !firstFieldData.Any())
                    return;

                // 2️⃣ Recorremos filas
                foreach (var rowIndex in firstFieldData
                    .Select(h => ((JsonElement)h["row"]).GetInt32())
                    .Distinct()
                    .OrderBy(x => x))
                {
                    col = 1;
                    worksheet.Cell(currentRow, col).Value = item.PeriodAuditScaleResult?.PeriodAuditGroupResult?.PeriodAudit?.Store?.Enterprise?.Name ?? "";
                    col++;
                    worksheet.Cell(currentRow, col).Value = item.PeriodAuditScaleResult?.PeriodAuditGroupResult?.PeriodAudit?.Store?.Name ?? "";
                    col++;
                    worksheet.Cell(currentRow, col).Value = item.PeriodAuditScaleResult?.PeriodAuditGroupResult?.PeriodAudit?.CorrelativeNumber ?? "";
                    col++;
                    worksheet.Cell(currentRow, col).Value = item.PeriodAuditScaleResult?.PeriodAuditGroupResult?.PeriodAudit?.AuditStatus?.Name ?? "";
                    col++;

                    var tbFields = item.PeriodAuditFieldValues?
                        .OrderBy(f => f.SortOrder)
                        .ToList() ?? new List<PeriodAuditFieldValues>();

                    // 3️⃣ Recorremos campos
                    foreach (var field in tbFields)
                    {
                        var fieldJson = field.TableDataHorizontal ?? "[]";

                        var fieldData = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(fieldJson);

                        var cellData = fieldData?
                            .FirstOrDefault(h => ((JsonElement)h["row"]).GetInt32() == rowIndex);

                        object? value = cellData?.GetValueOrDefault("value");

                        if (value is JsonElement je)
                        {
                            value = ConvertJsonElementToObject(je);
                        }

                        worksheet.Cell(currentRow, col).Value = ObjectToXLCellValue(value);
                        col++;
                    }

                    currentRow++;
                }
            }
        }


        private static void HandleVerticalOrientation(
            IGrouping<string?, PeriodAuditTableScaleTemplateResult> group,
            IXLWorksheet worksheet,
            ref int currentRow,
            ref int col)
        {
            foreach (var item in group)
            {
                col = 1;
                worksheet.Cell(currentRow, col).Value = item.PeriodAuditScaleResult?.PeriodAuditGroupResult?.PeriodAudit?.Store?.Enterprise?.Name ?? "";
                col++;
                worksheet.Cell(currentRow, col).Value = item.PeriodAuditScaleResult?.PeriodAuditGroupResult?.PeriodAudit?.Store?.Name ?? "";
                col++;
                worksheet.Cell(currentRow, col).Value = item.PeriodAuditScaleResult?.PeriodAuditGroupResult?.PeriodAudit?.CorrelativeNumber ?? "";
                col++;
                worksheet.Cell(currentRow, col).Value = item.PeriodAuditScaleResult?.PeriodAuditGroupResult?.PeriodAudit?.AuditStatus?.Name ?? "";  
                col++;

                var tbFields = item.PeriodAuditFieldValues?
                    .OrderBy(f => f.SortOrder)
                    .ToList() ?? new List<PeriodAuditFieldValues>();

                foreach (var field in tbFields)
                {
                    worksheet.Cell(currentRow, col).Value =  GetFieldValue(field);
                    col++;
                }

                currentRow++;
            }
        }


        private static XLCellValue GetFieldValue(PeriodAuditFieldValues field)
        {
            // Valores por defecto
            object? value = field.FieldType switch
            {
                FieldTypeCode.Text => field.TextValue,
                FieldTypeCode.Number => field.NumericValue,
                FieldTypeCode.Date => field.DateValue,
                FieldTypeCode.List => field.FieldOptionsValue,
                _ => string.Empty,
            };

            return ObjectToXLCellValue(value);
        }

        private static XLCellValue ObjectToXLCellValue(object? value)
        {
            return value == null ? string.Empty : value switch
            {
                string s     => s,
                int i        => i,
                long l       => l,
                decimal d   => d,
                double db   => db,
                DateTime dt => dt,
                bool b      => b,
                _ => value.ToString()
            };
        }

        private static object? ConvertJsonElementToObject(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.TryGetInt32(out var intValue) ? intValue : element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => element.ToString()
            };
        }
    }
}
