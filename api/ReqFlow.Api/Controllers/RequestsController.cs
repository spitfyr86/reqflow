using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ReqFlow.Application;

namespace ReqFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/requests")]
public sealed class RequestsController(IRequestService requestService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<RequestDetailDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<RequestDetailDto>> Create(CreateRequestDto dto, CancellationToken cancellationToken)
    {
        var created = await requestService.CreateAsync(dto, User.GetAuthenticatedUser(), cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RequestListItemDto>>> List(CancellationToken cancellationToken) =>
        Ok(await requestService.ListAsync(User.GetAuthenticatedUser(), cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RequestDetailDto>> Get(Guid id, CancellationToken cancellationToken) =>
        Ok(await requestService.GetAsync(id, User.GetAuthenticatedUser(), cancellationToken));

    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "Approver,Admin")]
    public async Task<ActionResult<RequestDetailDto>> Approve(Guid id, CancellationToken cancellationToken) =>
        Ok(await requestService.ApproveAsync(id, User.GetAuthenticatedUser(), cancellationToken));

    [HttpPost("{id:guid}/reject")]
    [Authorize(Roles = "Approver,Admin")]
    public async Task<ActionResult<RequestDetailDto>> Reject(Guid id, RejectRequestDto dto, CancellationToken cancellationToken) =>
        Ok(await requestService.RejectAsync(id, dto, User.GetAuthenticatedUser(), cancellationToken));
}
