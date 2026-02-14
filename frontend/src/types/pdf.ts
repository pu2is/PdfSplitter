export interface UploadPdfResponse {
  file_id: string;
  original_file_name: string;
  file_url: string;
}

export interface LocalPdfIndexResponse {
  file_id: number;
  path: string;
  filename: string;
  sha256: string;
  file_url: string;
}

export interface ApiErrorResponse {
  detail?: string;
}
