# MinimalShop

A lightweight .NET 10 Blazor Server shop with **category management**, **product catalog**, **cart/checkout**, and **order management**. Built to run on a small Hetzner VPS with Docker.

## Features

- Public storefront (home, shop, cart, checkout)
- Admin panel (`/admin`) for categories, products, and orders
- PostgreSQL database with EF Core migrations
- Docker Compose deployment (app + Postgres)

## Local development

Requirements: .NET 10 SDK, PostgreSQL (or use Docker for the database only).

```bash
# Start Postgres only
docker compose up db -d

# Run the app
dotnet run
```

Open http://localhost:5000 (or the port shown in the terminal).

Default admin password: `changeme` (set `Admin:Password` in `appsettings.json`).

## Deploy on Hetzner (minimal VPS)

1. SSH into your server and install Docker + Docker Compose plugin.
2. Clone or copy this project to the server.
3. Create `.env` from the example and set strong passwords:

```bash
cp .env.example .env
nano .env
```

4. Build and start:

```bash
docker compose up -d --build
```

5. Open `http://YOUR_SERVER_IP:8080`.

### Optional: Nginx + HTTPS

Put Nginx in front of the app on port 8080 and use Certbot for TLS. Example server block:

```nginx
server {
    listen 80;
    server_name shop.example.com;

    location / {
        proxy_pass http://127.0.0.1:8080;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host $host;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

Blazor Server needs WebSocket support (`Upgrade` headers above).

## Configuration

| Setting | Environment variable | Default |
|---------|---------------------|---------|
| Database | `ConnectionStrings__DefaultConnection` | see `appsettings.json` |
| Admin password | `Admin__Password` | `changeme` |
| Store name | `Shop__StoreName` | `MinimalShop` |

## Project structure

- `Models/` — Category, Product, Order entities
- `Data/` — EF Core `ShopDbContext`, migrations, seed data
- `Services/` — Shop, cart, and admin auth services
- `Components/Pages/` — Storefront and admin UI

## Commands

```bash
dotnet ef migrations add MigrationName
dotnet ef database update
dotnet publish -c Release -o ./publish
```
