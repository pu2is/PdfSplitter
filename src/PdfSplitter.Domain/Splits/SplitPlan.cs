namespace PdfSplitter.Domain.Splits;

public sealed class SplitPlan
{
    public SplitPlan(Guid documentId, IReadOnlyList<PageRange> ranges)
    {
        if (ranges.Count == 0)
        {
            throw new ArgumentException("At least one page range is required.", nameof(ranges));
        }

        DocumentId = documentId;
        Ranges = ranges;
    }

    public Guid DocumentId { get; }

    public IReadOnlyList<PageRange> Ranges { get; }
}
