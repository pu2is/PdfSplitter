using PdfSplitter.Domain.Documents;
using PdfSplitter.Domain.Splits;

namespace PdfSplitter.Application.Abstractions.Persistence;

public interface IPdfDocumentRepository
{
    Task AddAsync(PdfDocument document, CancellationToken cancellationToken = default);

    Task<PdfDocument?> GetByIdAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task SaveSplitPlanAsync(SplitPlan splitPlan, CancellationToken cancellationToken = default);

    Task<SplitPlan?> GetSplitPlanAsync(Guid documentId, CancellationToken cancellationToken = default);
}
