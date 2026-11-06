# Graph Report - MinimalShop  (2026-11-06)

## Corpus Check
- Corpus is ~10,121 words - fits in a single context window. You may not need a graph.

## Summary
- 549 nodes · 765 edges · 46 communities (39 shown, 7 thin omitted)
- Extraction: 92% EXTRACTED · 8% INFERRED · 1% AMBIGUOUS · INFERRED: 61 edges (avg confidence: 0.88)
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- Community 0
- Community 1
- Community 2
- Community 3
- Community 4
- Community 5
- Community 6
- Community 7
- Community 8
- Community 9
- Community 10
- Community 11
- Community 12
- Community 13
- Community 14
- Community 15
- Community 16
- Community 17
- Community 18
- Community 19
- Community 20
- Community 21
- Community 22
- Community 23
- Community 24
- Community 25
- Community 26
- Community 27
- Community 28
- Community 29
- Community 30
- Community 31
- Community 32
- Community 33
- Community 34
- Community 35
- Community 36
- Community 37
- Community 38
- Community 39
- Community 40
- Community 41
- Community 42

## God Nodes (most connected - your core abstractions)
1. `Product` - 26 edges
2. `Order` - 25 edges
3. `ShopService` - 23 edges
4. `CartService` - 21 edges
5. `Category` - 17 edges
6. `ShopDbContext` - 16 edges
7. `OrderItem` - 15 edges
8. `PaymentService` - 15 edges
9. `MinimalShop.Data` - 11 edges
10. `MinimalShop.Models` - 11 edges

## Surprising Connections (you probably didn't know these)
- `ShopDbContext` --references--> `Category`  [EXTRACTED]
  Data/ShopDbContext.cs → Models/Category.cs
- `ShopDbContext` --references--> `Order`  [EXTRACTED]
  Data/ShopDbContext.cs → Models/Order.cs
- `ShopDbContext` --references--> `OrderItem`  [EXTRACTED]
  Data/ShopDbContext.cs → Models/OrderItem.cs
- `ShopDbContext` --references--> `Product`  [EXTRACTED]
  Data/ShopDbContext.cs → Models/Product.cs
- `ShopService` --references--> `ShopDbContext`  [EXTRACTED]
  Services/ShopService.cs → Data/ShopDbContext.cs

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **Hetzner Docker Deployment Stack (compose + Postgres + Nginx/HTTPS)** — readme_hetzner_minimal_vps, readme_docker_compose_deployment, readme_postgresql, readme_nginx_https_reverse_proxy, readme_env_file_configuration [EXTRACTED 1.00]
- **Default Blazor favicon icon composition (flame spiral + @ symbol serving as site brand icon)** — wwwroot_favicon_favicon, wwwroot_favicon_blazor_logo, wwwroot_favicon_at_symbol, wwwroot_favicon_blazor_web_app_branding [INFERRED 0.85]
- **Lighting product catalog imagery cluster** — wwwroot_images_products_ml2170_desk_lamp_jpg, concept_ml2170_desk_lamp_product, concept_lighting_fixture_category, concept_broken_ceiling_light_fixture [INFERRED 0.85]
- **65W wall charger product presentation** — wwwroot_images_products_spark_65w_charger_product_photo, concept_spark_65w_charger_product, concept_wall_power_adapter, concept_european_two_pin_plug [INFERRED 0.85]
- **USB-C charging ecosystem (cable, connector, power bank)** — usb_c_charging_cable_concept, usb_c_connector_concept, portable_power_bank_concept [INFERRED 0.85]
- **Vintage Belarusian porcelain tea service defined by glaze and city crests** — wwwroot_images_products_porcelain_tea_set_4_porcelain_tea_set, wwwroot_images_products_porcelain_tea_set_4_crimson_airbrushed_glaze, wwwroot_images_products_porcelain_tea_set_4_city_crests [INFERRED 0.85]
- **Storefront Shopping Flow (catalog -> cart -> checkout -> order)** — readme_public_storefront, readme_product_catalog, readme_cart_checkout, readme_order_management [INFERRED 0.85]
- **Data Layer Stack (Models -> ShopDbContext -> EF Migrations -> PostgreSQL)** — readme_models_directory, readme_shop_dbcontext, readme_ef_core_migrations, readme_postgresql [INFERRED 0.95]

