# MinimalShop

A lightweight .NET 10 Blazor Server shop with **category management**, **product catalog**, **cart/checkout**, and **order management**. Designed to run on a minimal Hetzner VPS using Docker and PostgreSQL.

## Features

- Public storefront with category filter
- Shopping cart and checkout
- Admin area for categories, products, and orders
- PostgreSQL with EF Core migrations
- Docker Compose for one-command deployment

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (local development)
- Docker + Docker Compose (production on Hetzner)

## Local development

1. Start PostgreSQL:

```bash
docker compose up db -d
```

2. Run the app:

```bash
dotnet restore
dotnet run
```

3. Open http://localhost:5080

Default admin password: `changeme` (set `Admin:Password` in `appsettings.json` or environment variables).

## Deploy on Hetzner VPS

1. Install Docker on your server.
2. Clone this repo to the server.
3. Set a strong admin password:

```bash
export Admin__Password='your-secure-password'
```

4. Start everything:

```bash
docker compose up -d --build
```

5. Open `http://YOUR_SERVER_IP:8080`

For production, put Nginx or Caddy in front for HTTPS and reverse proxy to port 8080.

## Project structure

- `Components/Pages/` — storefront and admin UI
- `Models/` — Category, Product, Order entities
- `Data/` — EF Core DbContext
- `Services/` — cart, orders, admin session
- `Migrations/` — database schema

## Security notes

- Change the default admin password before going live.
- Use HTTPS in production.
- Restrict port 5432 so PostgreSQL is not exposed publicly (remove the `ports` mapping on `db` in production).
