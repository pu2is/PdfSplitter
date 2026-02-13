using System.ComponentModel.DataAnnotations;

namespace PdfSplitter.Web.ViewModels;

public sealed class SplitPlanInputModel
{
    [Required]
    public Guid DocumentId { get; init; }

    [Required]
    public string RangesText { get; init; } = string.Empty;
}
