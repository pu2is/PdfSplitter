import hashlib
import sqlite3
from pathlib import Path
from typing import Any

from app.utils import connect_sqlite


class PdfIndexService:
    def __init__(self, db_path: Path) -> None:
        self._db_path = db_path
        self._db_path.parent.mkdir(parents=True, exist_ok=True)
        self._initialize_db()

    def index_local_pdf(self, file_path: str) -> dict[str, Any]:
        normalized_path = self._normalize_pdf_path(file_path)
        normalized_path_str = str(normalized_path)
        file_name = normalized_path.name
        print(
            "[PdfIndexService] index request",
            f"path={normalized_path}",
            f"filename={file_name}",
        )
        with connect_sqlite(self._db_path) as connection:
            cursor = connection.cursor()
            row = self._find_by_path(cursor, normalized_path)
            if row is not None:
                print(
                    "[PdfIndexService] file found by path",
                    f"id={row[0]}",
                    f"path={row[1]}",
                    f"filename={row[2]}",
                    f"sha256={row[3]}",
                )
            else:
                sha256 = self._calculate_sha256(normalized_path)
                print("[PdfIndexService] calculated sha256", f"sha256={sha256}")

                row = self._check_sha256(cursor, sha256)
                if row is not None:
                    existing_id, existing_path, existing_filename, existing_sha = row
                    if (existing_path != normalized_path_str or existing_filename != file_name):
                        cursor.execute(
                            """
                            UPDATE pdf_files
                            SET path = ?, filename = ?
                            WHERE id = ?
                            """,
                            (normalized_path_str, file_name, existing_id),
                        )
                        connection.commit()
                        row = (existing_id, normalized_path_str, file_name, existing_sha)
                    print(
                        "[PdfIndexService] file found by sha256",
                        f"id={row[0]}",
                        f"path={row[1]}",
                        f"filename={row[2]}",
                        f"sha256={row[3]}",
                    )
                else:
                    cursor.execute(
                        """
                        INSERT INTO pdf_files (path, filename, sha256)
                        VALUES (?, ?, ?)
                        """,
                        (normalized_path_str, file_name, sha256),
                    )
                    connection.commit()
                    inserted_id = cursor.lastrowid
                    if inserted_id is None:
                        raise RuntimeError("Failed to index PDF file.")
                    row = (inserted_id, normalized_path_str, file_name, sha256)

        if row is None:
            raise RuntimeError("Failed to index PDF file.")
        print(
            "[PdfIndexService] sqlite upsert complete",
            f"id={row[0]}",
            f"path={row[1]}",
            f"filename={row[2]}",
            f"sha256={row[3]}",
            f"db={self._db_path}",
        )

        return {
            "id": row[0],
            "path": row[1],
            "filename": row[2],
            "sha256": row[3],
        }

    def get_file_path_by_id(self, file_id: int) -> Path | None:
        with connect_sqlite(self._db_path) as connection:
            cursor = connection.cursor()
            cursor.execute("SELECT path FROM pdf_files WHERE id = ?", (file_id,))
            row = cursor.fetchone()

        if row is None:
            return None

        return Path(row[0])

    def _initialize_db(self) -> None:
        with connect_sqlite(self._db_path) as connection:
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
        normalized_file_path = file_path.strip()
        if not normalized_file_path:
            raise ValueError("File path is required.")

        if "fakepath" in normalized_file_path.lower():
            raise ValueError("Invalid file path from browser input. Upload the PDF file instead.")

        try:
            resolved_path = Path(normalized_file_path).expanduser().resolve(strict=True)
        except FileNotFoundError as exc:
            raise ValueError("File path does not exist.") from exc

        if resolved_path.suffix.lower() != ".pdf":
            raise ValueError("Only PDF files are supported.")

        if not resolved_path.is_file():
            raise ValueError("File path must point to a file.")

        return resolved_path

    @staticmethod
    def _find_by_path(
        cursor: sqlite3.Cursor, normalized_path: Path
    ) -> tuple[int, str, str, str] | None:
        cursor.execute(
            """
            SELECT id, path, filename, sha256
            FROM pdf_files
            WHERE path = ?
            LIMIT 1
            """,
            (str(normalized_path),),
        )
        return cursor.fetchone()

    @staticmethod
    def _check_sha256(
        cursor: sqlite3.Cursor, sha256: str
    ) -> tuple[int, str, str, str] | None:
        cursor.execute(
            """
            SELECT id, path, filename, sha256
            FROM pdf_files
            WHERE sha256 = ?
            LIMIT 1
            """,
            (sha256,),
        )
        return cursor.fetchone()

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
