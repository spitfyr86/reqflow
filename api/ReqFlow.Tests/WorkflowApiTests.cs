using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ReqFlow.Application;
using ReqFlow.Domain;

namespace ReqFlow.Tests;

public sealed class WorkflowApiTests
{
    private static readonly Guid RequesterId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    private static readonly Guid OtherRequesterId = Guid.Parse("10000000-0000-0000-0000-000000000002");
    private static readonly Guid ApproverId = Guid.Parse("10000000-0000-0000-0000-000000000003");
    private static readonly Guid InactiveUserId = Guid.Parse("10000000-0000-0000-0000-000000000005");

    [Fact]
    public async Task RequestsRequireAuthentication()
    {
        using var factory = new ReqFlowApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/requests");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task HealthEndpointIsAvailableWithoutAuthentication()
    {
        using var factory = new ReqFlowApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task InactiveUserCannotLogin()
    {
        using var factory = new ReqFlowApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginDto(InactiveUserId));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AuthenticatedRequesterCanCreateRequest()
    {
        using var factory = new ReqFlowApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, RequesterId);

        var response = await client.PostAsJsonAsync("/api/requests", new CreateRequestDto("Monitor", "External monitor"));
        var detail = await response.Content.ReadFromJsonAsync<RequestDetailDto>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(detail);
        Assert.Equal("alex@example.com", detail.RequestedBy);
        Assert.Equal(RequesterId, detail.RequestedByUserId);
        Assert.Equal("Pending", detail.Status);
    }

    [Fact]
    public async Task RequesterOnlySeesOwnRequests()
    {
        using var factory = new ReqFlowApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, RequesterId);

        var requests = await client.GetFromJsonAsync<List<RequestListItemDto>>("/api/requests");
        var hiddenResponse = await client.GetAsync($"/api/requests/{factory.OtherRequesterRequestId}");

        Assert.NotNull(requests);
        Assert.All(requests, request => Assert.Equal("alex@example.com", request.RequestedBy));
        Assert.Equal(HttpStatusCode.Forbidden, hiddenResponse.StatusCode);
    }

    [Fact]
    public async Task ApproverSeesAllRequestsAndPendingCount()
    {
        using var factory = new ReqFlowApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, ApproverId);

        var requests = await client.GetFromJsonAsync<List<RequestListItemDto>>("/api/requests");
        var pending = await client.GetFromJsonAsync<PendingRequestCountDto>("/api/requests/pending-count");

        Assert.NotNull(requests);
        Assert.Equal(2, requests.Count);
        Assert.Equal(2, pending!.Count);
    }

    [Fact]
    public async Task RequesterCannotApproveRequest()
    {
        using var factory = new ReqFlowApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, RequesterId);

        var response = await client.PostAsync($"/api/requests/{factory.PendingRequestId}/approve", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ApproverCanApproveAndHistoryUsesAuthenticatedActor()
    {
        using var factory = new ReqFlowApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, ApproverId);

        var response = await client.PostAsync($"/api/requests/{factory.PendingRequestId}/approve", null);
        var detail = await response.Content.ReadFromJsonAsync<RequestDetailDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(detail);
        Assert.Equal("Approved", detail.Status);
        Assert.Equal("lead@example.com", detail.ApprovedRejectedBy);
        Assert.Equal("lead@example.com", detail.History.Last().ChangedBy);
    }

    [Fact]
    public async Task ApproverCannotApproveOwnRequest()
    {
        using var factory = new ReqFlowApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, ApproverId);
        var created = await client.PostAsJsonAsync("/api/requests", new CreateRequestDto("Team event", "Quarterly planning"));
        var detail = await created.Content.ReadFromJsonAsync<RequestDetailDto>();

        var response = await client.PostAsync($"/api/requests/{detail!.Id}/approve", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ApproverCanRejectRequest()
    {
        using var factory = new ReqFlowApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, ApproverId);

        var response = await client.PostAsJsonAsync($"/api/requests/{factory.PendingRequestId}/reject", new RejectRequestDto("Budget unavailable"));
        var detail = await response.Content.ReadFromJsonAsync<RequestDetailDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(detail);
        Assert.Equal("Rejected", detail.Status);
        Assert.Equal("Budget unavailable", detail.RejectionReason);
    }

    [Fact]
    public async Task UnknownRequestReturnsNotFound()
    {
        using var factory = new ReqFlowApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, ApproverId);

        var response = await client.GetAsync($"/api/requests/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RejectionRequiresReasonAndTerminalRequestReturnsConflict()
    {
        using var factory = new ReqFlowApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, ApproverId);

        var invalid = await client.PostAsJsonAsync($"/api/requests/{factory.PendingRequestId}/reject", new RejectRequestDto(" "));
        var approved = await client.PostAsync($"/api/requests/{factory.PendingRequestId}/approve", null);
        var repeated = await client.PostAsJsonAsync($"/api/requests/{factory.PendingRequestId}/reject", new RejectRequestDto("Too late"));

        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
        Assert.Equal(HttpStatusCode.OK, approved.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, repeated.StatusCode);
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(ReqFlowApiFactory factory, Guid userId)
    {
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginDto(userId));
        var login = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.AccessToken);
        return client;
    }
}

