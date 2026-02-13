using PdfSplitter.Application.Abstractions.Persistence;
using PdfSplitter.Domain.Splits;

namespace PdfSplitter.Application.Features.SplitPdf;

public sealed class SaveSplitPlanUseCase
{
    private readonly IPdfDocumentRepository _documentRepository;

    public SaveSplitPlanUseCase(IPdfDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task ExecuteAsync(
        SaveSplitPlanCommand command,
        CancellationToken cancellationToken = default)
    {
        var document = await _documentRepository.GetByIdAsync(command.DocumentId, cancellationToken);
        if (document is null)
        {
            throw new InvalidOperationException("PDF document was not found.");
        }

        if (command.Ranges.Count == 0)
        {
            throw new InvalidOperationException("At least one page range is required.");
        }

        if (command.Ranges.Any(range => !range.IsValidFor(document.TotalPages)))
        {
            throw new InvalidOperationException("One or more page ranges are invalid.");
        }

        var splitPlan = new SplitPlan(command.DocumentId, command.Ranges);
        await _documentRepository.SaveSplitPlanAsync(splitPlan, cancellationToken);
    }
}
