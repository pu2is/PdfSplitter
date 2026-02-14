import { defineStore } from "pinia";
import type { ApiErrorResponse, ChosenPdfForm, LocalPdfIndexResponse } from "../types/pdf";

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:8081";

function isLocalFilePath(filePath: string): boolean {
  const normalizedPath = filePath.trim();
  if (normalizedPath === "") {
    return false;
  }

  const loweredPath = normalizedPath.toLowerCase();
  if (loweredPath.includes("\\fakepath\\") || loweredPath.includes("/fakepath/")) {
    return false;
  }

  return normalizedPath.startsWith("/") || normalizedPath.startsWith("\\\\") || /^[a-zA-Z]:[\\/]/.test(normalizedPath);
}

interface PdfState {
  currentPdf: LocalPdfIndexResponse | null;
  loading: boolean;
  errorMessage: string;
}

export const usePdfStore = defineStore("pdf-store", {
  state: (): PdfState => ({
    currentPdf: null,
    loading: false,
    errorMessage: ""
  }),
  actions: {
    async selectPdf(form: ChosenPdfForm): Promise<LocalPdfIndexResponse | null> {
      const selectedFile = form.file;
      const hasLocalPath = isLocalFilePath(form.filePath);

      console.log("[pdfStore] selectPdf request started", hasLocalPath ? {
        mode: "index-local",
        filePath: form.filePath
      } : selectedFile instanceof File ? {
        mode: "upload",
        fileName: selectedFile.name
      } : {
        mode: "index-local",
        filePath: form.filePath
      });

      this.loading = true;
      this.errorMessage = "";

      try {
        let response: Response;

        if (hasLocalPath) {
          response = await fetch(`${apiBaseUrl}/api/files/index-local`, {
            method: "POST",
            headers: {
              "Content-Type": "application/json"
            },
            body: JSON.stringify({
              file_path: form.filePath
            })
          });
        } else if (selectedFile instanceof File) {
          const uploadFormData = new FormData();
          uploadFormData.append("file", selectedFile);

          response = await fetch(`${apiBaseUrl}/api/files/index-upload`, {
            method: "POST",
            body: uploadFormData
          });
        } else {
          this.errorMessage = "Unable to resolve an absolute file path. Please use the desktop file picker.";
          console.log("[pdfStore] selectPdf skipped", {
            reason: "missing-absolute-path-and-file",
            filePath: form.filePath
          });
          return null;
        }

        console.log("[pdfStore] selectPdf response received", {
          ok: response.ok,
          status: response.status,
          statusText: response.statusText
        });

        if (!response.ok) {
          let errorMessage = "Failed to choose PDF file.";

          try {
            const errorPayload = (await response.json()) as ApiErrorResponse;
            if (typeof errorPayload.detail === "string" && errorPayload.detail.trim() !== "") {
              errorMessage = errorPayload.detail;
            }
          } catch {
            // keep default error message
          }

          this.errorMessage = errorMessage;
          console.log("[pdfStore] selectPdf failed", {
            errorMessage
          });
          return null;
        }

        const indexedPdf = (await response.json()) as LocalPdfIndexResponse;
        this.currentPdf = indexedPdf;
        console.log("[pdfStore] selectPdf succeeded", {
          file_id: indexedPdf.file_id,
          filename: indexedPdf.filename
        });
        return indexedPdf;
      } catch {
        this.errorMessage = "Failed to connect to backend.";
        console.log("[pdfStore] selectPdf network error");
        return null;
      } finally {
        this.loading = false;
      }
    }
  }
});
