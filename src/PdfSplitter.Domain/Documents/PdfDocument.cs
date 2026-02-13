namespace PdfSplitter.Domain.Documents;

public sealed class PdfDocument
{
    public PdfDocument(Guid id, string originalFileName, int totalPages)
    {
        if (string.IsNullOrWhiteSpace(originalFileName))
        {
            throw new ArgumentException("File name is required.", nameof(originalFileName));
        }

        if (totalPages < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(totalPages), "Total pages must be greater than zero.");
        }

        Id = id;
        OriginalFileName = originalFileName;
        TotalPages = totalPages;
        CreatedUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; }

    public string OriginalFileName { get; }

    public int TotalPages { get; }

    public DateTimeOffset CreatedUtc { get; }
}
