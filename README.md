# PdfSplitter

A lightweight web application for PDF upload and preview flow, with an Electron desktop shell.

- Backend: FastAPI (Python)
- Frontend: Vue 3 + Vite + Tailwind CSS
- Desktop: Electron

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

## Run Backend (Ubuntu)

```bash
cd /home/chao/projects/PdfSplitter/backend
python3 -m venv .venv
source .venv/bin/activate
python -m pip install -r requirements.txt
python -m uvicorn app.main:app --reload --host 0.0.0.0 --port 8081
```

## Run Frontend (Ubuntu)

```bash
cd /home/chao/projects/PdfSplitter/frontend
cp .env.example .env
npm install
npm run dev
```

## Run Electron (Ubuntu)

```bash
cd /home/chao/projects/PdfSplitter
npm install
npm run electron:dev
```

## URLs

- Frontend: `http://localhost:5173`
- Backend: `http://localhost:8081`
- Health check: `http://localhost:8081/api/health`

## API

- `POST /api/upload` (form-data key: `file`)
- `GET /api/files/{file_id}`
