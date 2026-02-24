# EShopMicroservices

A production-grade e-commerce backend built with .NET 8, demonstrating microservices architecture patterns including Clean Architecture, CQRS, Domain-Driven Design, and event-driven communication.

---

## Architecture Overview

The solution is composed of four core microservices behind a YARP reverse proxy API gateway, with a Razor Pages web frontend client. Services communicate both synchronously (gRPC) and asynchronously (RabbitMQ).

```
Shopping.Web (Razor Pages)
        │
        ▼
YarpApiGateway (YARP Reverse Proxy + Rate Limiter)
        │
  ┌─────┼─────┬─────────┐
  ▼     ▼     ▼         ▼
Catalog Basket Ordering  Discount
 (Marten) (Marten  (EF Core /  (EF Core /
 + Pg)   + Redis  SQL Server) SQLite)
          + gRPC ──────────────▲
          + RabbitMQ ──────────▶ Ordering
```

---

## Services

### Catalog API
Manages the product catalog using Vertical Slice Architecture.

- **Database:** PostgreSQL via Marten (document store)
- **Endpoints:** `GET/POST/PUT/DELETE /products`, `GET /products/category/{category}`, `GET /products/{id}`
- **Patterns:** CQRS with MediatR, Carter minimal API endpoints, FluentValidation, Mapster
- **Port (Docker):** `6000`

### Basket API
Manages shopping carts with a Redis caching layer and gRPC-based discount deduction at checkout.

- **Database:** PostgreSQL via Marten, Redis distributed cache
- **Endpoints:** `GET/POST /basket`, `DELETE /basket/{userName}`, `POST /basket/checkout`
- **Patterns:** Decorator pattern (Scrutor) for transparent caching, CQRS, MassTransit publisher
- **Port (Docker):** `6001`

### Discount gRPC Service
Provides product discount lookups over gRPC with synchronous protobuf messaging.

- **Database:** SQLite via Entity Framework Core
- **Proto Operations:** `GetDiscount`, `CreateDiscount`, `UpdateDiscount`, `DeleteDiscount`
- **Port (Docker):** `6002`

### Ordering API
Processes orders using Clean Architecture with four layers: Domain, Application, Infrastructure, and API.

- **Database:** SQL Server via Entity Framework Core
- **Endpoints:** `GET/POST/PUT/DELETE /orders`, `GET /orders/{orderName}`, `GET /orders/customer/{customerId}`
- **Patterns:** DDD aggregates, domain events, integration events via MassTransit consumer, feature flags
- **Port (Docker):** `6003`

### YARP API Gateway
Single entry point for all client requests with route-based proxying and rate limiting.

- **Rate Limiting:** Fixed window (5 requests / 10 seconds) applied to the Ordering service route
- **Routes:** `/catalog-service/**`, `/basket-service/**`, `/ordering-service/**`
- **Port (Docker):** `6004` (HTTP), `6064` (HTTPS)

### Shopping.Web
Razor Pages frontend consuming all backend services via Refit typed HTTP clients through the gateway.

- **Port (Docker):** `5005`

---

## Key Patterns & Technologies

| Concern | Implementation |
|---|---|
| API Style | ASP.NET Minimal API + Carter |
| CQRS | MediatR |
| Object Mapping | Mapster |
| Validation | FluentValidation (pipeline behavior) |
| Logging | MediatR pipeline behavior + structured logging |
| Async Messaging | RabbitMQ via MassTransit (publish/subscribe) |
| Sync Communication | gRPC with Protobuf |
| Caching | Redis (StackExchange.Redis) |
| ORM (PostgreSQL) | Marten |
| ORM (SQL Server) | Entity Framework Core |
| DDD | Aggregates, Value Objects, Domain Events |
| Clean Architecture | Ordering service: Domain / Application / Infrastructure / API |
| Vertical Slice | Catalog and Basket services |
| Health Checks | AspNetCore.HealthChecks (Postgres, Redis, SQL Server) |
| Exception Handling | Global `IExceptionHandler` with ProblemDetails |
| Containerization | Docker + Docker Compose |
| Gateway | YARP Reverse Proxy |
| Rate Limiting | ASP.NET Core fixed-window rate limiter |
| Feature Flags | Microsoft.FeatureManagement (`OrderFullfilment` flag) |

---

## Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (for local development)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code

### Run with Docker Compose

Clone the repository and start all services:

```bash
git clone https://github.com/acisternino/eshop-microservices.git
cd eshop-microservices/src/eshop-microservices

docker compose up --build
```

All infrastructure (PostgreSQL, SQL Server, Redis, RabbitMQ) and application containers will start automatically.

### Service URLs (Docker)

| Service | HTTP | HTTPS |
|---|---|---|
| Catalog API | http://localhost:6000 | https://localhost:6060 |
| Basket API | http://localhost:6001 | https://localhost:6061 |
| Discount gRPC | http://localhost:6002 | https://localhost:6062 |
| Ordering API | http://localhost:6003 | https://localhost:6063 |
| API Gateway | http://localhost:6004 | https://localhost:6064 |
| RabbitMQ Management | http://localhost:15672 | — |

Default RabbitMQ credentials: `guest` / `guest`

### Local Development

Each service can also run independently. Update the relevant `appsettings.Local.json` or environment variables to point to local infrastructure, then run via the `http` or `https` launch profile.

```bash
cd src/Services/Catalog/Catalog.API
dotnet run --launch-profile https
```

---

## Solution Structure

```
src/
├── ApiGateways/
│   └── YarpApiGateway/          # YARP reverse proxy + rate limiter
├── BuildingBlocks/              # Shared CQRS interfaces, behaviors, exceptions, pagination
├── BuildingBlocks.Messaging/    # MassTransit/RabbitMQ integration events
├── Services/
│   ├── Catalog/
│   │   └── Catalog.API/         # Vertical slice – product catalog
│   ├── Basket/
│   │   └── Basket.API/          # Vertical slice – shopping cart
│   ├── Discount/
│   │   └── Discount.Grpc/       # gRPC discount service
│   └── Ordering/
│       ├── Ordering.API/        # Clean arch – API layer
│       ├── Ordering.Application/# Clean arch – use cases, handlers, events
│       ├── Ordering.Domain/     # Clean arch – aggregates, value objects, domain events
│       └── Ordering.Infrastructure/ # Clean arch – EF Core, interceptors, migrations
└── WebApps/
    └── Shopping.Web/            # Razor Pages frontend via Refit clients
```

---

## Event Flow: Basket Checkout

1. Client calls `POST /basket-service/basket/checkout` via the API Gateway.
2. **Basket API** retrieves the cart, deducts discounts via gRPC call to Discount service, publishes a `BasketCheckoutEvent` to RabbitMQ, and deletes the basket.
3. **Ordering API** consumes the `BasketCheckoutEvent` via MassTransit and creates a new `Order` aggregate, raising an `OrderCreatedEvent`.
4. If the `OrderFullfilment` feature flag is enabled, the domain event triggers an integration event published back to RabbitMQ for downstream fulfillment processing.

---

## License

MIT © 2024 Andrew Cisternino
