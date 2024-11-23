# EShopMicroservices

##### E-commerce backend app.  
---
- Consists of Product, Basket, Discount and Ordering microservices behind a YARP reverse proxy as API Gateway and rate limiter.
- Microservices designed with Vertical architecture. Ordering service designed with Clean Architecture
- Utilizes CQRS pattern with MediatR, Mapster, and FluentValidation.
- Utiliizes Carter library for Minimal API endpoints.
- Includes containerizing services and orchestration utuilizing Docker and Docker Compose.
- Cross-cutting features include Logging, HealthChecks, and global exception handling.
- Inter-service communication includes:
-- Asynchronous RabbitMQ (utilizing MassTransit) with publish/subscribe topic model
-- Synchronouse gRPC with Protobuf messages (for Discount Service communication)
- Basket service utilizes Redis cache
- Ordering service utilizes Domain Driven Design (DDD) entities

---
##### Technologies include: 
* Asp.Net Web API
* Docker 
* RabbitMQ
* MassTransit
* Grpc 
* Yarp API Gateway
* PostgreSQL
* Redis
* SQLite
* SqlServer
* Marten
* Entity Framework Core
* CQRS
* MediatR
* DDD
* Vertical and Clean Architecture 