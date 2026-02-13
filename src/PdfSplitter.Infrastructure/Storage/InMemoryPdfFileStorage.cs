using System.Collections.Concurrent;
using PdfSplitter.Application.Abstractions.Storage;

namespace PdfSplitter.Infrastructure.Storage;

public sealed class InMemoryPdfFileStorage : IPdfFileStorage
{
    private readonly ConcurrentDictionary<Guid, byte[]> _originalFiles = new();
    private readonly ConcurrentDictionary<Guid, byte[]> _exportedFiles = new();

    public async Task SaveOriginalAsync(
        Guid documentId,
        Stream source,
        CancellationToken cancellationToken = default)
    {
        _originalFiles[documentId] = await ReadAllBytesAsync(source, cancellationToken);
    }

    public Task<Stream?> OpenOriginalReadAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        _originalFiles.TryGetValue(documentId, out var data);
        Stream? stream = data is null ? null : new MemoryStream(data, writable: false);
        return Task.FromResult(stream);
    }

    public async Task SaveExportAsync(
        Guid documentId,
        Stream source,
        CancellationToken cancellationToken = default)
    {
        _exportedFiles[documentId] = await ReadAllBytesAsync(source, cancellationToken);
    }

    public Task<Stream?> OpenExportReadAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        _exportedFiles.TryGetValue(documentId, out var data);
        Stream? stream = data is null ? null : new MemoryStream(data, writable: false);
        return Task.FromResult(stream);
    }

    private static async Task<byte[]> ReadAllBytesAsync(Stream source, CancellationToken cancellationToken)
    {
        if (source.CanSeek)
        {
            source.Position = 0;
        }

        await using var memoryStream = new MemoryStream();
        await source.CopyToAsync(memoryStream, cancellationToken);
        return memoryStream.ToArray();
    }
}
