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

Prerequisites: .NET 8 SDK or newer, Node.js, npm, SQL Server LocalDB, and `sqlcmd`.

For the quickest startup, open PowerShell in the repository root and run:

```powershell
.\start-reqflow.ps1
```

The script opens separate visible PowerShell windows for the API and UI. To stop the app, press `Ctrl+C` in both windows.

To reset and reseed LocalDB before starting:

```powershell
.\start-reqflow.ps1 -ResetDatabase
```

The equivalent manual commands are below.

From the repository root, create the local database and load the demo users and requests:

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -b -i sql\001_create_reqflow_schema.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -b -i sql\002_seed_sample_data.sql
```

Start the API in one terminal:

```powershell
dotnet restore
dotnet build ReqFlow.slnx
dotnet run --project api\ReqFlow.Api --launch-profile http
```

Swagger is available at `http://localhost:5000/swagger`.

Start the UI in a second terminal:

```powershell
cd ui
npm install
npm run dev
```

Open `http://localhost:5173`. Sign in as a seeded requester, approver, or administrator to exercise each workflow path.

See [docs/LocalDevelopment.md](docs/LocalDevelopment.md) for a complete request-approval walkthrough, demo identities, database reset instructions, alternate SQL Server configuration, validation commands, and troubleshooting.

## Reviewer Quick Tour

Use the [Run Locally](#run-locally) commands above or the detailed [local development guide](docs/LocalDevelopment.md), then allow about five minutes:

1. Open `http://localhost:5173` and sign in as **Alex Requester**.
2. Create a request and confirm it appears as **Pending** with an audit-history entry.
3. Notice that requester accounts only see their own requests and cannot review them.
4. Sign out and sign in as **Lee Approver**.
5. Confirm the avatar shows the number of pending requests, filter the list by status, then approve Alex's request.
6. Create another request as Alex and reject it as Lee with a reason.
7. Inspect the history entries and try a repeated transition through Swagger to see the `409 Conflict` guardrail.

Useful local endpoints:

- UI: `http://localhost:5173`
- Swagger: `http://localhost:5000/swagger`
- Health check: `http://localhost:5000/health`

The submission intentionally stays small: a modular monolith, domain-owned transitions, SQL Server persistence, claim-derived actors, role-based visibility, and a demo JWT issuer that should be replaced by Entra ID or another OIDC provider in production.

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
- Requesters see only their own requests; approvers and administrators see the complete queue.
- Demo login keeps the assessment runnable. Production should replace it with Microsoft Entra ID or another OAuth2/OIDC provider.
- `POST /approve` and `POST /reject` express business actions more clearly than a generic status patch.
- EF Core keeps application code concise; SQL scripts make the database design visible to reviewers.
- Mantine provides a consistent UI quickly. Local React state is enough at this scale.

## Known Limitations

- Authentication uses a demo-only seeded-user login and local JWT signing key. Production should use OAuth2/OIDC, externalized secrets, and platform-managed token issuance.
- SQL scripts are the initial database bootstrap path. A production team should add reviewed EF Core migrations for incremental schema rollout.
- Frontend automated tests and SQL Server-isolated integration tests are documented next steps.
- Pagination and richer observability are intentionally deferred.

## AI Tool Use

AI-assisted implementation was used to scaffold code, documentation, and verification steps. The artifacts should still be reviewed like any other contribution: run builds, inspect SQL, exercise smoke tests, and validate design assumptions with the team.

## Next Improvements

Add isolated SQL Server-backed integration tests, frontend component tests, Microsoft Entra ID integration, pagination, Application Insights telemetry, and CI/CD pipelines with environment approvals. Refresh tokens, notifications, and multi-step approvals are production-scope options to evaluate only when requirements justify them.

See [docs/RequestApproval_TechnicalDesign.md](docs/RequestApproval_TechnicalDesign.md) for the architecture and trade-offs.
