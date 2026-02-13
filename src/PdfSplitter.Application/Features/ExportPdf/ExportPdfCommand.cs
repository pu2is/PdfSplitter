namespace PdfSplitter.Application.Features.ExportPdf;

public sealed record ExportPdfCommand(Guid DocumentId, string OutputFileName);
