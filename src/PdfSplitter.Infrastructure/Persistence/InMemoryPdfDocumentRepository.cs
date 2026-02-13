using System.Collections.Concurrent;
using PdfSplitter.Application.Abstractions.Persistence;
using PdfSplitter.Domain.Documents;
using PdfSplitter.Domain.Splits;

namespace PdfSplitter.Infrastructure.Persistence;

public sealed class InMemoryPdfDocumentRepository : IPdfDocumentRepository
{
    private readonly ConcurrentDictionary<Guid, PdfDocument> _documents = new();
    private readonly ConcurrentDictionary<Guid, SplitPlan> _splitPlans = new();

    public Task AddAsync(PdfDocument document, CancellationToken cancellationToken = default)
    {
        _documents[document.Id] = document;
        return Task.CompletedTask;
    }

    public Task<PdfDocument?> GetByIdAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        _documents.TryGetValue(documentId, out var document);
        return Task.FromResult(document);
    }

    public Task SaveSplitPlanAsync(SplitPlan splitPlan, CancellationToken cancellationToken = default)
    {
        _splitPlans[splitPlan.DocumentId] = splitPlan;
        return Task.CompletedTask;
    }

    public Task<SplitPlan?> GetSplitPlanAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        _splitPlans.TryGetValue(documentId, out var splitPlan);
        return Task.FromResult(splitPlan);
    }
}
