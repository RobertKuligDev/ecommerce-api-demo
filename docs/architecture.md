# Architecture Decision Records

## ADR-001: In-memory persistence in public demo

### Status: Accepted

### Context
This repository is the **public demo** of a production E-commerce Platform whose source
code is private for business reasons (modular monolith, outbox pattern, EF Core +
PostgreSQL, Angular 18 SPA, payment gateway integrations).

### Decision
The public demo uses in-memory storage for baskets, products and users. The API
contract (URLs, request/response shapes, error codes, auth scheme) is identical to
production — only the persistence layer differs.

### Consequences
- Reviewers can interact with the same endpoints the production exposes
- No secrets or infrastructure required to run locally
- Production concerns (EF migrations, transactions, distributed locking) are
  intentionally out of scope — see private repo

## ADR-002: Clean Architecture with 4 layers

Domain → Application → Infrastructure → Api

- **Domain**: pure POCOs, no dependencies (Basket aggregate, Product, User)
- **Application**: commands, handlers, Result pattern, repository interfaces
- **Infrastructure**: persistence implementations, JWT service
- **Api**: HTTP endpoints, validators, middleware

Each inner layer knows nothing about outer layers.

## ADR-003: Result pattern instead of exceptions for business errors

### Context
Business errors (basket not found, insufficient stock, invalid credentials) are
expected control flow, not exceptional conditions.

### Decision
All handlers return `Result<T>` with explicit `Error` (code + message + type).
HTTP layer maps `ErrorType` to RFC 7807 ProblemDetails status codes.

### Consequences
- No try/catch for business logic
- Errors are values — composable, testable
- API layer is the only place that knows about HTTP status codes

## ADR-004: Validation at boundary, business rules in handlers

### Context
Two distinct concerns often get conflated:
1. Request shape (email format, quantity > 0)
2. Business rules (stock available, user authorized)

### Decision
- **ValidationFilter** (FluentValidation) at API boundary: shape only
- **Handlers** (Application layer): business rules only

### Consequences
- Validators are simple, composable, testable in isolation
- Handlers don't know about HTTP (no `BadRequest`, no `ModelState`)
- Clear separation: "what did you send?" vs "what does it mean?"