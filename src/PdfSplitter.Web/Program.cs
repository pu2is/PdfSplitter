using PdfSplitter.Application.Abstractions.Persistence;
using PdfSplitter.Application.Abstractions.Processing;
using PdfSplitter.Application.Abstractions.Storage;
using PdfSplitter.Application.Features.ExportPdf;
using PdfSplitter.Application.Features.SplitPdf;
using PdfSplitter.Application.Features.UploadPdf;
using PdfSplitter.Infrastructure.Persistence;
using PdfSplitter.Infrastructure.Processing;
using PdfSplitter.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IPdfDocumentRepository, InMemoryPdfDocumentRepository>();
builder.Services.AddSingleton<IPdfFileStorage, InMemoryPdfFileStorage>();
builder.Services.AddSingleton<IPdfSplitEngine, StubPdfSplitEngine>();

builder.Services.AddScoped<UploadPdfUseCase>();
builder.Services.AddScoped<SaveSplitPlanUseCase>();
builder.Services.AddScoped<ExportPdfUseCase>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/PdfWorkflow/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=PdfWorkflow}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
