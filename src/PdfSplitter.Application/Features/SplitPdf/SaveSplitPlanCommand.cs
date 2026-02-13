using PdfSplitter.Domain.Splits;

namespace PdfSplitter.Application.Features.SplitPdf;

public sealed record SaveSplitPlanCommand(Guid DocumentId, IReadOnlyList<PageRange> Ranges);
