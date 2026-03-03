# Articles

REST Web API for an Articles site written in ASP.NET Core.
Users can read blogs, upload and read articles and comment on them.

## Project features

- Fully custom authentication using access and refresh tokens
- Fully custom role-permission-based authorization (RBAC)
- Custom Domain Events system using the Outbox pattern
- PostgreSQL as the main persistent storage
- Caching with Redis
- Local file storage in MinIO with auto-cleaning
- Grafana stack telemetry:
	- Logs to Loki
	- Metrics to Prometheus
	- Distributed traces to Jaeger
- Email testing with MailHog

## How to build & run locally

1. Download Docker for your OS
2. Clone this repository
3. Open a command prompt in the project folder and run the following commands:

```bash
cd docker
docker compose up -d --build
```

- Swagger API is available at `http://localhost:7777/swagger`
- MailHog is available at `http://localhost:5025`
- Admin credentials: email - `admin@rarticles.com`, password - `admin`

This is a fully educational project. The features implemented in this project should not be used in production.
