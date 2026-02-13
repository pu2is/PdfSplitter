# Architecture Overview

## Goals

- High maintainability
- Easy onboarding and straightforward naming
- Clear boundaries for future feature delivery

## Layer Responsibilities

### `PdfSplitter.Domain`

- Core business models
- No dependency on external frameworks
- Current placeholders:
  - `PdfDocument`
  - `PageRange`
  - `SplitPlan`

### `PdfSplitter.Application`

- Use-case orchestration
- Application interfaces for persistence/storage/processing
- Feature folders:
  - `UploadPdf`
  - `SplitPdf`
  - `ExportPdf`

### `PdfSplitter.Infrastructure`

- Placeholder implementations for application interfaces
- Current implementations are in-memory and stubbed
- Safe to replace with real adapters later without touching use cases

### `PdfSplitter.Web`

- ASP.NET Core MVC entry point
- Controllers and view models for user workflow
- Placeholder pages for upload/split/export

## Dependency Rules

- `Domain` references nothing
- `Application` references only `Domain`
- `Infrastructure` references `Application` and `Domain`
- `Web` references `Application` and `Infrastructure`

This keeps core business decisions independent from framework and IO details.

## Workflow Placeholder

1. User uploads a PDF file (manual page count input for now)
2. User enters split ranges (e.g., `1-3, 4-6`)
3. User triggers export

Current export is a stub and simply keeps structure ready for real PDF integration.
