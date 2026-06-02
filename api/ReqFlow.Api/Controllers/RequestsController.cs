using Microsoft.AspNetCore.Mvc;
using ReqFlow.Application;

namespace ReqFlow.Api.Controllers;

[ApiController]
[Route("api/requests")]
public sealed class RequestsController(IRequestService requestService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<RequestDetailDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<RequestDetailDto>> Create(CreateRequestDto dto, CancellationToken cancellationToken)
    {
        var created = await requestService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RequestListItemDto>>> List(CancellationToken cancellationToken) =>
        Ok(await requestService.ListAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RequestDetailDto>> Get(Guid id, CancellationToken cancellationToken) =>
        Ok(await requestService.GetAsync(id, cancellationToken));

    [HttpPost("{id:guid}/approve")]
    public async Task<ActionResult<RequestDetailDto>> Approve(Guid id, ApproveRequestDto dto, CancellationToken cancellationToken) =>
        Ok(await requestService.ApproveAsync(id, dto, cancellationToken));

    [HttpPost("{id:guid}/reject")]
    public async Task<ActionResult<RequestDetailDto>> Reject(Guid id, RejectRequestDto dto, CancellationToken cancellationToken) =>
        Ok(await requestService.RejectAsync(id, dto, cancellationToken));
}
