# PdfSplitter

## Description
PdfSplitter is a small full-stack app for selecting a PDF, indexing it, and storing file metadata in SQLite.

- Frontend: Vue 3 + Vite (`frontend/`)
- Backend: FastAPI (`backend/`)
- Stored metadata: file path, filename, sha256

## Ports
- Frontend dev server: `http://localhost:5173`
- Backend API: `http://localhost:8081`
- Health check: `http://localhost:8081/api/health`

If you change frontend origin, set backend env var `FRONTEND_ORIGIN`.
If you change backend URL, set frontend env var `VITE_API_BASE_URL`.

## How To Run
1. Start backend:
```bash
cd backend
python -m venv .venv
source .venv/bin/activate
pip install -r requirements.txt
FRONTEND_ORIGIN=http://localhost:5173 uvicorn app.main:app --reload --host 0.0.0.0 --port 8081
```
2. Start frontend (new terminal):
```bash
cd frontend
npm install
cp .env.example .env
npm run dev
```
3. Open `http://localhost:5173`.

## How To Dev Further
- Frontend app code: `frontend/src/`
- Backend API code: `backend/app/`
- Run frontend type checks: `cd frontend && npm run type-check`
- Build frontend: `cd frontend && npm run build`
- Run Electron shell (optional): `cd frontend && npm run electron:dev` (after `npm run dev`)
- Reset local SQLite index:
```bash
sqlite3 backend/storage/pdf_index.db "DELETE FROM pdf_files; DELETE FROM sqlite_sequence WHERE name='pdf_files'; VACUUM;"
```
