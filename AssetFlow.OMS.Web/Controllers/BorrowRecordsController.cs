using AssetFlow.OMS.Web.DTOs.BorrowRecords;
using AssetFlow.OMS.Web.Extensions;
using AssetFlow.OMS.Web.Models.Enums;
using AssetFlow.OMS.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetFlow.OMS.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class BorrowRecordsController : ControllerBase
{
    private readonly IBorrowService _borrowService;

    public BorrowRecordsController(IBorrowService borrowService)
    {
        _borrowService = borrowService;
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpGet]
    public async Task<ActionResult<List<BorrowRecordResponseDto>>> GetAll(
        [FromQuery] bool? returnedOnly,
        [FromQuery] bool overdueOnly,
        CancellationToken cancellationToken)
    {
        return Ok(await _borrowService.GetAllAsync(returnedOnly, overdueOnly, null, cancellationToken));
    }

    [HttpGet("mine")]
    public async Task<ActionResult<List<BorrowRecordResponseDto>>> GetMine(
        [FromQuery] bool? returnedOnly,
        [FromQuery] bool overdueOnly,
        CancellationToken cancellationToken)
    {
        return Ok(await _borrowService.GetAllAsync(returnedOnly, overdueOnly, User.GetUserId(), cancellationToken));
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpGet("overdue")]
    public async Task<ActionResult<List<BorrowRecordResponseDto>>> GetOverdue(CancellationToken cancellationToken)
    {
        return Ok(await _borrowService.GetAllAsync(false, true, null, cancellationToken));
    }

    [HttpPost("borrow")]
    public async Task<ActionResult<BorrowRecordResponseDto>> Borrow(BorrowCreateRequestDto request, CancellationToken cancellationToken)
    {
        BorrowRecordResponseDto result = await _borrowService.BorrowAsync(request, User.GetUserId(), User.GetUserName(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:int}/return")]
    public async Task<ActionResult<BorrowRecordResponseDto>> Return(int id, ReturnBorrowRequestDto request, CancellationToken cancellationToken)
    {
        BorrowRecordResponseDto result = await _borrowService.ReturnAsync(id, request, User.GetUserId(), User.GetUserName(), cancellationToken);
        return Ok(result);
    }
}
