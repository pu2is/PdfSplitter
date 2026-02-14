/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_BASE_URL?: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}

interface ChoosePdfFileResult {
  filePath: string;
  fileName: string;
}

interface Window {
  desktopBridge?: {
    choosePdfFile: () => Promise<ChoosePdfFileResult | null>;
  };
}