## Communities (46 total, 7 thin omitted)

### Community 0 - "Community 0"
Cohesion: 0.11
Nodes (8): MinimalShop.Migrations, MinimalShop.Data, Microsoft.EntityFrameworkCore, InitialCreate, ProductSpecs, ProductImage, OrderPayment, ShopDbContextModelSnapshot

### Community 1 - "Community 1"
Cohesion: 0.05
Nodes (32): Microsoft.Extensions.Options, AuthorityData, Authority, Code, PaymentService, BaseUrl, IsConfigured, Settings (+24 more)

### Community 2 - "Community 2"
Cohesion: 0.06
Nodes (21): Microsoft.AspNetCore.Authorization, MinimalShop.Services, MinimalShop.Models, MinimalShop, Microsoft.AspNetCore.Components.Authorization, Microsoft.AspNetCore.Components.Forms, Microsoft.AspNetCore.Components.Routing, Microsoft.AspNetCore.Components.Web (+13 more)

### Community 3 - "Community 3"
Cohesion: 0.06
Nodes (35): condition, environment, healthcheck, image, restart, volumes, db, Admin__LockoutMinutes (+27 more)

### Community 5 - "Community 5"
Cohesion: 0.10
Nodes (24): Admin Auth Service, Admin Panel (/admin), Admin Password Setting (Admin__Password), Blazor Server, Cart / Checkout, Category Management, Components/Pages/ Directory (storefront and admin UI), Configuration Settings (appsettings.json / environment variables) (+16 more)

### Community 6 - "Community 6"
Cohesion: 0.10
Nodes (11): Microsoft.JSInterop, CartLine, LineTotal, Name, Price, ProductId, Quantity, CartService (+3 more)

### Community 7 - "Community 7"
Cohesion: 0.12
Nodes (9): AdminAuthOptions, LockoutMinutes, MaxFailedAttempts, Password, AdminAuthService, AttemptState, FailedCount, LockoutUntil (+1 more)

### Community 8 - "Community 8"
Cohesion: 0.10
Nodes (19): DeleteProductAsync, EditProduct, NewProduct, OnInitializedAsync, AdminNav, Category, EditForm, InputCheckbox (+11 more)

### Community 9 - "Community 9"
Cohesion: 0.12
Nodes (16): Product, Category, CategoryId, Color, CreatedAt, Description, HeightCm, Id (+8 more)

### Community 10 - "Community 10"
Cohesion: 0.13
Nodes (15): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, applicationUrl, commandName (+7 more)

### Community 11 - "Community 11"
Cohesion: 0.13
Nodes (15): Order, CreatedAt, CustomerEmail, CustomerName, CustomerPhone, Id, Items, OrderNumber (+7 more)

### Community 12 - "Community 12"
Cohesion: 0.14
Nodes (13): DeleteCategoryAsync, EditCategory, NewCategory, OnInitializedAsync, AdminNav, Category, EditForm, InputCheckbox (+5 more)

### Community 13 - "Community 13"
Cohesion: 0.18
Nodes (6): DatabaseInitializer, ShopDbContext, Categories, OrderItems, Orders, Products

### Community 14 - "Community 14"
Cohesion: 0.20
Nodes (8): Dispose, OnInitializedAsync, Category, IDisposable, NavLink, ShopDbContext, ShopSettings, SearchAsync

### Community 15 - "Community 15"
Cohesion: 0.20
Nodes (9): OnInitializedAsync, PlaceOrderAsync, EditForm, InputText, InputTextArea, PageTitle, DataAnnotationsValidator, route:/checkout (+1 more)

### Community 16 - "Community 16"
Cohesion: 0.20
Nodes (10): OrderItem, Id, LineTotal, Order, OrderId, Product, ProductId, ProductName (+2 more)

### Community 17 - "Community 17"
Cohesion: 0.22
Nodes (5): OnInitializedAsync, Microsoft.AspNetCore.Authorization, PageTitle, OnInitialized, route:/admin

### Community 18 - "Community 18"
Cohesion: 0.22
Nodes (8): LoadProductsAsync, OnInitialized, OnParametersSetAsync, Category, NavLink, PageTitle, Product, route:/shop

### Community 19 - "Community 19"
Cohesion: 0.22
Nodes (8): Category, CreatedAt, Description, Id, IsActive, Name, Products, Slug

