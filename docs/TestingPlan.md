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

## API Integration Tests

The API integration suite verifies the HTTP security and workflow boundary with an in-memory repository:

- Anonymous requests return `401`.
- Inactive users cannot log in.
- Authenticated request creation derives the requester from the token.
- Requesters cannot approve requests.
- Approver actions record the authenticated identity in history.
- Approvers can reject requests with a reason.
- Self-approval returns `403`.
- Blank rejection reason returns `400`.
- An unknown request returns `404`.
- A second transition returns `409`.

Next, add SQL Server-backed integration coverage for EF mappings and competing row-version updates.

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

See [LocalDevelopment.md](LocalDevelopment.md) for a step-by-step UI walkthrough.

## Test Data

The seed script provides one pending request and one approved request. Add per-test data in integration tests to avoid shared-state coupling.
