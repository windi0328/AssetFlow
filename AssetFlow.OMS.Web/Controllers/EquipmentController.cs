using AssetFlow.OMS.Web.DTOs.Equipment;
using AssetFlow.OMS.Web.Extensions;
using AssetFlow.OMS.Web.Models.Enums;
using AssetFlow.OMS.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetFlow.OMS.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class EquipmentController : ControllerBase
{
    private readonly IEquipmentService _equipmentService;

    public EquipmentController(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EquipmentResponseDto>>> GetAll(
        [FromQuery] string? category,
        [FromQuery] EquipmentStatus? status,
        [FromQuery] string? keyword,
        CancellationToken cancellationToken)
    {
        return Ok(await _equipmentService.GetAllAsync(category, status, keyword, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EquipmentResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        return Ok(await _equipmentService.GetByIdAsync(id, cancellationToken));
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPost]
    public async Task<ActionResult<EquipmentResponseDto>> Create(EquipmentUpsertRequestDto request, CancellationToken cancellationToken)
    {
        EquipmentResponseDto result = await _equipmentService.CreateAsync(request, User.GetUserId(), User.GetUserName(), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<EquipmentResponseDto>> Update(int id, EquipmentUpsertRequestDto request, CancellationToken cancellationToken)
    {
        return Ok(await _equipmentService.UpdateAsync(id, request, User.GetUserId(), User.GetUserName(), cancellationToken));
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _equipmentService.DeleteAsync(id, User.GetUserId(), User.GetUserName(), cancellationToken);
        return NoContent();
    }
}
