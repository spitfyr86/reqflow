# Testing Plan

## Unit Tests

The initial xUnit suite verifies:

- New request starts pending and records history.
- Pending request approval succeeds.
- Rejection with a reason succeeds.
- Rejection without a reason fails.
- Rejected request cannot be approved.
- Approved request cannot be rejected.

## API Integration Tests

Next, add tests against a disposable SQL Server instance for:

- Create, list, and get happy paths
- Validation ProblemDetails contract
- Unknown ID returns `404`
- Second transition returns `409`
- Competing row-version updates return `409`

## Frontend Tests

Add Vitest and React Testing Library coverage for form validation, loading/error states, status badges, and hiding terminal transition buttons.

## Manual Smoke Test

1. Apply both SQL scripts.
2. Start API and UI.
3. Confirm list displays seeded rows.
4. Create a request and confirm pending history.
5. Approve the request and confirm buttons disappear.
6. Reject another pending request and confirm blank reason is blocked.
7. Use Swagger to attempt a second transition and confirm `409`.

## Test Data

The seed script provides one pending request and one approved request. Add per-test data in integration tests to avoid shared-state coupling.
