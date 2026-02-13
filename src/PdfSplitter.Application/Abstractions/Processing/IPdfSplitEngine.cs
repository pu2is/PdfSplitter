using PdfSplitter.Domain.Splits;

namespace PdfSplitter.Application.Abstractions.Processing;

public interface IPdfSplitEngine
{
    Task<Stream> SplitAsync(
        Stream originalPdf,
        IReadOnlyList<PageRange> ranges,
        CancellationToken cancellationToken = default);
}
