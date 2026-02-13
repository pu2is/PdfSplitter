namespace PdfSplitter.Application.Features.UploadPdf;

public sealed record UploadPdfCommand(string FileName, Stream FileContent, int TotalPages);
