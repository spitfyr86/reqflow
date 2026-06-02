You are helping me implement an interview case-study application named ReqFlow.

Context:
ReqFlow means “Requests + Workflow”. This is for a Technical Lead Fullstack practical assessment. The goal is not to build a perfect production system, but to demonstrate how I analyze, design, build, test, release, communicate, and lead delivery clearly.

The assessment covers:
- .NET API
- SQL Server
- React UI
- Azure deployment approach
- validation, error handling, testing, rollback plan
- technical leadership thinking

Please implement the initial version of the app and documentation based on the requirements below.

Important:
- Keep the app small but clean.
- Prioritize clarity, maintainability, and technical leadership artifacts.
- Do not over-engineer into microservices.
- Use a clean, modular structure.
- Include enough working sample code to demonstrate the design.
- Include docs that explain decisions and trade-offs.
- Use Mantine as the React UI component library.

Application name:
ReqFlow

Feature:
Request Approval

Core behavior:
1. A user can create a request.
2. A user can view a list of requests.
3. A user can view request details.
4. A pending request can be approved.
5. A pending request can be rejected.
6. Rejection requires a reason.
7. Approved or rejected requests cannot be approved/rejected again.
8. Every status change should be recorded in history.
9. The API must enforce the business rules, even if the UI also validates them.

Recommended repository structure:

/docs
  RequestApproval_Analysis.md
  RequestApproval_TechnicalDesign.md
  RequestApproval_APIContract.md
  TestingPlan.md
  ReleasePlan.md
  RiskRollbackTeamPlan.md

/api
  ReqFlow.Api
  ReqFlow.Application
  ReqFlow.Domain
  ReqFlow.Infrastructure
  ReqFlow.Tests

/ui
  reqflow-ui

/sql
  001_create_reqflow_schema.sql
  002_seed_sample_data.sql

README.md

Technology choices:
Backend:
- .NET 8 Web API
- C#
- EF Core
- SQL Server provider
- Swagger/OpenAPI
- xUnit for tests

Frontend:
- React
- TypeScript
- Vite
- Mantine UI
- React Router
- Prefer simple local state/custom hooks for now
- Keep API client code centralized

Database:
- SQL Server
- Use SQL scripts under /sql
- EF Core model should match the SQL schema
- Use SQL Server LocalDB or configurable SQL Server connection string for local development

Backend design:
Use a Clean Architecture-inspired structure, but keep it pragmatic.

ReqFlow.Domain:
- Request entity
- RequestStatus enum
- RequestStatusHistory entity
- Business methods such as Approve and Reject
- Business rules:
  - New request starts as Pending
  - Only Pending requests can be Approved
  - Only Pending requests can be Rejected
  - Reject requires a reason
  - Status transition creates history
  - Approved/Rejected requests cannot transition again

ReqFlow.Application:
- DTOs
- Request service interface and implementation
- CreateRequestDto
- RequestListItemDto
- RequestDetailDto
- ApproveRequestDto
- RejectRequestDto
- Application-level validation
- Mapping between entities and DTOs
- Custom exceptions if useful:
  - NotFoundException
  - ValidationException or BadRequestException
  - ConflictException for invalid status transitions

ReqFlow.Infrastructure:
- AppDbContext
- EF Core configurations
- Repository or direct DbContext usage through application service, whichever is cleaner for this small sample
- SQL Server setup

ReqFlow.Api:
- Controllers
- Global exception handling middleware or ProblemDetails-based error handling
- Swagger
- CORS configured for local React frontend
- Endpoints:
  POST   /api/requests
  GET    /api/requests
  GET    /api/requests/{id}
  POST   /api/requests/{id}/approve
  POST   /api/requests/{id}/reject

Use action endpoints for approve/reject because these are domain workflow actions, not generic CRUD updates.

