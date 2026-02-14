export function bytesToSizeString(bytes: number | undefined): string {
  if (bytes === undefined || !Number.isFinite(bytes) || bytes < 0) {
    return "";
  }

  if (bytes < 1024) {
    return `${bytes} B`;
  }

  const kib = bytes / 1024;
  if (kib < 1024) {
    const digits = kib >= 100 ? 0 : kib >= 10 ? 1 : 2;
    return `${kib.toFixed(digits)} KB`;
  }

  const mib = kib / 1024;
  if (mib < 1024) {
    const digits = mib >= 100 ? 0 : mib >= 10 ? 1 : 2;
    return `${mib.toFixed(digits)} MB`;
  }

  const gib = mib / 1024;
  if (gib < 1024) {
    const digits = gib >= 100 ? 0 : gib >= 10 ? 1 : 2;
    return `${gib.toFixed(digits)} GB`;
  }

  const tib = gib / 1024;
  const digits = tib >= 100 ? 0 : tib >= 10 ? 1 : 2;
  return `${tib.toFixed(digits)} TB`;
}
