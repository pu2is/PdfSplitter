import { defineStore } from "pinia";
import type { ApiErrorResponse, LocalPdfIndexResponse } from "../types/pdf";

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:8081";

interface PdfState {
  indexing: boolean;
  errorMessage: string;
  selectedPath: string;
  selectedSha256: string;
}

export const usePdfStore = defineStore("pdf", {
  state: (): PdfState => ({
    indexing: false,
    errorMessage: "",
    selectedPath: "",
    selectedSha256: ""
  }),
  actions: {
    setErrorMessage(message: string): void {
      this.errorMessage = message;
    },
    async indexLocalPdf(filePath: string): Promise<LocalPdfIndexResponse> {
      this.errorMessage = "";
      this.indexing = true;

      try {
        const response = await fetch(`${apiBaseUrl}/api/files/index-local`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json"
          },
          body: JSON.stringify({ file_path: filePath })
        });

        if (!response.ok) {
          const payload = (await response.json().catch(() => null)) as ApiErrorResponse | null;
          throw new Error(payload?.detail ?? "Failed to index local PDF.");
        }

        const payload = (await response.json()) as LocalPdfIndexResponse;
        this.selectedPath = payload.path;
        this.selectedSha256 = payload.sha256;
        return payload;
      } catch (error: unknown) {
        if (error instanceof TypeError) {
          this.errorMessage = `Cannot reach backend at ${apiBaseUrl}. Start the API server or update VITE_API_BASE_URL.`;
        } else {
          this.errorMessage =
            error instanceof Error ? error.message : "Failed to index local PDF.";
        }

        throw error;
      } finally {
        this.indexing = false;
      }
    }
  }
});
