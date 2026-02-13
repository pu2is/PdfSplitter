# PdfSplitter

This project is now a lightweight web app based on:

- Backend: FastAPI (Python)
- Frontend: Vue 3 + Vite + Tailwind CSS

Current scope only keeps:

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
cd backend
python -m venv .venv
.\.venv\Scripts\Activate.ps1
pip install -r requirements.txt
uvicorn app.main:app --reload --host 0.0.0.0 --port 8000
```

## Run Frontend

```powershell
cd frontend
copy .env.example .env
npm install
npm run dev
```

Frontend: `http://localhost:5173`  
Backend: `http://localhost:8000`

## API

- `POST /api/upload` with form-data key `file`
- `GET /api/files/{file_id}` to stream uploaded PDF
