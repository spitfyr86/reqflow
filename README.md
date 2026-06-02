# ReqFlow

ReqFlow means **Requests + Workflow**. This case-study application demonstrates a small, maintainable request-approval feature across a .NET 8 API, SQL Server schema, React UI, tests, and delivery documentation.

## What It Demonstrates

- Domain rules enforced by the API, not only the UI
- Demo JWT authentication with persisted users and role-based authorization
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
cd ui
npm install
npm run dev
```

The UI runs at `http://localhost:5173`. Copy `.env.example` to `.env` only when the API base URL needs to change. Sign in as a seeded requester, approver, or administrator to exercise each workflow path.

## Tests

```powershell
dotnet test ReqFlow.slnx
cd ui
npm run build
```

## Key Decisions

- A modular monolith is enough for this workflow and easier to operate than microservices.
- Domain methods own state transitions and create history entries.
- JWT claims identify the current user; request bodies cannot spoof requesters or reviewers.
- Demo login keeps the assessment runnable. Production should replace it with Microsoft Entra ID or another OAuth2/OIDC provider.
- `POST /approve` and `POST /reject` express business actions more clearly than a generic status patch.
- EF Core keeps application code concise; SQL scripts make the database design visible to reviewers.
- Mantine provides a consistent UI quickly. Local React state is enough at this scale.

## Known Limitations

- Authentication uses a demo-only seeded-user login and local JWT signing key. Production should use OAuth2/OIDC, externalized secrets, and platform-managed token issuance.
- SQL scripts are the initial database bootstrap path. A production team should add reviewed EF Core migrations for incremental schema rollout.
- Frontend automated tests and API integration tests are documented next steps.
- Pagination, filtering, and richer observability are intentionally deferred.

## AI Tool Use

AI-assisted implementation was used to scaffold code, documentation, and verification steps. The artifacts should still be reviewed like any other contribution: run builds, inspect SQL, exercise smoke tests, and validate design assumptions with the team.

## Next Improvements

Add SQL Server-backed integration tests, frontend component tests, Microsoft Entra ID integration, pagination, Application Insights telemetry, and CI/CD pipelines with environment approvals.

See [docs/RequestApproval_TechnicalDesign.md](docs/RequestApproval_TechnicalDesign.md) for the architecture and trade-offs.
