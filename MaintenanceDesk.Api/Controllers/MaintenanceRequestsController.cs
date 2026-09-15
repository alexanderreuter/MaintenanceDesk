using System.Diagnostics;
using MaintenanceDesk.Api.Domain;
using MaintenanceDesk.Api.Dtos;
using MaintenanceDesk.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceDesk.Api.Controllers;

[ApiController]
[Route("api/maintenance-requests")]
[Produces("application/json")]
public class MaintenanceRequestsController(MaintenanceRequestService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<MaintenanceRequestDto>>> List(
        [FromQuery] MaintenanceRequestQuery query,
        CancellationToken cancellationToken)
    {
        var page = await service.ListAsync(query.Status, query.Priority, query.Page, query.PageSize, cancellationToken);

        return new PagedResult<MaintenanceRequestDto>(
            page.Items.Select(MaintenanceRequestDto.From).ToList(),
            page.Page,
            page.PageSize,
            page.TotalCount);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MaintenanceRequestDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var request = await service.GetByIdAsync(id, cancellationToken);

        if (request is null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Maintenance request '{id}' does not exist.");
        }

        return MaintenanceRequestDto.From(request);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MaintenanceRequestDto>> Create(
        CreateMaintenanceRequestDto dto,
        CancellationToken cancellationToken)
    {
        // [ApiController] has already answered 400 if validation failed, so the [Required] values are present.
        var result = await service.CreateAsync(
            dto.ResidentId!.Value,
            dto.Title,
            dto.Description,
            dto.Category!.Value,
            dto.Priority!.Value,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return ToActionResult(result, nameof(dto.ResidentId));
        }

        var created = MaintenanceRequestDto.From(result.Value);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MaintenanceRequestDto>> ChangeStatus(
        Guid id,
        ChangeStatusDto dto,
        CancellationToken cancellationToken)
    {
        var result = await service.ChangeStatusAsync(id, dto.Status!.Value, dto.ResolutionNotes, cancellationToken);
        return ToActionResult(result, nameof(dto.Status));
    }

    [HttpPatch("{id:guid}/assignment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MaintenanceRequestDto>> Assign(
        Guid id,
        AssignTechnicianDto dto,
        CancellationToken cancellationToken)
    {
        var result = await service.AssignAsync(id, dto.TechnicianId!.Value, cancellationToken);
        return ToActionResult(result, nameof(dto.TechnicianId));
    }

    private ActionResult<MaintenanceRequestDto> ToActionResult(ServiceResult<MaintenanceRequest> result, string invalidField)
    {
        if (result.IsSuccess)
        {
            return MaintenanceRequestDto.From(result.Value);
        }

        if (result.Status == ResultStatus.Invalid)
        {
            ModelState.AddModelError(invalidField, result.Error);
            return ValidationProblem(ModelState);
        }

        return result.Status switch
        {
            ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, detail: result.Error),
            ResultStatus.Conflict => Problem(statusCode: StatusCodes.Status409Conflict, detail: result.Error),
            _ => throw new UnreachableException($"Unhandled result status '{result.Status}'."),
        };
    }
}
