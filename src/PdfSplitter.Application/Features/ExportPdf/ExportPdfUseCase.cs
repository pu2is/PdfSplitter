using PdfSplitter.Application.Abstractions.Persistence;
using PdfSplitter.Application.Abstractions.Processing;
using PdfSplitter.Application.Abstractions.Storage;

namespace PdfSplitter.Application.Features.ExportPdf;

public sealed class ExportPdfUseCase
{
    private readonly IPdfDocumentRepository _documentRepository;
    private readonly IPdfFileStorage _fileStorage;
    private readonly IPdfSplitEngine _pdfSplitEngine;

    public ExportPdfUseCase(
        IPdfDocumentRepository documentRepository,
        IPdfFileStorage fileStorage,
        IPdfSplitEngine pdfSplitEngine)
    {
        _documentRepository = documentRepository;
        _fileStorage = fileStorage;
        _pdfSplitEngine = pdfSplitEngine;
    }

    public async Task<string> ExecuteAsync(
        ExportPdfCommand command,
        CancellationToken cancellationToken = default)
    {
        var document = await _documentRepository.GetByIdAsync(command.DocumentId, cancellationToken);
        if (document is null)
        {
            throw new InvalidOperationException("PDF document was not found.");
        }

        var splitPlan = await _documentRepository.GetSplitPlanAsync(command.DocumentId, cancellationToken);
        if (splitPlan is null)
        {
            throw new InvalidOperationException("Split plan was not configured.");
        }

        await using var originalStream =
            await _fileStorage.OpenOriginalReadAsync(command.DocumentId, cancellationToken) ??
            throw new InvalidOperationException("Original PDF file was not found.");

        await using var exportedStream =
            await _pdfSplitEngine.SplitAsync(originalStream, splitPlan.Ranges, cancellationToken);

        await _fileStorage.SaveExportAsync(command.DocumentId, exportedStream, cancellationToken);

        return command.OutputFileName;
    }
}
