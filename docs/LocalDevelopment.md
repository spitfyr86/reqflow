# Local Development

This guide starts ReqFlow on Windows with SQL Server LocalDB, the .NET API, and the Vite UI.

## Prerequisites

- .NET 8 SDK or newer
- Node.js and npm
- SQL Server LocalDB
- `sqlcmd`

Confirm the tools are available:

```powershell
dotnet --version
node --version
npm --version
sqlcmd -?
SqlLocalDB info
```

## 1. Create And Seed The Database

Run these commands from the repository root:

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -b -i sql\001_create_reqflow_schema.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -b -i sql\002_seed_sample_data.sql
```

The first script recreates the ReqFlow schema. The second script adds demo users, one pending request, one approved request, and their audit-history rows.

The schema script drops existing ReqFlow tables before recreating them. Use it as a local reset command, not as a production migration.

## 2. Start The API

In the first terminal:

```powershell
dotnet restore
dotnet build ReqFlow.slnx
dotnet run --project api\ReqFlow.Api --launch-profile http
```

The API listens at `http://localhost:5000`.

Swagger is available at:

```text
http://localhost:5000/swagger
```

## 3. Start The UI

In a second terminal:

```powershell
cd ui
npm install
npm run dev
```

Open:

```text
http://localhost:5173
```

The UI uses `http://localhost:5000` as its default API base URL.

## Demo Users

The login screen lists active demo identities.

| User | Email | Role | Use |
| --- | --- | --- | --- |
| Alex Requester | `alex@example.com` | `Requester` | Create requests and view workflow state |
| Sam Requester | `sam@example.com` | `Requester` | Exercise a second requester identity |
| Lee Approver | `lead@example.com` | `Approver` | Approve or reject pending requests |
| Casey Admin | `admin@example.com` | `Admin` | Exercise administrator review access |

An inactive approver is also seeded to verify that inactive accounts cannot log in.

## Complete Approval Workflow In The UI

Use this walkthrough after the API and UI are running.

### Create A Request

1. Open `http://localhost:5173`.
2. Sign in as **Alex Requester**.
3. Select **Create request**.
4. Enter a title and description, then submit the form.
5. Confirm the request detail page shows status **Pending** and an initial history entry created by `alex@example.com`.

### Approve The Request

1. Sign out.
2. Sign in as **Lee Approver**.
3. Open the pending request created by Alex.
4. Select **Approve**.
5. Confirm the status changes to **Approved**, the review actions disappear, and history records the approval by `lead@example.com`.

### Reject A Request

1. Sign out and sign in as **Alex Requester** again.
2. Create another request.
3. Sign out and sign in as **Lee Approver**.
4. Open the new pending request and select **Reject**.
5. Enter a rejection reason and confirm the action.
6. Confirm the status changes to **Rejected**, the reason is displayed, and history records the rejection by `lead@example.com`.

### Verify Guardrails

- Sign in as a requester and confirm pending requests do not show approve or reject actions.
- Sign in as an approver, create a request, and confirm the approver cannot review their own request.
- Try to reject without a reason and confirm the UI blocks submission.
- Try to transition an approved or rejected request through Swagger and confirm the API returns `409 Conflict`.

## Validate The Setup

Run the backend suite from the repository root:

```powershell
dotnet test ReqFlow.slnx
```

Build the frontend:

```powershell
cd ui
npm run build
```

## Reset Local Data

Stop the API, return to the repository root, then rerun:

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -b -i sql\001_create_reqflow_schema.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -b -i sql\002_seed_sample_data.sql
```

## Alternate SQL Server Instance

The default connection string is configured in `api/ReqFlow.Api/appsettings.json`:

```text
Server=(localdb)\MSSQLLocalDB;Database=ReqFlow;Trusted_Connection=True;TrustServerCertificate=True
```

Override it for another SQL Server instance:

```powershell
$env:ConnectionStrings__ReqFlow = "Server=YOUR_SERVER;Database=ReqFlow;Trusted_Connection=True;TrustServerCertificate=True"
dotnet run --project api\ReqFlow.Api --launch-profile http
```

Apply the SQL scripts against the same server by changing the `-S` value passed to `sqlcmd`.

## UI API Override

The UI defaults to `http://localhost:5000`. To point it elsewhere:

```powershell
cd ui
Copy-Item .env.example .env
```

Then update `VITE_API_BASE_URL` in `.env` before running `npm run dev`.

## Troubleshooting

- If the UI cannot load demo users, confirm the API is running at `http://localhost:5000`.
- If LocalDB is unavailable, run `SqlLocalDB info` and verify `MSSQLLocalDB` exists.
- If `sqlcmd` cannot connect, confirm the `-S` value matches the SQL Server instance used by the API connection string.
- If port `5000` or `5173` is already occupied, stop the conflicting process or update the relevant API/UI configuration.
