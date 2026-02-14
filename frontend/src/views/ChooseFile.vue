<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref } from "vue";
import { useRouter } from "vue-router";
// icons
import { FilePlus, FileText, Check, TriangleAlert, CircleQuestionMark } from "lucide-vue-next";
// store
import { usePdfStore } from "@/stores/pdfStore";
// type
import type { ChosenPdfForm } from "@/types/pdf";
// helper
import { bytesToSizeString } from "@/utils";

const router = useRouter();
const pdfStore = usePdfStore();

const errorMessage = ref("");
const fileInputRef = ref<HTMLInputElement | null>(null);
const isWindowDraggingFile = ref(false);
const isHelperVisible = ref(false);
const hasError = computed((): boolean => errorMessage.value.trim() !== "");

const chosenPdf = ref<ChosenPdfForm>({
  filePath: "",
  fileName: ""
});

const hasChosenPdf = computed((): boolean => {
  const filePath = chosenPdf.value.filePath.trim();
  const fileName = chosenPdf.value.fileName?.trim() ?? "";
  return filePath !== "" && fileName !== "";
});

const chosenPdfSizeLabel = computed((): string => {
  return bytesToSizeString(chosenPdf.value.fileSizeBytes);
});


async function browserFile(): Promise<void> {
  errorMessage.value = "";
  fileInputRef.value?.click();
}

function choosePdfFile(files: FileList | null | undefined, filePath?: string): boolean {
  if (!files || files.length === 0) {
    return false;
  }

  if (files.length > 1) {
    errorMessage.value = "Please choose only one PDF file.";
    return false;
  }

  const file = files[0];
  const isPdfFile = file.type === "application/pdf" || file.name.toLowerCase().endsWith(".pdf");

  if (!isPdfFile) {
    errorMessage.value = "Only PDF files are supported.";
    return false;
  }

  errorMessage.value = "";
  chosenPdf.value = {
    filePath: filePath?.trim() || file.name,
    fileName: file.name,
    fileSizeBytes: file.size
  };
  return true;
}

function onFileSelect(event: Event): void {
  const input = event.target as HTMLInputElement | null;
  const isChosen = choosePdfFile(input?.files, input?.value);
  if (!isChosen && input) {
    input.value = "";
  }
}

function onWindowDragOver(event: DragEvent): void {
  const hasFiles = Array.from(event.dataTransfer?.types ?? []).includes("Files");
  if (!hasFiles) {
    return;
  }

  event.preventDefault();
  isWindowDraggingFile.value = true;

  if (event.dataTransfer) {
    event.dataTransfer.dropEffect = "copy";
  }
}

function onWindowDragLeave(event: DragEvent): void {
  if (event.clientX === 0 && event.clientY === 0) {
    isWindowDraggingFile.value = false;
  }
}

function onWindowDrop(event: DragEvent): void {
  const hasFiles = Array.from(event.dataTransfer?.types ?? []).includes("Files");
  if (!hasFiles) {
    return;
  }

  event.preventDefault();
  isWindowDraggingFile.value = false;
  choosePdfFile(event.dataTransfer?.files);
}

// send request to backend via pinia store
function confirmChoose() {

}

onMounted(() => {
  window.addEventListener("dragover", onWindowDragOver);
  window.addEventListener("dragleave", onWindowDragLeave);
  window.addEventListener("drop", onWindowDrop);
});

onBeforeUnmount(() => {
  window.removeEventListener("dragover", onWindowDragOver);
  window.removeEventListener("dragleave", onWindowDragLeave);
  window.removeEventListener("drop", onWindowDrop);
});
</script>

<template>
  <div class="relative flex h-full w-full flex-col items-center justify-center gap-6 rounded-3xl border-2 border-dashed transition-colors duration-150"
    :class="isWindowDraggingFile ? 'border-green-400 bg-green-50/70' : 'border-gray-200 bg-gray-50'">
    
    <input ref="fileInputRef"
      type="file"
      accept=".pdf,application/pdf"
      class="hidden"
      @change="onFileSelect" />

    <div class="absolute right-5 top-5 z-30">
      <button type="button"
        class="rounded-full p-1 text-gray-400 transition-colors duration-150 hover:text-gray-600"
        aria-label="Show file selection help"
        @click="isHelperVisible = !isHelperVisible">
        <CircleQuestionMark class="size-5" />
      </button>
      <div v-if="isHelperVisible" class="absolute right-0 mt-2 w-64 rounded-xl border border-gray-200 bg-white p-3 shadow-lg">
        <p class="text-sm font-semibold text-gray-700">How to use</p>
        <ul class="mt-2 list-disc space-y-1 pl-5 text-xs text-gray-500">
          <li>Click <span class="mx-1 py-1 px-2 rounded-full bg-gray-100 text-gray-500">Select Pdf</span> to choose one file.</li>
          <li>You can also drag and drop a PDF here.</li>
          <li>Only PDF files are supported.</li>
        </ul>
      </div>
    </div>
    
    <!-- Chosen pdf tag -->
    <div v-if="hasChosenPdf"
      class="pointer-events-none absolute left-1/2 top-1/2 z-20 flex h-10 -translate-x-1/2 -translate-y-[10rem] items-center gap-3 rounded-full bg-white/90 px-4 shadow-sm backdrop-blur-sm">
      <div class="flex items-center justify-center w-6 h-6 bg-green-100 rounded-full">
        <Check class="text-green-500 size-4" />
      </div>
      <span class="text-gray-500">
        {{ chosenPdf.fileName }}
        <span v-if="chosenPdfSizeLabel">({{ chosenPdfSizeLabel }})</span>
      </span>
    </div>
    
    <!-- Icon -->
    <Transition mode="out-in"
      enter-active-class="transition-opacity duration-200 ease-in-out"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition-opacity duration-200 ease-in-out"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0">
      <component :is="hasChosenPdf ? FileText : FilePlus"
        :key="hasChosenPdf ? 'file-text' : 'file-plus'"
        class="size-32 w-auto text-gray-200"
        :stroke-width="1.8" />
    </Transition>
    
    <!-- Button -->
    <div class="flex gap-4">
      <div class="text-xl py-2 px-8 rounded-full bg-gray-100 text-gray-500 font-bold cursor-pointer hover:bg-gray-100/50 hover:shadow-sm transition ease-in-out duration-300"
        @click="browserFile">
        {{ hasChosenPdf ? "Change Pdf" : "Select Pdf" }}
      </div>
      <div class="text-xl py-2 px-8 rounded-full font-bold cursor-pointer hover:shadow-sm transition ease-in-out duration-300"
        :class="hasChosenPdf ? 'bg-green-100 text-green-500 hover:bg-green-100/70' : 'bg-gray-100 text-gray-500 hover:bg-gray-100/50'"
        @click="confirmChoose">
        Confirm
      </div>
    </div>
    
    <!-- Error message -->
    <div class="absolute left-1/2 top-1/2 flex h-10 -translate-x-1/2 -translate-y-[-7rem] items-center gap-4 transition-opacity ease-in-out duration-[400ms]"
      :class="hasError ? 'opacity-100' : 'opacity-0'">
      <TriangleAlert class="size-4 text-red-400" />
      <p class="text-center text-sm font-medium text-red-400">
        {{ errorMessage }}
      </p>
    </div>


  </div>
</template>
