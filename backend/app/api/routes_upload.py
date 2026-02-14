from pathlib import Path
from uuid import uuid4

from fastapi import APIRouter, File, HTTPException, UploadFile
from fastapi.responses import FileResponse

from app.api.schemas import (
    LocalPdfIndexRequest,
    LocalPdfIndexResponse,
)
from app.core.config import settings
from app.services.pdf_index import PdfIndexService

router = APIRouter(prefix="/api", tags=["pdf"])
pdf_index = PdfIndexService(settings.sqlite_db_path)


@router.get("/health")
def health() -> dict[str, str]:
    return {"status": "ok"}


@router.post("/files/index-local", response_model=LocalPdfIndexResponse)
def index_local_pdf(payload: LocalPdfIndexRequest) -> LocalPdfIndexResponse:
    print("[routes_upload] /files/index-local", f"file_path={payload.file_path}")
    try:
        indexed = pdf_index.index_local_pdf(payload.file_path)
    except ValueError as exc:
        raise HTTPException(status_code=400, detail=str(exc)) from exc

    return LocalPdfIndexResponse(
        file_id=indexed["id"],
        path=indexed["path"],
        filename=indexed["filename"],
        sha256=indexed["sha256"],
        file_url=f"/api/files/local/{indexed['id']}",
    )


@router.post("/files/index-upload", response_model=LocalPdfIndexResponse)
def index_uploaded_pdf(file: UploadFile = File(...)) -> LocalPdfIndexResponse:
    original_name = (file.filename or "").strip()
    if not original_name:
        raise HTTPException(status_code=400, detail="File name is required.")

    if Path(original_name).suffix.lower() != ".pdf":
        raise HTTPException(status_code=400, detail="Only PDF files are supported.")

    safe_file_name = f"{uuid4().hex}_{Path(original_name).name}"
    saved_path = settings.upload_root / safe_file_name

    try:
        with saved_path.open("wb") as output_file:
            while True:
                chunk = file.file.read(1024 * 1024)
                if not chunk:
                    break
                output_file.write(chunk)
    except OSError as exc:
        raise HTTPException(status_code=500, detail="Failed to store uploaded file.") from exc
    finally:
        file.file.close()

    print("[routes_upload] /files/index-upload", f"path={saved_path}")
    try:
        indexed = pdf_index.index_local_pdf(str(saved_path))
    except ValueError as exc:
        raise HTTPException(status_code=400, detail=str(exc)) from exc

    return LocalPdfIndexResponse(
        file_id=indexed["id"],
        path=indexed["path"],
        filename=indexed["filename"],
        sha256=indexed["sha256"],
        file_url=f"/api/files/local/{indexed['id']}",
    )


@router.get("/files/local/{file_id}")
def get_local_indexed_pdf(file_id: int) -> FileResponse:
    file_path = pdf_index.get_file_path_by_id(file_id)
    if file_path is None:
        raise HTTPException(status_code=404, detail="Indexed file not found.")

    if not file_path.exists():
        raise HTTPException(status_code=404, detail="Local file path no longer exists.")

    return FileResponse(
        path=file_path,
        media_type="application/pdf",
        filename=file_path.name,
    )
