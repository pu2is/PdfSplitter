<script setup lang="ts">
defineProps<{
  title: string;
  subtitle?: string;
}>();
</script>

<template>
  <section class="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
    <h1 class="flex items-center gap-2 text-2xl font-semibold text-slate-900">
      <slot name="titleIcon" />
      <span>{{ title }}</span>
    </h1>
    <p v-if="subtitle" class="mt-2 text-sm text-slate-600">{{ subtitle }}</p>
    <div class="mt-6">
      <slot />
    </div>
  </section>
</template>
