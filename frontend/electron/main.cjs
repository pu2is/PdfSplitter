const { app, BrowserWindow, dialog, ipcMain } = require("electron");
const fs = require("fs");
const path = require("path");

async function isDevServerRunning(devUrl) {
  const controller = new AbortController();
  const timeout = setTimeout(() => controller.abort(), 1200);
  try {
    const response = await fetch(devUrl, {
      method: "GET",
      signal: controller.signal
    });
    return response.ok;
  } catch {
    return false;
  } finally {
    clearTimeout(timeout);
  }
}

function showStartupHelp(win) {
  const html = `
    <!doctype html>
    <html lang="en">
      <head>
        <meta charset="utf-8" />
        <title>PdfSplitter Desktop</title>
        <style>
          body { font-family: Segoe UI, Arial, sans-serif; margin: 0; padding: 24px; background: #f8fafc; color: #0f172a; }
          .card { max-width: 760px; margin: 32px auto; background: #fff; border: 1px solid #cbd5e1; border-radius: 12px; padding: 18px 20px; }
          h1 { margin: 0 0 12px; font-size: 22px; }
          p { margin: 8px 0; color: #334155; }
          code { background: #e2e8f0; padding: 2px 6px; border-radius: 6px; }
          pre { background: #0f172a; color: #e2e8f0; border-radius: 8px; padding: 10px 12px; overflow: auto; }
        </style>
      </head>
      <body>
        <div class="card">
          <h1>Renderer Not Started</h1>
          <p>Vite dev server is not running, and no valid local build is available.</p>
          <p>Start with two terminals:</p>
          <pre>cd d:\\projects\\PdfSplitter\\frontend
npm run dev</pre>
          <pre>cd d:\\projects\\PdfSplitter\\frontend
npm run electron:dev</pre>
        </div>
      </body>
    </html>
  `;
  win.loadURL(`data:text/html;charset=utf-8,${encodeURIComponent(html)}`);
}

async function createMainWindow() {
  const win = new BrowserWindow({
    width: 1280,
    height: 840,
    minWidth: 960,
    minHeight: 640,
    webPreferences: {
      contextIsolation: true,
      nodeIntegration: false,
      preload: path.join(__dirname, "preload.cjs")
    }
  });

  process.env.VITE_DEV_SERVER_URL =
    process.env.VITE_DEV_SERVER_URL || "http://localhost:5173";
  const devUrl = process.env.VITE_DEV_SERVER_URL;
  const isDev =
    process.env.ELECTRON_DEV === "true" ||
    process.env.NODE_ENV === "development";

  const canUseDevServer = isDev || (await isDevServerRunning(devUrl));
  if (canUseDevServer) {
    await win.loadURL(devUrl);
    return;
  }

  const distEntry = path.join(__dirname, "..", "dist", "index.html");
  if (fs.existsSync(distEntry)) {
    try {
      await win.loadFile(distEntry);
      return;
    } catch {
      showStartupHelp(win);
      return;
    }
  }

  showStartupHelp(win);
}

app.whenReady().then(() => {
  ipcMain.handle("file:choosePdf", async () => {
    const result = await dialog.showOpenDialog({
      title: "Choose PDF File",
      properties: ["openFile"],
      filters: [{ name: "PDF Files", extensions: ["pdf"] }]
    });

    if (result.filePaths.length === 0) {
      return null;
    }

    const filePath = result.filePaths[0];
    const fileSizeBytes = fs.statSync(filePath).size;
    return {
      filePath,
      fileName: path.basename(filePath),
      fileSizeBytes
    };
  });

  createMainWindow();

  app.on("activate", () => {
    if (BrowserWindow.getAllWindows().length === 0) {
      createMainWindow();
    }
  });
});

app.on("window-all-closed", () => {
  if (process.platform !== "darwin") {
    app.quit();
  }
});
