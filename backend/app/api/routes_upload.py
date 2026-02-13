from fastapi import APIRouter, File, HTTPException, UploadFile
from fastapi.responses import FileResponse

from app.api.schemas import UploadPdfResponse
from app.core.config import settings
from app.services.storage import FileStorageService

router = APIRouter(prefix="/api", tags=["pdf"])
storage = FileStorageService(settings.upload_root)


@router.get("/health")
def health() -> dict[str, str]:
    return {"status": "ok"}


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
