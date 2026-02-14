from pathlib import Path


class Settings:
    def __init__(self) -> None:
        self.project_root = Path(__file__).resolve().parents[2]
        self.upload_root = self.project_root / "storage" / "uploads"
        self.upload_root.mkdir(parents=True, exist_ok=True)
        self.storage_root = self.project_root / "storage"
        self.storage_root.mkdir(parents=True, exist_ok=True)
        self.sqlite_db_path = self.storage_root / "pdf_index.db"
        self.frontend_origin = "http://localhost:5173"


settings = Settings()
