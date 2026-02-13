using PdfSplitter.Application.Features.UploadPdf;

namespace PdfSplitter.Application.Tests;

public sealed class UploadPdfCommandTests
{
    [Fact]
    public void Constructor_AssignsValues()
    {
        using var stream = new MemoryStream();

        var command = new UploadPdfCommand("sample.pdf", stream, 12);

        Assert.Equal("sample.pdf", command.FileName);
        Assert.Equal(stream, command.FileContent);
        Assert.Equal(12, command.TotalPages);
    }
}
