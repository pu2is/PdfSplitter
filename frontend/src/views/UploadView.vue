<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import { FileSearch, FileText, Fingerprint } from "lucide-vue-next";
import PageCard from "../components/PageCard.vue";
import type { ApiErrorResponse, LocalPdfIndexResponse } from "../types/pdf";

const router = useRouter();
const indexing = ref(false);
const errorMessage = ref("");
const selectedPath = ref("");
const selectedSha256 = ref("");

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:8000";

async function chooseFileAndIndex(): Promise<void> {
  if (!window.desktopBridge?.choosePdfFile) {
    errorMessage.value = "Electron bridge is unavailable. Start this page in Electron.";
    return;
  }

  errorMessage.value = "";
  indexing.value = true;

  try {
    const chosen = await window.desktopBridge.choosePdfFile();
    if (chosen.canceled || !chosen.filePath) {
      return;
    }

    const response = await fetch(`${apiBaseUrl}/api/files/index-local`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({ file_path: chosen.filePath })
    });

    if (!response.ok) {
      const payload = (await response.json().catch(() => null)) as ApiErrorResponse | null;
      throw new Error(payload?.detail ?? "Failed to index local PDF.");
    }

    const payload = (await response.json()) as LocalPdfIndexResponse;
    selectedPath.value = payload.path;
    selectedSha256.value = payload.sha256;

    await router.push({ name: "rendering", params: { fileId: String(payload.file_id) } });
  } catch (error: unknown) {
    errorMessage.value =
      error instanceof Error ? error.message : "Failed to index local PDF.";
  } finally {
    indexing.value = false;
  }
}
</script>

<template>
  <PageCard
    title="Choose Local PDF"
    subtitle="Choose file in Electron, then backend computes SHA256 and stores path/filename/hash in SQLite."
  >
    <template #titleIcon>
      <FileText class="h-6 w-6" />
    </template>

    <div class="space-y-4">
      <button
        class="inline-flex items-center gap-2 rounded-md bg-slate-900 px-4 py-2 text-sm font-medium text-white disabled:cursor-not-allowed disabled:opacity-50"
        type="button"
        :disabled="indexing"
        @click="chooseFileAndIndex"
      >
        <FileSearch class="h-4 w-4" />
        {{ indexing ? "Processing..." : "Choose File" }}
      </button>

      <div v-if="selectedPath" class="rounded-md border border-slate-200 bg-slate-50 p-3 text-sm">
        <p class="break-all text-slate-700"><strong>Path:</strong> {{ selectedPath }}</p>
        <p class="mt-2 break-all text-slate-700">
          <span class="inline-flex items-center gap-1">
            <Fingerprint class="h-4 w-4" />
            <strong>SHA256:</strong>
          </span>
          {{ selectedSha256 }}
        </p>
      </div>
    </div>

    <p v-if="errorMessage" class="mt-4 text-sm text-red-600">{{ errorMessage }}</p>
  </PageCard>
</template>
