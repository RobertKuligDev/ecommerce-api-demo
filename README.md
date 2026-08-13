# 🛒 Ecom.API — Public Demo

> Production-grade e-commerce API. Same contract, simplified persistence.  
> Spin it up in seconds and start playing.

---

## 🚀 Quick start

```bash
docker compose up
# Swagger UI available at: http://localhost:5000/swagger
```

---

## 🧪 Try it yourself

```bash
# 1. Login and grab a token
TOKEN=$(curl -s -X POST http://localhost:5000/api/accounts/login \
  -H "Content-Type: application/json" \
  -d '{"email":"demo@example.com","password":"Demo123!"}' | jq -r .token)

# 2. Create a new basket
BASKET=$(curl -s -X POST http://localhost:5000/api/baskets \
  -H "Authorization: Bearer $TOKEN" | jq -r .id)

# 3. Add an item
curl -X POST http://localhost:5000/api/baskets/$BASKET/items \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"productId":1,"quantity":2}'

# 4. View your basket
curl http://localhost:5000/api/baskets/$BASKET \
  -H "Authorization: Bearer $TOKEN"
```

---

## ⚖️ Demo vs Production

| Concern | Public demo | Production |
|---------|-------------|------------|
| 💾 **Persistence** | In-memory dictionaries | `EF Core` + `PostgreSQL` |
| 🧱 **Architecture** | 4-layer Clean Architecture | Modular monolith + outbox |
| 🎨 **Frontend** | None (`Swagger UI`) | `Angular 18 SPA` |
| 💳 **Payment** | None | `Stripe` + `BLIK` |
| 🚢 **Deployment** | `Docker Compose` | `Kubernetes` + `ArgoCD` |

📄 Full architectural decisions: [`docs/architecture.md`](docs/architecture.md) 

---

## 🧰 Tech Stack

| Layer | Technologies |
|-------|--------------|
| **Runtime** |	`.NET 8` |
| **API style** | `Minimal APIs` |
| **Validation** | `FluentValidation` |
| **Logging** |	`Serilog` |
| **Testing** |	`xUnit` |
| **Container** | `Docker` |

---

## 📁 Project structure

```text
src/
├── EcomDemo.Domain/          # Pure POCOs, no dependencies
├── EcomDemo.Application/     # Commands, handlers, Result pattern
├── EcomDemo.Infrastructure/  # In-memory repos, JWT service
└── EcomDemo.Api/             # HTTP endpoints, validators, middleware

tests/
└── EcomDemo.Tests/           # Unit tests with fake repositories
```

## 🛠️ Local development
```bash
dotnet restore
dotnet build --warnaserror
dotnet test
dotnet run --project src/EcomDemo.Api
```

---

## 🔁 CI/CD
GitHub Actions runs build + test + Docker smoke test on every push to main.

---

## 🤝 Contributing

This is a **demo repository** showcasing architectural patterns. Contributions welcome for:
- Additional features (orders, wishlist, reviews)
- Integration tests (currently only unit tests)
- Documentation improvements

For production features, see the private repo (not public).

---

## 📜 License
MIT — feel free to use, modify, and distribute.