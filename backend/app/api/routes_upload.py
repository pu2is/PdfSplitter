from fastapi import APIRouter, File, HTTPException, UploadFile
from fastapi.responses import FileResponse

from app.api.schemas import (
    LocalPdfIndexRequest,
    LocalPdfIndexResponse,
    UploadPdfResponse,
)
from app.core.config import settings
from app.services.pdf_index import PdfIndexService
from app.services.storage import FileStorageService

router = APIRouter(prefix="/api", tags=["pdf"])
storage = FileStorageService(settings.upload_root)
pdf_index = PdfIndexService(settings.sqlite_db_path)


@router.get("/health")
def health() -> dict[str, str]:
    return {"status": "ok"}


@router.post("/files/index-local", response_model=LocalPdfIndexResponse)
def index_local_pdf(payload: LocalPdfIndexRequest) -> LocalPdfIndexResponse:
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


@router.post("/upload", response_model=UploadPdfResponse)
async def upload_pdf(file: UploadFile = File(...)) -> UploadPdfResponse:
    try:
        file_id, original_file_name = await storage.save_pdf(file)
    except ValueError as exc:
        raise HTTPException(status_code=400, detail=str(exc)) from exc

    file_url = f"/api/files/{file_id}"
    return UploadPdfResponse(
        file_id=file_id,
        original_file_name=original_file_name,
        file_url=file_url,
    )


@router.get("/files/{file_id}")
def get_pdf(file_id: str) -> FileResponse:
    file_path = storage.get_pdf_path(file_id)
    if not file_path.exists():
        raise HTTPException(status_code=404, detail="File not found.")

    return FileResponse(
        path=file_path,
        media_type="application/pdf",
        filename=file_path.name,
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
