using Microsoft.AspNetCore.Mvc;
using PdfSplitter.Application.Features.ExportPdf;
using PdfSplitter.Application.Features.SplitPdf;
using PdfSplitter.Application.Features.UploadPdf;
using PdfSplitter.Domain.Splits;
using PdfSplitter.Web.ViewModels;

namespace PdfSplitter.Web.Controllers;

public sealed class PdfWorkflowController : Controller
{
    private readonly UploadPdfUseCase _uploadPdfUseCase;
    private readonly SaveSplitPlanUseCase _saveSplitPlanUseCase;
    private readonly ExportPdfUseCase _exportPdfUseCase;

    public PdfWorkflowController(
        UploadPdfUseCase uploadPdfUseCase,
        SaveSplitPlanUseCase saveSplitPlanUseCase,
        ExportPdfUseCase exportPdfUseCase)
    {
        _uploadPdfUseCase = uploadPdfUseCase;
        _saveSplitPlanUseCase = saveSplitPlanUseCase;
        _exportPdfUseCase = exportPdfUseCase;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new PdfUploadInputModel { TotalPages = 1 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(PdfUploadInputModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || model.File is null)
        {
            return View(nameof(Index), model);
        }

        await using var fileStream = model.File.OpenReadStream();
        var result = await _uploadPdfUseCase.ExecuteAsync(
            new UploadPdfCommand(model.File.FileName, fileStream, model.TotalPages),
            cancellationToken);

        return RedirectToAction(nameof(Split), new { documentId = result.DocumentId });
    }

    [HttpGet]
    public IActionResult Split(Guid documentId)
    {
        return View(new SplitPlanInputModel
        {
            DocumentId = documentId,
            RangesText = "1-1"
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Split(SplitPlanInputModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var ranges = ParseRanges(model.RangesText);
            await _saveSplitPlanUseCase.ExecuteAsync(
                new SaveSplitPlanCommand(model.DocumentId, ranges),
                cancellationToken);
        }
        catch (Exception ex) when (ex is FormatException or InvalidOperationException)
        {
            ModelState.AddModelError(nameof(model.RangesText), ex.Message);
            return View(model);
        }

        return RedirectToAction(nameof(Export), new { documentId = model.DocumentId });
    }

    [HttpGet]
    public IActionResult Export(Guid documentId)
    {
        return View(new ExportPdfInputModel
        {
            DocumentId = documentId,
            OutputFileName = $"split-{documentId:N}.pdf"
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Export(ExportPdfInputModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var outputFileName = await _exportPdfUseCase.ExecuteAsync(
                new ExportPdfCommand(model.DocumentId, model.OutputFileName),
                cancellationToken);

            TempData["StatusMessage"] = $"Export placeholder completed: {outputFileName}";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Error()
    {
        return View("~/Views/Shared/Error.cshtml");
    }

    private static IReadOnlyList<PageRange> ParseRanges(string rangesText)
    {
        var ranges = new List<PageRange>();
        var segments = rangesText.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var segment in segments)
        {
            var parts = segment.Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2 ||
                !int.TryParse(parts[0], out var startPage) ||
                !int.TryParse(parts[1], out var endPage))
            {
                throw new FormatException("Use range format like: 1-3, 4-6.");
            }

            ranges.Add(new PageRange(startPage, endPage));
        }

        if (ranges.Count == 0)
        {
            throw new FormatException("At least one page range is required.");
        }

        return ranges;
    }
}
