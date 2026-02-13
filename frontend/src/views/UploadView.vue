<script setup>
import { ref } from "vue";
import { useRouter } from "vue-router";
import { FileUp, FileText } from "lucide-vue-next";

const router = useRouter();
const file = ref(null);
const uploading = ref(false);
const errorMessage = ref("");

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:8000";

async function submitUpload() {
  if (!file.value) {
    errorMessage.value = "Please select a PDF file.";
    return;
  }

  errorMessage.value = "";
  uploading.value = true;

  const formData = new FormData();
  formData.append("file", file.value);

  try {
    const response = await fetch(`${apiBaseUrl}/api/upload`, {
      method: "POST",
      body: formData
    });

    if (!response.ok) {
      const payload = await response.json().catch(() => null);
      throw new Error(payload?.detail ?? "Upload failed.");
    }

    const payload = await response.json();
    await router.push({ name: "rendering", params: { fileId: payload.file_id } });
  } catch (error) {
    errorMessage.value = error.message;
  } finally {
    uploading.value = false;
  }
}
</script>

<template>
  <section class="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
    <h1 class="flex items-center gap-2 text-2xl font-semibold text-slate-900">
      <FileText class="h-6 w-6" />
      <span>Upload PDF</span>
    </h1>
    <p class="mt-2 text-sm text-slate-600">
      Upload a PDF and jump to rendering page automatically.
    </p>

    <form class="mt-6 space-y-4" @submit.prevent="submitUpload">
      <input
        class="block w-full rounded-md border border-slate-300 p-2 text-sm"
        type="file"
        accept="application/pdf"
        @change="file = $event.target.files?.[0] ?? null"
      />

      <button
        class="inline-flex items-center gap-2 rounded-md bg-slate-900 px-4 py-2 text-sm font-medium text-white disabled:cursor-not-allowed disabled:opacity-50"
        type="submit"
        :disabled="uploading"
      >
        <FileUp class="h-4 w-4" />
        {{ uploading ? "Uploading..." : "Upload" }}
      </button>
    </form>

    <p v-if="errorMessage" class="mt-4 text-sm text-red-600">{{ errorMessage }}</p>
  </section>
</template>
