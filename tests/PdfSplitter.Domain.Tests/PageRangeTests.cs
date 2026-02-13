using PdfSplitter.Domain.Splits;

namespace PdfSplitter.Domain.Tests;

public sealed class PageRangeTests
{
    [Fact]
    public void IsValidFor_ReturnsTrue_WhenRangeFitsDocument()
    {
        var range = new PageRange(2, 5);

        var isValid = range.IsValidFor(10);

        Assert.True(isValid);
    }

    [Fact]
    public void IsValidFor_ReturnsFalse_WhenRangeExceedsDocument()
    {
        var range = new PageRange(5, 20);

        var isValid = range.IsValidFor(10);

        Assert.False(isValid);
    }
}
