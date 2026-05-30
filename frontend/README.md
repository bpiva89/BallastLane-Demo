# BallastLane Product Management — Frontend

Angular 19 application. Minimalist design — intentionally focused on demonstrating functionality, not UI/UX.

## Tech Stack

| | Technology |
|---|---|
| Framework | Angular 19 (standalone components) |
| UI Library | Angular Material |
| HTTP | Angular HttpClient |
| Auth | JWT stored in localStorage |
| Styling | SCSS (minimal) |

## Structure

```
src/
├── app/
│   ├── core/
│   │   ├── guards/    authGuard
│   │   ├── models/    product.model, auth-response.model, create-product-request.model
│   │   └── services/  auth.service, product.service
│   └── features/
│       ├── login/          Login page
│       ├── register/       Register page
│       ├── products/       Product list with CRUD
│       └── product-form/   Create/Edit dialog
└── environments/
    ├── environment.ts       (development — points to localhost:8080)
    └── environment.prod.ts  (production — relative /api/v1, proxied by nginx)
```

## Run Locally

```bash
npm install
ng serve
```

App runs at `http://localhost:4200`. The backend must be running at `http://localhost:8080`.

## Build for Production

```bash
npm run build
```

## Run with Docker

See root `docker-compose.yml`.