public sealed class ReqFlowApiFactory : WebApplicationFactory<Program>
{
    public Guid PendingRequestId => Services.GetRequiredService<TestStore>().PendingRequestId;
    public Guid OtherRequesterRequestId => Services.GetRequiredService<TestStore>().OtherRequesterRequestId;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IRequestRepository>();
            services.RemoveAll<IUserRepository>();
            services.AddSingleton<TestStore>();
            services.AddSingleton<IRequestRepository>(provider => provider.GetRequiredService<TestStore>());
            services.AddSingleton<IUserRepository>(provider => provider.GetRequiredService<TestStore>());
        });
    }
}

public sealed class TestStore : IRequestRepository, IUserRepository
{
    private static readonly Guid RequesterId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    private static readonly Guid OtherRequesterId = Guid.Parse("10000000-0000-0000-0000-000000000002");
    private static readonly Guid ApproverId = Guid.Parse("10000000-0000-0000-0000-000000000003");
    private static readonly Guid InactiveUserId = Guid.Parse("10000000-0000-0000-0000-000000000005");
    private readonly List<User> _users =
    [
        new(RequesterId, "alex@example.com", "Alex Requester", UserRole.Requester),
        new(OtherRequesterId, "sam@example.com", "Sam Requester", UserRole.Requester),
        new(ApproverId, "lead@example.com", "Lee Approver", UserRole.Approver),
        new(InactiveUserId, "inactive@example.com", "Inactive User", UserRole.Approver, false)
    ];
    private readonly List<Request> _requests;

    public TestStore()
    {
        var request = new Request("Laptop", "Replacement laptop", RequesterId, "alex@example.com");
        var otherRequesterRequest = new Request("Training", "Course registration", OtherRequesterId, "sam@example.com");
        PendingRequestId = request.Id;
        OtherRequesterRequestId = otherRequesterRequest.Id;
        _requests = [request, otherRequesterRequest];
    }

    public Guid PendingRequestId { get; }
    public Guid OtherRequesterRequestId { get; }

    public Task AddAsync(Request request, CancellationToken cancellationToken)
    {
        _requests.Add(request);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Request>> ListAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Request>>(_requests);

    public Task<Request?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(_requests.SingleOrDefault(request => request.Id == id));

    public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task<IReadOnlyList<User>> ListActiveAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<User>>(_users.Where(user => user.IsActive).ToList());

    async Task<User?> IUserRepository.GetAsync(Guid id, CancellationToken cancellationToken) =>
        await Task.FromResult(_users.SingleOrDefault(user => user.Id == id));
}
