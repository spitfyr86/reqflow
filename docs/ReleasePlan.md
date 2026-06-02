# Release Plan

## Azure Shape

- Azure App Service: .NET API
- Azure Static Web Apps or App Service: React UI
- Azure SQL Database: relational persistence
- App Service Configuration and optionally Key Vault: connection strings and secrets
- Application Insights: request telemetry, errors, and dependency monitoring
- GitHub Actions or Azure DevOps: build, test, deploy, and approvals

## Pipeline

1. Restore dependencies.
2. Run `dotnet build` and `dotnet test`.
3. Run `npm ci` and `npm run build`.
4. Publish versioned API and UI artifacts.
5. Review and apply schema scripts with a deployment identity.
6. Deploy API to staging, configure connection string, and smoke test.
7. Deploy UI with `VITE_API_BASE_URL` for the target environment.
8. Promote after approval and monitor Application Insights.

## Configuration

- API: `ConnectionStrings__ReqFlow`, allowed UI origin, logging settings
- UI build: `VITE_API_BASE_URL`
- Secrets belong in platform configuration or Key Vault, never source control.

## Smoke Checks

Confirm API health and Swagger in non-production environments, create a request, list it, inspect history, approve one request, reject another, and confirm invalid repeated transitions return `409`.
