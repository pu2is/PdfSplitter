namespace PdfSplitter.Application.Contracts;

public sealed record PdfDocumentSummary(Guid DocumentId, string FileName, int TotalPages);
