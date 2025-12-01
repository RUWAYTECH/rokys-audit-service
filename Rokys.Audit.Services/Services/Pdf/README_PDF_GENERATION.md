# Generación de PDFs para Informes de Auditoría

## Estado Actual
Actualmente, el generador de PDFs (`AuditPdfGenerator.cs`) genera un archivo HTML simple que se envía como adjunto. Esto funciona para pruebas iniciales, pero para producción se recomienda usar una librería de PDF profesional.

## Recomendación: Instalar QuestPDF

QuestPDF es una librería moderna, gratuita y fácil de usar para generar PDFs en .NET.

### Instalación

```bash
cd Rokys.Audit.Services
dotnet add package QuestPDF
```

### Ejemplo de Implementación con QuestPDF

Reemplazar el contenido de `AuditPdfGenerator.cs` con:

```csharp
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Services.Services.Pdf
{
    public class AuditPdfGenerator
    {
        public static async Task<byte[]> GenerateAuditReportPdf(
            PeriodAudit audit,
            List<(int nro, string proceso, string observations, string impact, string recommendation, string valorized)> auditData)
        {
            // Configurar licencia (Community License - gratuita)
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header()
                        .Text($"INFORME DE AUDITORÍA DE LA TIENDA {audit.Store?.Name?.ToUpper() ?? "SIN TIENDA"}")
                        .SemiBold().FontSize(16).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(10);

                            // Información de cabecera
                            var administrator = audit.PeriodAuditParticipants?.FirstOrDefault(p => p.RoleCodeSnapshot == "ADMIN_TIENDA");
                            var supervisor = audit.PeriodAuditParticipants?.FirstOrDefault(p => p.RoleCodeSnapshot == "SUPERVISOR_TRABAJO");
                            var auditor = audit.PeriodAuditParticipants?.FirstOrDefault(p => p.RoleCodeSnapshot == "AUDITOR");

                            column.Item().Text($"Administrador de tienda: {administrator?.UserReference?.FullName ?? "N/A"}");
                            column.Item().Text($"Nombre de tienda: {audit.Store?.Name ?? "N/A"}");
                            column.Item().Text($"Supervisor: {supervisor?.UserReference?.FullName ?? "N/A"}");
                            column.Item().Text($"Auditor: {auditor?.UserReference?.FullName ?? "N/A"}");
                            column.Item().Text($"Fecha de auditoría: {audit.EndDate:dd/MM/yyyy}");

                            column.Item().PaddingTop(20);

                            // Tabla de datos
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(30);    // Nro
                                    columns.RelativeColumn(2);     // Proceso
                                    columns.RelativeColumn(2);     // Observación
                                    columns.RelativeColumn(2);     // Impacto
                                    columns.RelativeColumn(2);     // Recomendación
                                    columns.RelativeColumn(1);     // Valorizado
                                });

                                // Header
                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Nro");
                                    header.Cell().Element(CellStyle).Text("Proceso auditado");
                                    header.Cell().Element(CellStyle).Text("Observación");
                                    header.Cell().Element(CellStyle).Text("Impacto");
                                    header.Cell().Element(CellStyle).Text("Recomendación");
                                    header.Cell().Element(CellStyle).Text("Valorizado");

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                    }
                                });

                                // Datos
                                foreach (var row in auditData)
                                {
                                    table.Cell().Element(CellStyle).Text(row.nro.ToString());
                                    table.Cell().Element(CellStyle).Text(row.proceso);
                                    table.Cell().Element(CellStyle).Text(row.observations);
                                    table.Cell().Element(CellStyle).Text(row.impact);
                                    table.Cell().Element(CellStyle).Text(row.recommendation);
                                    table.Cell().Element(CellStyle).Text(row.valorized);

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                    }
                                }
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                        });
                });
            });

            var bytes = document.GeneratePdf();
            return await Task.FromResult(bytes);
        }
    }
}
```

## Alternativas

### iText7 (Comercial)
- Muy potente pero requiere licencia comercial para uso productivo
- `dotnet add package itext7`

### PdfSharpCore
- Gratuita y open source
- `dotnet add package PdfSharpCore`

### DinkToPdf
- Convierte HTML a PDF usando wkhtmltopdf
- `dotnet add package DinkToPdf`

## Ubicación de PDFs Generados

Los PDFs se generan en memoria y se adjuntan directamente al correo. No se almacenan en disco por defecto.

Si deseas almacenarlos, puedes guardarlos en:
- `wwwroot/reports/audits/` (para acceso web)
- Una carpeta del servidor como `C:/AuditReports/` o `/var/audit-reports/`
- Azure Blob Storage o AWS S3 (para cloud)

Para guardar en disco, agregar al método después de generar el PDF:

```csharp
var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Reports", "Audits");
Directory.CreateDirectory(folderPath);
var filePath = Path.Combine(folderPath, fileName);
await File.WriteAllBytesAsync(filePath, pdfBytes);
```
