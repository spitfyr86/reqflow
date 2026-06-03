# Request Approval API Contract

Base URL: `http://localhost:5000`

## Endpoints

| Method | Path | Success | Purpose |
| --- | --- | --- | --- |
| `GET` | `/api/auth/users` | `200` | List active local seeded identities |
| `POST` | `/api/auth/login` | `200` | Issue a local JWT for a selected seeded identity |
| `GET` | `/health` | `200` | Lightweight API health check |
| `POST` | `/api/requests` | `201` | Create a request |
| `GET` | `/api/requests` | `200` | List requests |
| `GET` | `/api/requests/pending-count` | `200` | Count visible pending requests |
| `GET` | `/api/requests/{id}` | `200` | Get request details and history |
| `POST` | `/api/requests/{id}/approve` | `200` | Approve a pending request |
| `POST` | `/api/requests/{id}/reject` | `200` | Reject a pending request |

## Complete API Workflow

Use `POST /api/auth/login` to obtain a bearer token. The requester and reviewer identities are derived from the token, not request bodies.

### 1. Login As A Requester

```json
{
  "userId": "10000000-0000-0000-0000-000000000001"
}
```

The response contains `accessToken`. Use that token to create a request.

### 2. Create A Pending Request

```json
{
  "title": "New laptop",
  "description": "Replacement laptop for a developer."
}
```

The response is `201 Created`. Save the returned request `id`.

### 3. Login As An Approver

```json
{
  "userId": "10000000-0000-0000-0000-000000000003"
}
```

Use the approver token for one of the following terminal actions.

### 4a. Approve

Call:

```text
POST /api/requests/{id}/approve
```

Approval has no request body.

### 4b. Reject

Call:

```text
POST /api/requests/{id}/reject
```

```json
{
  "reason": "Budget is not available."
}
```

### 5. Inspect History

Call:

```text
GET /api/requests/{id}
```

The response includes status, reviewer, rejection reason when applicable, and the ordered `history` collection.

## Visibility Rules

- Requesters only receive requests they created from list and detail endpoints.
- Approvers and administrators can view all requests.
- The pending-count endpoint follows the same visibility rule. The UI uses it as a queue indicator for approvers and administrators.

## Create

Protected endpoints require `Authorization: Bearer {accessToken}`. The API derives requester and reviewer identities from the JWT.

```json
{
  "title": "New laptop",
  "description": "Replacement laptop for a developer."
}
```

## Approve

Approval has no request body.

## Reject

```json
{
  "reason": "Budget is not available."
}
```

## Detail Response

```json
{
  "id": "74392af9-c4bb-4b63-8a81-344252d4816a",
  "title": "New laptop",
  "description": "Replacement laptop for a developer.",
  "requestedBy": "alex@example.com",
  "status": "Pending",
  "createdAt": "2026-06-02T10:00:00Z",
  "updatedAt": null,
  "approvedRejectedBy": null,
  "approvedRejectedAt": null,
  "rejectionReason": null,
  "history": []
}
```

## Errors

Known errors use `application/problem+json`.

```json
{
  "title": "Request conflict",
  "status": 409,
  "detail": "A request with status 'Approved' cannot transition again.",
  "instance": "/api/requests/74392af9-c4bb-4b63-8a81-344252d4816a/reject"
}
```

| Status | When |
| --- | --- |
| `400` | Missing or invalid field |
| `401` | Missing or invalid bearer token |
| `403` | Inactive user, insufficient role, or self-review attempt |
| `404` | Request ID does not exist |
| `409` | Invalid transition or concurrent update |
| `500` | Unexpected server error |
