import hashlib
import sqlite3
from pathlib import Path
from typing import Any


class PdfIndexService:
    def __init__(self, db_path: Path) -> None:
        self._db_path = db_path
        self._db_path.parent.mkdir(parents=True, exist_ok=True)
        self._initialize_db()

    def index_local_pdf(self, file_path: str) -> dict[str, Any]:
        normalized_path = self._normalize_pdf_path(file_path)
        sha256 = self._calculate_sha256(normalized_path)
        file_name = normalized_path.name

        with sqlite3.connect(self._db_path) as connection:
            cursor = connection.cursor()
            cursor.execute(
                """
                INSERT INTO pdf_files (path, filename, sha256)
                VALUES (?, ?, ?)
                ON CONFLICT(path) DO UPDATE SET
                  filename = excluded.filename,
                  sha256 = excluded.sha256
                """,
                (str(normalized_path), file_name, sha256),
            )
            connection.commit()

            cursor.execute(
                """
                SELECT id, path, filename, sha256
                FROM pdf_files
                WHERE path = ?
                """,
                (str(normalized_path),),
            )
            row = cursor.fetchone()

        if row is None:
            raise RuntimeError("Failed to index PDF file.")

        return {
            "id": row[0],
            "path": row[1],
            "filename": row[2],
            "sha256": row[3],
        }

    def get_file_path_by_id(self, file_id: int) -> Path | None:
        with sqlite3.connect(self._db_path) as connection:
            cursor = connection.cursor()
            cursor.execute("SELECT path FROM pdf_files WHERE id = ?", (file_id,))
            row = cursor.fetchone()

        if row is None:
            return None

        return Path(row[0])

    def _initialize_db(self) -> None:
        with sqlite3.connect(self._db_path) as connection:
            cursor = connection.cursor()
            cursor.execute(
                """
                CREATE TABLE IF NOT EXISTS pdf_files (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    path TEXT NOT NULL UNIQUE,
                    filename TEXT NOT NULL,
                    sha256 TEXT NOT NULL,
                    created_at TEXT NOT NULL DEFAULT (datetime('now'))
                )
                """
            )
            cursor.execute(
                "CREATE INDEX IF NOT EXISTS idx_pdf_files_sha256 ON pdf_files(sha256)"
            )
            connection.commit()

    @staticmethod
    def _normalize_pdf_path(file_path: str) -> Path:
        if not file_path.strip():
            raise ValueError("File path is required.")

        resolved_path = Path(file_path).expanduser().resolve(strict=True)
        if resolved_path.suffix.lower() != ".pdf":
            raise ValueError("Only PDF files are supported.")

        if not resolved_path.is_file():
            raise ValueError("File path must point to a file.")

        return resolved_path

    @staticmethod
    def _calculate_sha256(file_path: Path) -> str:
        digest = hashlib.sha256()
        with file_path.open("rb") as file_stream:
            while True:
                chunk = file_stream.read(1024 * 1024)
                if not chunk:
                    break
                digest.update(chunk)
        return digest.hexdigest()
