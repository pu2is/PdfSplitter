using System.ComponentModel.DataAnnotations;

namespace PdfSplitter.Web.ViewModels;

public sealed class PdfUploadInputModel
{
    [Required]
    public IFormFile? File { get; init; }

    [Range(1, int.MaxValue)]
    public int TotalPages { get; init; }
}
