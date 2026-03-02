# React Client

## Setup

```bash
npm install
```

## Run

```bash
npm run dev
```

## Default Credentials

- Username: `admin`
- Password: `admin123`

## API Gateway

The client connects to the API Gateway at `http://localhost:5000` which routes to:
- `/auth/*` → Auth Service (port 5001)
- `/products/*` → Product Service (port 5002)