### Community 20 - "Community 20"
Cohesion: 0.25
Nodes (7): AuthorizeRouteView, CascadingAuthenticationState, FocusOnNavigate, Found, NotAuthorized, RedirectToLogin, Router

### Community 21 - "Community 21"
Cohesion: 0.25
Nodes (6): Dispose, OnInitializedAsync, IDisposable, PageTitle, UpdateQty, route:/cart

### Community 22 - "Community 22"
Cohesion: 0.32
Nodes (6): handleReconnectStateChanged(), reconnectModal, resumeButton, retry(), retryButton, retryWhenDocumentBecomesVisible()

### Community 23 - "Community 23"
Cohesion: 0.50
Nodes (8): Cyrillic city coat-of-arms crests (Minsk and Pinsk) decorating lid band and cup interior, Crimson-to-black airbrushed gradient glaze with white porcelain accents and gold trim, Porcelain tea set with teapot and teacup, Studio product photography style for e-commerce catalog listing, Porcelain teacup with matching handle and gold-rimmed foot, Porcelain teapot with sculpted lid finial and white spout and handle, Product photo: porcelain tea set (teapot with lid and teacup), Vintage mid-century (Soviet-era Belarusian) porcelain tableware style

### Community 24 - "Community 24"
Cohesion: 0.29
Nodes (6): OnParametersSetAsync, AdminNav, OrderStatus, PageTitle, UpdateStatusAsync, route:/admin/orders/{OrderId:int}

### Community 25 - "Community 25"
Cohesion: 0.29
Nodes (6): AddToCartAsync, FormatWeight, OnInitializedAsync, PageTitle, ShopSettings, route:/product/{Id:int}

### Community 26 - "Community 26"
Cohesion: 0.57
Nodes (7): Bamboo mat / slatted wooden surface background, Broken ceiling light fixture with exposed wiring and sockets, Two frosted LED light bulbs (A-type), one in socket and one detached, Lighting fixture product category (e-commerce shop imagery), Desk lamp product ml2170 (catalog listing named by filename), Shattered frosted glass shards scattered across the floor, Product image ml2170-desk-lamp.jpg

### Community 27 - "Community 27"
Cohesion: 0.43
Nodes (4): load(), parse(), unwatch(), watch()

### Community 28 - "Community 28"
Cohesion: 0.33
Nodes (5): OnInitializedAsync, AdminNav, Order, PageTitle, route:/admin/orders

### Community 29 - "Community 29"
Cohesion: 0.60
Nodes (6): E-commerce catalog product image, Fitness band, Fitness and activity tracking, Samsung Galaxy Fit (brand identification), Smartwatch / wearable device, galaxyfit3-watch.jpg product photo

### Community 30 - "Community 30"
Cohesion: 0.80
Nodes (5): Charging accessories / electronics category, Portable power bank with LED indicators, USB-C charging cable (1m braided), USB Type-C connector, Product photo: typec-cable-1m.jpg

### Community 31 - "Community 31"
Cohesion: 0.40
Nodes (4): OnInitialized, PageTitle, route:/Error, System.Diagnostics

### Community 32 - "Community 32"
Cohesion: 0.40
Nodes (3): OnInitializedAsync, PageTitle, route:/order/{Id:int}

### Community 33 - "Community 33"
Cohesion: 0.40
Nodes (5): Anker electronics brand, Anker 20000mAh portable power bank (black, with Micro-USB and USB output charging cables), Consumer electronics and accessories category, Portable battery charger / power bank product category, Anker PowerPort 20000mAh power bank product photo (wwwroot/images/products/anker-powerbank-20000.jpg)

### Community 34 - "Community 34"
Cohesion: 0.60
Nodes (5): E-commerce product catalog imagery, European (Europlug) two-pin wall plug, Spark 65W Charger product, Wall-mount USB power adapter, Product photo: white wall charger with European two-pin plug (spark-65w-charger.jpg)

### Community 35 - "Community 35"
Cohesion: 0.40
Nodes (4): net10.0, Microsoft.EntityFrameworkCore.Design (10.0.11), Npgsql.EntityFrameworkCore.PostgreSQL (10.0.3), Microsoft.NET.Sdk.Web

