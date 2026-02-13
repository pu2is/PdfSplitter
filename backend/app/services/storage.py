from pathlib import Path
from uuid import uuid4

from fastapi import UploadFile


class FileStorageService:
    def __init__(self, upload_root: Path) -> None:
        self._upload_root = upload_root
        self._upload_root.mkdir(parents=True, exist_ok=True)

    async def save_pdf(self, file: UploadFile) -> tuple[str, str]:
        if file.content_type not in {"application/pdf", "application/octet-stream"}:
            raise ValueError("Only PDF files are supported.")

        if not file.filename or not file.filename.lower().endswith(".pdf"):
            raise ValueError("File name must end with .pdf.")

        file_id = uuid4().hex
        target_path = self._upload_root / f"{file_id}.pdf"

        # Stream in chunks to avoid loading large PDFs fully into memory.
        try:
            with target_path.open("wb") as output:
                while True:
                    chunk = await file.read(1024 * 1024)
                    if not chunk:
                        break
                    output.write(chunk)
        finally:
            await file.close()

        return file_id, file.filename

    def get_pdf_path(self, file_id: str) -> Path:
        return self._upload_root / f"{file_id}.pdf"