API response behavior:
- 201 Created for successful request creation
- 200 OK for list, get, approve, reject
- 400 Bad Request for validation errors
- 404 Not Found when request does not exist
- 409 Conflict for invalid status transition or concurrency conflict
- 500 only for unexpected errors
- Use consistent error response bodies, preferably ProblemDetails style

Database schema:
Create SQL scripts with at least these tables:

Requests:
- Id UNIQUEIDENTIFIER PRIMARY KEY
- Title NVARCHAR(150) NOT NULL
- Description NVARCHAR(1000) NOT NULL
- Status NVARCHAR(30) NOT NULL
- RequestedBy NVARCHAR(100) NOT NULL
- CreatedAt DATETIME2 NOT NULL
- UpdatedAt DATETIME2 NULL
- ApprovedRejectedBy NVARCHAR(100) NULL
- ApprovedRejectedAt DATETIME2 NULL
- RejectionReason NVARCHAR(500) NULL
- RowVersion ROWVERSION NOT NULL
- CHECK constraint for Status IN ('Pending', 'Approved', 'Rejected')

RequestStatusHistory:
- Id UNIQUEIDENTIFIER PRIMARY KEY
- RequestId UNIQUEIDENTIFIER NOT NULL
- FromStatus NVARCHAR(30) NULL
- ToStatus NVARCHAR(30) NOT NULL
- ChangedBy NVARCHAR(100) NOT NULL
- ChangedAt DATETIME2 NOT NULL
- Comment NVARCHAR(500) NULL
- Foreign key to Requests(Id)

Indexes:
- Requests(Status)
- Requests(CreatedAt)
- RequestStatusHistory(RequestId)

React UI:
Use React + TypeScript + Vite + Mantine.

Pages:
1. RequestsListPage
   - Shows request list
   - Columns: Title, Requested By, Status, Created At, actions
   - Has button to create request
   - Uses Mantine Table, Badge, Button, Card or Paper

2. CreateRequestPage
   - Form fields:
     - Title
     - Description
     - Requested By
   - Validate required fields
   - Submit to API
   - Navigate to detail page or list page after success
   - Use Mantine TextInput, Textarea, Button, notifications if installed, or Alert

3. RequestDetailsPage
   - Shows full request details
   - Shows status badge
   - Shows approve button if Pending
   - Shows reject button if Pending
   - Reject should collect reason using Mantine Modal or Textarea
   - Shows status history if returned by API
   - Disable/hide approve/reject if request is not Pending
   - Show loading and error states

Frontend structure:
src/
  api/
    requestApprovalApi.ts
  types/
    request.ts
  pages/
    RequestsListPage.tsx
    CreateRequestPage.tsx
    RequestDetailsPage.tsx
  components/
    RequestStatusBadge.tsx
    RequestTable.tsx
    RejectRequestModal.tsx
    LoadingView.tsx
    ErrorAlert.tsx
  App.tsx
  main.tsx

Use MantineProvider in main.tsx.
Use BrowserRouter or createBrowserRouter.
Centralize API base URL using environment variable:
VITE_API_BASE_URL=http://localhost:5000 or similar.

Testing:
Implement initial backend tests using xUnit.

Minimum tests:
- Create request starts as Pending
- Approve pending request succeeds
- Reject pending request with reason succeeds
- Reject pending request without reason fails
- Approve rejected request fails
- Reject approved request fails

Optional but good:
- API endpoint test for create/list/get
- Not found case
- Conflict case

Documentation:
Create the following docs.

README.md:
- Project overview
- Why the app is named ReqFlow
- What the case study demonstrates
- Tech stack
- Folder structure
- How to run locally
- How to run tests
- Local database setup
- Key design decisions
- Known limitations
- How AI/tools were used, if applicable
- What I would improve next

docs/RequestApproval_Analysis.md:
- Requirements understanding
- Clarifying questions
- Assumptions
- Functional requirements
- Non-functional requirements
- Acceptance criteria

