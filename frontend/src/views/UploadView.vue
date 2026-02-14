<script setup lang="ts">
import { storeToRefs } from "pinia";
import { useRouter } from "vue-router";
import { FilePlus } from "lucide-vue-next";
import { usePdfStore } from "../stores/pdfStore";

const router = useRouter();
const pdfStore = usePdfStore();
const { errorMessage } = storeToRefs(pdfStore);

async function chooseFileAndIndex(): Promise<void> {
  if (!window.desktopBridge?.choosePdfFile) {
    pdfStore.setErrorMessage("Electron bridge is unavailable. Start this page in Electron.");
    return;
  }

  pdfStore.setErrorMessage("");

  try {
    const chosen = await window.desktopBridge.choosePdfFile();
    if (chosen.canceled || !chosen.filePath) {
      return;
    }

    const payload = await pdfStore.indexLocalPdf(chosen.filePath);

    await router.push({ name: "rendering", params: { fileId: String(payload.file_id) } });
  } catch {
    // Errors are handled in pdfStore.errorMessage.
  }
}
</script>

<template>
  <div class="flex h-full w-full flex-col items-center justify-center gap-6 rounded-3xl border-2 border-dashed border-gray-200 bg-gray-50">
    <FilePlus class="h-1/6 w-auto text-gray-200"
      :stroke-width="1" />

    <div class="text-xl py-2 px-8 rounded-full bg-gray-100 text-gray-500 font-bold cursor-pointer hover:bg-gray-100/50 hover:shadow-sm"
      @click="chooseFileAndIndex">
      Select PDF
    </div>

    <p v-if="errorMessage" class="max-w-[80%] text-center text-sm font-medium text-red-500">
      {{ errorMessage }}
    </p>
  </div>
</template>
