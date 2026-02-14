<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref } from "vue";
import { CircleQuestionMark } from "lucide-vue-next";

const isVisible = ref(false);
const helperRef = ref<HTMLElement | null>(null);

function toggleVisible(): void {
  isVisible.value = !isVisible.value;
}

function closeOnOutsideClick(event: MouseEvent): void {
  if (!isVisible.value) {
    return;
  }

  const target = event.target;
  if (!(target instanceof Node)) {
    return;
  }

  if (!helperRef.value?.contains(target)) {
    isVisible.value = false;
  }
}

onMounted(() => {
  window.addEventListener("click", closeOnOutsideClick);
});

onBeforeUnmount(() => {
  window.removeEventListener("click", closeOnOutsideClick);
});
</script>

<template>
  <div ref="helperRef" class="absolute right-5 top-5 z-30">
    <button type="button"
      class="rounded-full p-1 text-gray-400 transition-colors duration-150 hover:text-gray-600"
      aria-label="Show helper"
      @click="toggleVisible">
      <CircleQuestionMark class="size-5" />
    </button>

    <Transition enter-active-class="transition duration-200 ease-out"
      enter-from-class="opacity-0 -translate-y-1 scale-95"
      enter-to-class="opacity-100 translate-y-0 scale-100"
      leave-active-class="transition duration-150 ease-in"
      leave-from-class="opacity-100 translate-y-0 scale-100"
      leave-to-class="opacity-0 -translate-y-1 scale-95">
      <div v-if="isVisible" class="absolute right-0 mt-2 w-64 rounded-xl border border-gray-200 bg-white p-3 shadow-lg">
        <slot />
      </div>
    </Transition>
  </div>
</template>
