# Integrated Test — Multi-Service Pipeline

## Overview

This project demonstrates an event-driven integration between three containerized services that communicate through **RabbitMQ** message queues and share a **PostgreSQL** database. The pipeline ingests a person record, computes their age, and validates the full flow through automated integration tests.

## Architecture

```
[Python test runner]
        │
        │  publishes message (name + birthdate)
        ▼
  [RabbitMQ: ingestion queue]
        │
        ▼
  [Node.js consumer]
        │  inserts person into PostgreSQL
        │  publishes person ID
        ▼
  [RabbitMQ: fill-age queue]
        │
        ▼
  [.NET worker]
        │  fetches person from PostgreSQL
        │  calculates age
        └─ updates person record in PostgreSQL
```

### Services

| Service | Technology | Responsibility |
|---|---|---|
| `app-node` | Node.js + TypeScript | Consumes the ingestion queue, persists the person record to PostgreSQL, and forwards the new ID to the age-calculation queue |
| `app-dotnet` | .NET 8 (BackgroundService) | Consumes the age-calculation queue, fetches the person from PostgreSQL, calculates age based on birthdate, and updates the record |
| `app-python` | Python + FastAPI + pytest | Exposes an HTTP endpoint that triggers end-to-end integration tests, validating the entire pipeline |

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running

## Running the Project

**1. Build and start all containers:**

```bash
docker-compose up --build -d
```

This starts: `rabbitmq`, `postgres`, `app-node`, `app-dotnet`, and `app-python`.

**2. Trigger the integration tests:**

```
GET http://localhost:8000/run-tests
```

The Python service publishes a person payload to RabbitMQ, waits for both workers to process it, then queries PostgreSQL to assert that the record was correctly inserted and the age was accurately calculated.

## Stopping the Project

Stop all containers:

```bash
docker-compose down
```

Stop and remove all persistent volumes (clears RabbitMQ and PostgreSQL data):

```bash
docker-compose down -v
```
