using PdfSplitter.Application.Abstractions.Persistence;
using PdfSplitter.Application.Abstractions.Storage;
using PdfSplitter.Application.Contracts;
using PdfSplitter.Domain.Documents;

namespace PdfSplitter.Application.Features.UploadPdf;

public sealed class UploadPdfUseCase
{
    private readonly IPdfDocumentRepository _documentRepository;
    private readonly IPdfFileStorage _fileStorage;

    public UploadPdfUseCase(IPdfDocumentRepository documentRepository, IPdfFileStorage fileStorage)
    {
        _documentRepository = documentRepository;
        _fileStorage = fileStorage;
    }

    public async Task<PdfDocumentSummary> ExecuteAsync(
        UploadPdfCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.TotalPages < 1)
        {
            throw new InvalidOperationException("Total pages must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(command.FileName))
        {
            throw new InvalidOperationException("File name is required.");
        }

        var document = new PdfDocument(Guid.NewGuid(), command.FileName, command.TotalPages);

        await _documentRepository.AddAsync(document, cancellationToken);
        await _fileStorage.SaveOriginalAsync(document.Id, command.FileContent, cancellationToken);

        return new PdfDocumentSummary(document.Id, document.OriginalFileName, document.TotalPages);
    }
}
