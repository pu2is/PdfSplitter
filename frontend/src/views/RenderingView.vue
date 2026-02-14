<script setup lang="ts">
import { computed } from "vue";
import { useRouter } from "vue-router";
import { ArrowLeft, ScanLine } from "lucide-vue-next";

const props = defineProps<{
  fileId: string;
}>();

const router = useRouter();
const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:8081";

const pdfUrl = computed(() => `${apiBaseUrl}/api/files/local/${props.fileId}`);

function backToUpload(): void {
  void router.push({ name: "upload" });
}
</script>

<template>
  <section class="space-y-4 rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
    <div class="flex items-center justify-between">
      <span class="inline-flex items-center gap-2 text-xl font-semibold text-slate-900">
        <ScanLine class="h-5 w-5" />
        <span> rendering </span>
      </span>
      <label
        class="inline-flex cursor-pointer items-center gap-1 text-sm font-medium text-blue-700 underline underline-offset-4"
        @click="backToUpload"
      >
        <ArrowLeft class="h-4 w-4" />
        Back
      </label>
    </div>

    <p class="text-sm text-slate-600">
      Client-side PDF preview keeps backend lightweight.
    </p>

    <iframe
      class="h-[70vh] w-full rounded-md border border-slate-300"
      :src="pdfUrl"
      title="PDF rendering"
    />
  </section>
</template>
