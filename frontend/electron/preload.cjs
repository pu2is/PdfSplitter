const { contextBridge, ipcRenderer } = require("electron");

contextBridge.exposeInMainWorld("desktopBridge", {
  choosePdfFile: () => ipcRenderer.invoke("file:choosePdf")
});
