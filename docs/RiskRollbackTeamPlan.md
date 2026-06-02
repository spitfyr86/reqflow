# Risk, Rollback, And Team Plan

## Delivery Risks

| Risk | Mitigation |
| --- | --- |
| Competing approvals | SQL `ROWVERSION`, EF concurrency handling, API `409` |
| Demo authentication used outside local environments | Replace demo JWT issuance with Entra ID or another OIDC provider before production |
| Schema drift | Review versioned SQL scripts and add migration discipline |
| Missed regression | Unit tests, integration tests, UI smoke checks, staged deployment |
| Secret exposure | Store secrets in App Service Configuration or Key Vault |

## Rollback

Use versioned artifacts and deploy through staging slots. For an application issue, swap back to the previous healthy slot or redeploy the last known-good artifact. Prefer backward-compatible database changes; use a reviewed compensating script when schema rollback is unavoidable. Pause traffic-changing rollout steps while investigating data integrity concerns.

## Production Issue Handling

Triage impact, preserve logs and correlation IDs, decide whether to rollback or hotfix, communicate status clearly, verify recovery with smoke checks, and write a short incident review with follow-up owners.

## Team Breakdown

- Backend developer: domain, application service, API, EF mappings
- Frontend developer: Mantine screens, API client, UI tests
- Database/release owner: SQL review, Azure setup, pipeline, monitoring
- Technical lead: clarify requirements, review design, unblock delivery, own quality gates

## Guiding Junior Developers

Start with one vertical flow, pair on domain invariants, review small pull requests, explain why rules live on the server, and use failing tests to make edge cases concrete.

## Code Review Expectations

Check correctness, readability, business-rule placement, error behavior, concurrency handling, tests, configuration hygiene, and operational impact. Require evidence of local verification.

## Definition Of Done

- Acceptance criteria implemented
- Builds and tests pass
- SQL reviewed and smoke tested
- No real secrets committed
- Docs and API contract updated
- Rollback path understood
- Staging smoke test completed