docs/RequestApproval_TechnicalDesign.md:
- Architecture diagram in text/markdown
- API/backend design
- Database design
- Frontend design
- Business logic placement
- Validation strategy
- Error handling strategy
- Authentication/authorization note
- Trade-offs

docs/RequestApproval_APIContract.md:
- List all endpoints
- Request/response samples
- Error response samples
- Status code behavior

docs/TestingPlan.md:
- Unit test plan
- API/integration test plan
- Frontend test plan
- Manual smoke test scenarios
- Test data

docs/ReleasePlan.md:
- Azure deployment approach
- Azure services:
  - Azure App Service for API
  - Azure Static Web Apps or App Service for React UI
  - Azure SQL Database
  - App Service Configuration or Key Vault for secrets
  - Application Insights for monitoring
  - Azure DevOps or GitHub Actions for CI/CD
- Deployment steps
- Smoke test steps
- Environment variables/config

docs/RiskRollbackTeamPlan.md:
- Key delivery risks
- Mitigations
- Rollback approach
- Production issue handling
- Team task breakdown
- How I would guide junior developers
- Code review expectations
- Definition of done

Trade-offs to document:
1. SQL Server vs other DB
   - Choose SQL Server because the feature is relational/transactional and the job post specifically expects SQL Server.

2. Clean Architecture-inspired structure vs simple single project
   - Choose modular structure to show maintainability and testability.
   - Mention a single project would be faster but harder to maintain as feature grows.

3. Business action endpoints vs generic PATCH status
   - Choose POST approve/reject because these are domain workflow actions.

4. EF Core vs raw SQL
   - Choose EF Core for productivity and maintainability.
   - Use SQL scripts for assessment review and production-style visibility.

5. Mantine vs custom CSS
   - Choose Mantine to deliver a clean, consistent UI quickly.

6. Local state/custom hooks vs Redux
   - Choose local state/custom API hooks because this is simple server-state-driven workflow.
   - Mention React Query may be considered later for caching/refetching.

7. Azure App Service vs AKS
   - Choose App Service because it is simpler and enough for this app.
   - AKS is overkill unless there are complex container orchestration requirements.

8. Simulated auth vs full OAuth2/JWT
   - Keep auth simulated or documented as future production enhancement.
   - Explain that production should use OAuth2/OIDC with JWT and role-based authorization.

Implementation quality expectations:
- Code should compile.
- Keep code readable.
- Use meaningful names.
- Avoid unnecessary abstractions.
- Do not use CQRS/MediatR unless there is a clear reason; simple services are fine.
- Use async methods in API/data access.
- Add comments only where they clarify decisions.
- Include sample appsettings with placeholder connection string.
- Do not commit real secrets.
- Include .gitignore.
- Include clear run instructions.

Local development:
- API should run on a predictable port.
- UI should point to API through VITE_API_BASE_URL.
- Enable CORS for local UI origin.
- Swagger should be available in development.

Suggested local commands:
Backend:
dotnet restore
dotnet build
dotnet test
dotnet run --project api/ReqFlow.Api

Frontend:
cd ui/reqflow-ui
npm install
npm run dev

Database:
Provide SQL scripts under /sql.
Also configure EF Core so the schema can be created using migrations if desired, but SQL scripts are required for the submission.

Deliverable goal:
At the end, I should have a GitHub repo or ZIP-ready folder that contains:
1. README.md
2. RequestApproval_Analysis.md
3. RequestApproval_TechnicalDesign.md
4. RequestApproval_APIContract.md
5. SQL script/schema
6. .NET API sample code
7. React UI sample code using Mantine
8. TestingPlan.md
9. ReleasePlan.md
10. RiskRollbackTeamPlan.md

After implementing:
- Run dotnet build.
- Run dotnet test.
- Run npm install if needed.
- Run npm build if possible.
- Fix compile/build errors.
- Report what was completed and what was not verified.
- Do not hide failures. If something cannot be run locally, document why and what command should be run manually.

Start by creating the folder structure, then implement the backend, SQL scripts, frontend, tests, and documentation.