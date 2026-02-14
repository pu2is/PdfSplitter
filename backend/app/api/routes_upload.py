from fastapi import APIRouter, File, HTTPException, UploadFile
from fastapi.responses import FileResponse

from app.api.schemas import (
    LocalPdfIndexRequest,
    LocalPdfIndexResponse,
    UploadPdfResponse,
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
