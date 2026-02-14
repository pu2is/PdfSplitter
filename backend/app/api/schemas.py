from pydantic import BaseModel


class UploadPdfResponse(BaseModel):
    file_id: str
    original_file_name: str
    file_url: str


class LocalPdfIndexRequest(BaseModel):
    file_path: str


class LocalPdfIndexResponse(BaseModel):
    file_id: int
    path: str
    filename: str
    sha256: str
    file_url: str
