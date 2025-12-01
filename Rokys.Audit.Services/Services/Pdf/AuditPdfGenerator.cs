using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Common.Constant;

namespace Rokys.Audit.Services.Services.Pdf
{
    public class AuditPdfGenerator
    {
        public static async Task<byte[]> GenerateAuditReportPdf(
            PeriodAudit audit,
            List<(int nro, string proceso, string observations, string impact, string recommendation, string valorized)> auditData)
        {
            // Configurar licencia (Community License - gratuita para uso no comercial)
            QuestPDF.Settings.License = LicenseType.Community;

            var storeName = audit.Store?.Name ?? "Sin tienda";
            var enterpriseName = audit.Store?.Enterprise?.Name ?? "Sin empresa";
            var administrator = audit.PeriodAuditParticipants?.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.JefeDeArea.Code);
            var supervisor = audit.PeriodAuditParticipants?.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code);
            var auditor = audit.PeriodAuditParticipants?.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.Auditor.Code);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape()); // Horizontal para que quepa la tabla
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header()
                        .Column(column =>
                        {
                            column.Item().AlignCenter()
                                .Text($"INFORME DE AUDITORÍA DE LA TIENDA {storeName.ToUpper()}")
                                .SemiBold()
                                .FontSize(14)
                                .FontColor(Colors.Blue.Darken2);

                            column.Item().PaddingTop(5);
                        });

                    page.Content()
                        .PaddingVertical(0.5f, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(5);

                            // Información de cabecera en dos columnas
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(leftColumn =>
                                {
                                    leftColumn.Item().Text(text =>
                                    {
                                        text.Span("Administrador de tienda: ").SemiBold();
                                        text.Span(administrator?.UserReference?.FullName ?? "N/A");
                                    });

                                    leftColumn.Item().Text(text =>
                                    {
                                        text.Span("Empresa: ").SemiBold();
                                        text.Span(enterpriseName);
                                    });

                                    leftColumn.Item().Text(text =>
                                    {
                                        text.Span("Nombre de tienda: ").SemiBold();
                                        text.Span(storeName);
                                    });

                                    leftColumn.Item().Text(text =>
                                    {
                                        text.Span("Supervisor: ").SemiBold();
                                        text.Span(supervisor?.UserReference?.FullName ?? "N/A");
                                    });
                                });

                                row.RelativeItem().Column(rightColumn =>
                                {
                                    rightColumn.Item().Text(text =>
                                    {
                                        text.Span("Auditor: ").SemiBold();
                                        text.Span(auditor?.UserReference?.FullName ?? "N/A");
                                    });

                                    rightColumn.Item().Text(text =>
                                    {
                                        text.Span("Fecha de auditoría: ").SemiBold();
                                        text.Span(audit.EndDate.ToString("dd/MM/yyyy"));
                                    });
                                });
                            });

                            column.Item().PaddingTop(15);

                            // Tabla de datos
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(35);    // Nro
                                    columns.RelativeColumn(2);     // Proceso
                                    columns.RelativeColumn(3);     // Observaciones
                                    columns.RelativeColumn(2);     // Impacto
                                    columns.RelativeColumn(3);     // Recomendación
                                    columns.RelativeColumn(2);     // Valorizado
                                });

                                // Header
                                table.Header(header =>
                                {
                                    header.Cell().Element(HeaderCellStyle).Text("Nro");
                                    header.Cell().Element(HeaderCellStyle).Text("Proceso auditado");
                                    header.Cell().Element(HeaderCellStyle).Text("Observación");
                                    header.Cell().Element(HeaderCellStyle).Text("Impacto");
                                    header.Cell().Element(HeaderCellStyle).Text("Recomendación");
                                    header.Cell().Element(HeaderCellStyle).Text("Valorizado");

                                    static IContainer HeaderCellStyle(IContainer container)
                                    {
                                        return container
                                            .DefaultTextStyle(x => x.SemiBold().FontColor(Colors.White))
                                            .Background(Colors.Green.Darken2)
                                            .PaddingVertical(5)
                                            .PaddingHorizontal(3)
                                            .AlignCenter()
                                            .AlignMiddle();
                                    }
                                });

                                // Datos
                                foreach (var row in auditData)
                                {
                                    table.Cell().Element(CellStyle).AlignCenter().Text(row.nro.ToString());
                                    table.Cell().Element(CellStyle).Text(row.proceso);
                                    table.Cell().Element(CellStyle).Text(row.observations);
                                    table.Cell().Element(CellStyle).Text(row.impact);
                                    table.Cell().Element(CellStyle).Text(row.recommendation);
                                    table.Cell().Element(CellStyle).Text(row.valorized);

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container
                                            .Border(1)
                                            .BorderColor(Colors.Grey.Lighten2)
                                            .PaddingVertical(4)
                                            .PaddingHorizontal(3);
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
                            x.Span(" de ");
                            x.TotalPages();
                        });
                });
            });

            var bytes = document.GeneratePdf();
            return await Task.FromResult(bytes);
        }
    }
}
