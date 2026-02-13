namespace PdfSplitter.Application.Abstractions.Storage;

public interface IPdfFileStorage
{
    Task SaveOriginalAsync(Guid documentId, Stream source, CancellationToken cancellationToken = default);

    Task<Stream?> OpenOriginalReadAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task SaveExportAsync(Guid documentId, Stream source, CancellationToken cancellationToken = default);

    Task<Stream?> OpenExportReadAsync(Guid documentId, CancellationToken cancellationToken = default);
}
