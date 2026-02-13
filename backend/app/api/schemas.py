from pydantic import BaseModel


class UploadPdfResponse(BaseModel):
    file_id: str
    original_file_name: str
    file_url: str
