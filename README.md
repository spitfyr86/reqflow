# ReqFlow

ReqFlow means **Requests + Workflow**. This case-study application demonstrates a small, maintainable request-approval feature across a .NET 8 API, SQL Server schema, React UI, tests, and delivery documentation.

## What It Demonstrates

- Domain rules enforced by the API, not only the UI
- Clean Architecture-inspired separation without microservices
- SQL Server persistence with optimistic concurrency
- React + TypeScript + Mantine screens for a complete user flow
- Testing, release, risk, rollback, and team-delivery thinking

## Stack

- API: .NET 8, ASP.NET Core controllers, EF Core, SQL Server, Swagger
- UI: React, TypeScript, Vite, Mantine, React Router
- Tests: xUnit
- Deployment target: Azure App Service, Azure Static Web Apps, Azure SQL Database

## Structure

```text
api/        .NET solution projects and tests
docs/       analysis, design, API, testing, release, and team plans
sql/        schema and sample-data scripts
ui/         Vite React application
```

## Run Locally

Prerequisites: .NET 8 SDK or newer, Node.js, npm, and SQL Server LocalDB or a configurable SQL Server instance.

1. Run `sql/001_create_reqflow_schema.sql`, then optionally `sql/002_seed_sample_data.sql`.
2. Override `ConnectionStrings__ReqFlow` if LocalDB is not appropriate.
3. Start the API:

```powershell
dotnet restore
dotnet build ReqFlow.slnx
dotnet run --project api/ReqFlow.Api
```

Swagger is available at `http://localhost:5000/swagger`.

4. Start the UI:

```powershell
cd ui/reqflow-ui
npm install
npm run dev
```

The UI runs at `http://localhost:5173`. Copy `.env.example` to `.env` only when the API base URL needs to change.

## Tests

```powershell
dotnet test ReqFlow.slnx
cd ui/reqflow-ui
npm run build
```

## Key Decisions

- A modular monolith is enough for this workflow and easier to operate than microservices.
- Domain methods own state transitions and create history entries.
- `POST /approve` and `POST /reject` express business actions more clearly than a generic status patch.
- EF Core keeps application code concise; SQL scripts make the database design visible to reviewers.
- Mantine provides a consistent UI quickly. Local React state is enough at this scale.

## Known Limitations

- Authentication is simulated through `requestedBy` and `changedBy` fields. Production should use OAuth2/OIDC, JWT validation, and role-based authorization.
- SQL scripts are the initial database bootstrap path. A production team should add reviewed EF Core migrations for incremental schema rollout.
- Frontend automated tests and API integration tests are documented next steps.
- Pagination, filtering, and richer observability are intentionally deferred.

## AI Tool Use

AI-assisted implementation was used to scaffold code, documentation, and verification steps. The artifacts should still be reviewed like any other contribution: run builds, inspect SQL, exercise smoke tests, and validate design assumptions with the team.

## Next Improvements

Add integration tests with a disposable SQL Server database, frontend component tests, authenticated identities, pagination, Application Insights telemetry, and CI/CD pipelines with environment approvals.

See [docs/RequestApproval_TechnicalDesign.md](docs/RequestApproval_TechnicalDesign.md) for the architecture and trade-offs.
