# Request Approval Analysis

## Requirements Understanding

ReqFlow supports a single approval workflow: create a request, browse requests, inspect details, and approve or reject a pending request. Rejection requires a reason. Every transition is auditable and terminal outcomes cannot transition again.

## Clarifying Questions

Questions to confirm with a product owner:

1. Which roles may create, approve, and reject requests?
2. May a requester approve their own request?
3. Are edits, cancellation, pagination, search, or attachments needed?
4. How long must audit history be retained?
5. Should notifications be sent after a transition?

## Assumptions

- Authentication uses demo JWTs for seeded users. Production uses an OAuth2/OIDC identity provider.
- A request has exactly one approval decision.
- Stored timestamps are UTC and displayed using the browser locale.
- The API remains the source of truth when multiple users act concurrently.

## Functional Requirements

- Create a pending request with title, description, and requester.
- Show requesters only their own requests while approvers and administrators see the complete queue.
- List requests and show request details with status history.
- Approve or reject only pending requests.
- Require actor on transitions and reason on rejection.
- Allow only active approvers or administrators to review requests.
- Prevent requesters from reviewing their own requests.
- Return clear validation, not-found, and conflict errors.

## Non-Functional Requirements

- Keep the solution readable, testable, and small.
- Preserve auditability with immutable history rows.
- Use optimistic concurrency for competing updates.
- Keep configuration externalized and secrets out of source control.

## Acceptance Criteria

1. A valid create call returns `201` and a pending request with initial history.
2. A pending request can be approved or rejected once.
3. A blank rejection reason returns `400`.
4. A second transition returns `409`.
5. An unknown request returns `404`.
6. List, details, transitions, and history are usable from the React UI.
7. Requester reads are scoped to requests they created.
8. Approvers and administrators can view the complete request queue.
