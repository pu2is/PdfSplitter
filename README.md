# PdfSplitter

Architecture skeleton for a C# web application with a maintainable workflow:

1. Upload PDF
2. Manually define split ranges
3. Export PDF

This repository intentionally includes only structure and placeholders. No real PDF parsing or splitting is implemented yet.

## Architecture Style

- `Clean Architecture` with a `modular monolith` setup
- Strong dependency direction:
  - `Domain` <- `Application` <- `Infrastructure` <- `Web`
- Feature-oriented folders inside `Application`

## Project Structure

```text
src/
  PdfSplitter.Domain/
  PdfSplitter.Application/
  PdfSplitter.Infrastructure/
  PdfSplitter.Web/
tests/
  PdfSplitter.Domain.Tests/
  PdfSplitter.Application.Tests/
docs/
  Architecture.md
PdfSplitter.slnx
```

## Run

```powershell
dotnet restore
dotnet build
dotnet run --project src/PdfSplitter.Web/PdfSplitter.Web.csproj
```

## Next Implementation Targets

- Replace in-memory storage with durable file/object storage
- Replace stub split engine with a real PDF library integration
- Add application-level validation and integration tests
