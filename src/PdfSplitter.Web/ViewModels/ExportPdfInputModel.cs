using System.ComponentModel.DataAnnotations;

namespace PdfSplitter.Web.ViewModels;

public sealed class ExportPdfInputModel
{
    [Required]
    public Guid DocumentId { get; init; }

    [Required]
    public string OutputFileName { get; init; } = string.Empty;
}
