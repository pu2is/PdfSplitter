# PdfSplitter

A lightweight web application for PDF upload and preview flow.

- Backend: FastAPI (Python)
- Frontend: Vue 3 + Vite + Tailwind CSS

Current scope:

1. Upload PDF
2. Auto redirect to rendering page
3. Back label to return to upload page

## Project Structure

```text
backend/
  app/
    api/
    core/
    services/
    main.py
  requirements.txt
  storage/uploads/
frontend/
  src/
```

## Prerequisites

- Python 3.11+
- Node.js 20+
- npm 10+

## Run Backend

```powershell
cd d:\projects\PdfSplitter\backend
python -m venv .venv
.\.venv\Scripts\python -m pip install -r requirements.txt
.\.venv\Scripts\python -m uvicorn app.main:app --reload --host 0.0.0.0 --port 8000
```

## Run Frontend

```powershell
cd d:\projects\PdfSplitter\frontend
copy .env.example .env
npm install
npm run dev
```

## URLs

- Frontend: `http://localhost:5173`
- Backend: `http://localhost:8000`
- Health check: `http://localhost:8000/api/health`

## API

- `POST /api/upload` (form-data key: `file`)
- `GET /api/files/{file_id}`