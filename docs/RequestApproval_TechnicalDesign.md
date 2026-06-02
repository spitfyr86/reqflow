# Request Approval Technical Design

## Architecture

```text
React + Mantine UI
        |
        v
ASP.NET Core Controllers -> RequestService -> Request domain entity
                                  |
                                  v
                         IRequestRepository
                                  |
                                  v
                       EF Core + SQL Server
```

The API is a modular monolith with pragmatic Clean Architecture-inspired projects:

- `ReqFlow.Domain`: entities, statuses, and transition rules
- `ReqFlow.Application`: DTOs, validation, mapping, and workflow orchestration
- `ReqFlow.Infrastructure`: EF Core mappings and SQL Server repository
- `ReqFlow.Api`: HTTP endpoints and ProblemDetails middleware

## Database Design

`Requests` stores current state and a `ROWVERSION` concurrency token. `RequestStatusHistory` stores append-only transition records. Indexes support status filtering, newest-first request browsing, and history lookup. SQL scripts under `/sql` make the schema reviewable.

`Users` stores seeded demo identities, roles, and active status. Requests and history rows retain user foreign keys plus email snapshots so audit output remains readable if a profile later changes.

## Frontend Design

The UI has list, create, and detail pages. A centralized API client handles JSON and ProblemDetails responses. Local component state is sufficient because this is a small server-state-driven feature. React Query is a reasonable next step when caching and invalidation become important.

## Business Logic Placement

The `Request` entity owns transitions so invalid changes cannot bypass rules through a different controller or caller. The application service validates lengths, maps DTOs, and translates domain failures into API-facing exceptions.

## Validation And Errors

- UI validation improves usability.
- Application validation protects field limits and required values.
- Domain validation protects workflow invariants.
- Middleware maps known errors to ProblemDetails: `400`, `404`, and `409`.
- Unexpected errors log server-side and return a generic `500` detail.

## Authentication And Authorization

The local assessment issues JWTs through a demo-login endpoint for seeded users. Request creation and review derive identity from JWT claims, never from request payloads. The application service re-checks active status and reviewer roles against persisted users. Only approvers and administrators can review requests, and self-review is forbidden.

The demo issuer is intentionally replaceable. Production should use Microsoft Entra ID or another OAuth2/OIDC provider, platform-managed signing keys, short-lived access tokens, and environment-specific configuration.

## Trade-Offs

- SQL Server fits relational, transactional workflow data and the expected job stack.
- Modular projects add some setup but keep responsibilities clear as the feature grows.
- Action endpoints communicate approve/reject intent better than generic `PATCH`.
- EF Core improves productivity; SQL scripts preserve production-style schema visibility.
- Mantine gives consistent UI quickly without custom CSS work.
- App Service is simpler than AKS for this workload. AKS is unnecessary without orchestration requirements.
