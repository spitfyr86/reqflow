# Request Approval API Contract

Base URL: `http://localhost:5000`

## Endpoints

| Method | Path | Success | Purpose |
| --- | --- | --- | --- |
| `GET` | `/api/auth/demo-users` | `200` | List active local demo identities |
| `POST` | `/api/auth/demo-login` | `200` | Issue a local demo JWT |
| `POST` | `/api/requests` | `201` | Create a request |
| `GET` | `/api/requests` | `200` | List requests |
| `GET` | `/api/requests/{id}` | `200` | Get request details and history |
| `POST` | `/api/requests/{id}/approve` | `200` | Approve a pending request |
| `POST` | `/api/requests/{id}/reject` | `200` | Reject a pending request |

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
