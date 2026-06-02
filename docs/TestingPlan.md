# Testing Plan

## Unit Tests

The initial xUnit suite verifies:

- New request starts pending and records history.
- Pending request approval succeeds.
- Rejection with a reason succeeds.
- Rejection without a reason fails.
- Rejected request cannot be approved.
- Approved request cannot be rejected.
- A requester cannot approve their own request.
- EF treats domain-generated status-history IDs as client-assigned values so new audit rows are inserted during transitions.

## API Integration Tests

The API integration suite verifies the HTTP security and workflow boundary with an in-memory repository:

- Anonymous requests return `401`.
- Inactive users cannot log in.
- Authenticated request creation derives the requester from the token.
- Requesters only see their own requests and receive `403` for another requester's detail.
- Approvers see the complete queue and its pending count.
- Requesters cannot approve requests.
- Approver actions record the authenticated identity in history.
- Approvers can reject requests with a reason.
- Self-approval returns `403`.
- Blank rejection reason returns `400`.
- An unknown request returns `404`.
- A second transition returns `409`.

## SQL Server-Backed Smoke Coverage

The LocalDB bootstrap and live smoke commands validate the real SQL scripts and EF-backed API path during submission verification. A permanent SQL Server-backed test fixture is intentionally deferred: reliable per-run database isolation and cleanup would add more setup than this interview exercise warrants. In a production repository, add an isolated ephemeral SQL Server database in CI to cover EF mappings and competing row-version updates.

The verified LocalDB approval smoke check confirms a pending request transitions to approved and its persisted history count increases from one row to two.

## Frontend Tests

Add Vitest and React Testing Library coverage for form validation, loading/error states, status badges, and hiding terminal transition buttons.

## Manual Smoke Test

1. Apply both SQL scripts.
2. Start API and UI.
3. Confirm the login page lists active demo users.
4. Sign in as Alex Requester and confirm the list displays seeded rows.
5. Create a request and confirm pending history records `alex@example.com`.
6. Sign out, sign in as Lee Approver, approve the request, and confirm buttons disappear.
7. Create a second request as Alex, then reject it as Lee with a reason.
8. Confirm requester accounts cannot see review actions.
9. Confirm an approver cannot review a request they created.
10. Confirm blank rejection reason is blocked.
11. Use Swagger to attempt a second transition and confirm `409`.
12. Confirm `GET /health` returns `200`.

See [LocalDevelopment.md](LocalDevelopment.md) for a step-by-step UI walkthrough.

## Test Data

The seed script provides one pending request and one approved request. Add per-test data in integration tests to avoid shared-state coupling.