### Community 36 - "Community 36"
Cohesion: 0.50
Nodes (5): At (@) Symbol Motif, Blazor Framework Logo (purple flame spiral), Blazor Web App Site Branding (default template icon), MinimalShop Favicon (favicon.png), MinimalShop Site Concept

### Community 37 - "Community 37"
Cohesion: 0.50
Nodes (3): HeadOutlet, ImportMap, Routes

### Community 38 - "Community 38"
Cohesion: 0.83
Nodes (4): Headphones product category (consumer audio equipment), Product photo file sony-headphone-whch520.jpg: flat-lay studio shot of wired over-ear headphones with wooden earcups on tan background, Sony WH-CH520 on-ear wireless headphones implied by the product filename, Depicted product: premium wired over-ear headphones with wood-grain earcups, black leather earpads and padded headband (no visible brand/model text; resembles wooden audiophile headphones)

### Community 39 - "Community 39"
Cohesion: 0.67
Nodes (4): Product photo: e-commerce catalog image (xiaomi-kitchen-scale.jpg), Depicted object: digital kitchen scale (round weighing platform, LCD display, On/Tare/Off button, g/oz unit toggle), Depicted brand attribute: 'Escali' logo printed on display panel (matches Escali Pico-style scale, not Xiaomi), Category concept: digital kitchen scales for weighing ingredients in cooking and baking

## Ambiguous Edges - Review These
- `Product photo file sony-headphone-whch520.jpg: flat-lay studio shot of wired over-ear headphones with wooden earcups on tan background` → `Sony WH-CH520 on-ear wireless headphones implied by the product filename`  [AMBIGUOUS]
  wwwroot/images/products/sony-headphone-whch520.jpg · relation: references
- `Crimson-to-black airbrushed gradient glaze with white porcelain accents and gold trim` → `Cyrillic city coat-of-arms crests (Minsk and Pinsk) decorating lid band and cup interior`  [AMBIGUOUS]
  wwwroot/images/products/porcelain-tea-set-4.jpg · relation: conceptually_related_to
- `Product image ml2170-desk-lamp.jpg` → `Desk lamp product ml2170 (catalog listing named by filename)`  [AMBIGUOUS]
  wwwroot/images/products/ml2170-desk-lamp.jpg · relation: references
- `Desk lamp product ml2170 (catalog listing named by filename)` → `Broken ceiling light fixture with exposed wiring and sockets`  [AMBIGUOUS]
  wwwroot/images/products/ml2170-desk-lamp.jpg · relation: conceptually_related_to

## Knowledge Gaps
- **190 isolated node(s):** `ImportMap`, `HeadOutlet`, `Routes`, `LayoutComponentBase`, `NavMenu` (+185 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 308 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **7 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `MinimalShop.Data` connect `Community 0` to `Community 2`, `Community 14`?**
  _High betweenness centrality (0.202) - this node is a cross-community bridge._
- **What connects `ImportMap`, `HeadOutlet`, `Routes` to the rest of the system?**
  _190 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Community 0` be split into smaller, more focused modules?**
  _Cohesion score 0.11382113821138211 - nodes in this community are weakly interconnected._
- **Why does `ShopDbContext` connect `Community 13` to `Community 0`, `Community 1`, `Community 4`, `Community 9`, `Community 43`, `Community 11`, `Community 16`, `Community 19`?**
  _High betweenness centrality (0.097) - this node is a cross-community bridge._
- **Should `Community 1` be split into smaller, more focused modules?**
  _Cohesion score 0.05121951219512195 - nodes in this community are weakly interconnected._

### Low-confidence Hints
_AMBIGUOUS edges — the extractor was unsure. Verify before acting on these._

- **What is the exact relationship between `Product photo file sony-headphone-whch520.jpg: flat-lay studio shot of wired over-ear headphones with wooden earcups on tan background` and `Sony WH-CH520 on-ear wireless headphones implied by the product filename`?**
  _Edge tagged AMBIGUOUS (relation: references) - confidence is low._
- **What is the exact relationship between `Crimson-to-black airbrushed gradient glaze with white porcelain accents and gold trim` and `Cyrillic city coat-of-arms crests (Minsk and Pinsk) decorating lid band and cup interior`?**
  _Edge tagged AMBIGUOUS (relation: conceptually_related_to) - confidence is low._