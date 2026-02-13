using PdfSplitter.Application.Abstractions.Processing;
using PdfSplitter.Domain.Splits;

namespace PdfSplitter.Infrastructure.Processing;

public sealed class StubPdfSplitEngine : IPdfSplitEngine
{
    public async Task<Stream> SplitAsync(
        Stream originalPdf,
        IReadOnlyList<PageRange> ranges,
        CancellationToken cancellationToken = default)
    {
        if (ranges.Count == 0)
        {
            throw new InvalidOperationException("At least one page range is required.");
        }

        if (originalPdf.CanSeek)
        {
            originalPdf.Position = 0;
        }

        var output = new MemoryStream();
        await originalPdf.CopyToAsync(output, cancellationToken);
        output.Position = 0;

        return output;
    }
}
