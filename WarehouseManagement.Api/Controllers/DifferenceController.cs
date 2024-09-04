using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Difference;
using static WarehouseManagement.Common.MessageConstants.Keys.DifferenceMessageKeys;

namespace WarehouseManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DifferenceController : ControllerBase
{
    private readonly IDifferenceService differenceService;
    private readonly IDeliveryService deliveryService;

    public DifferenceController(
        IDifferenceService differenceService,
        IDeliveryService deliveryService
    )
    {
        this.differenceService = differenceService;
        this.deliveryService = deliveryService;
    }

    [HttpGet("all")]
    [ProducesResponseType(200, Type = typeof(PageDto<DifferenceDto>))]
    public async Task<IActionResult> All([FromQuery] PaginationParameters paginationParams)
    {
        var pageDto = await differenceService.GetAllAsync(paginationParams);

        return Ok(pageDto);
    }

    [HttpGet("all-with-deleted")]
    [ProducesResponseType(200, Type = typeof(PageDto<DifferenceDto>))]
    public async Task<IActionResult> AllWithDeleted(
        [FromQuery] PaginationParameters paginationParams
    )
    {
        var pageDto = await differenceService.GetAllWithDeletedAsync(paginationParams);

        return Ok(pageDto);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(DifferenceDto))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await differenceService.GetByIdAsync(id);

        return Ok(model);
    }

    [HttpPost("add")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Add([FromBody] DifferenceFormDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await differenceService.CreateAsync(model, User.Id());

        var deliveryId = await differenceService.GetDeliveryIdAsync(id);
        await deliveryService.ChangeDeliveryStatusIfNeeded(deliveryId);

        return Ok(DifferenceAddedSuccessfully);
    }

    [HttpPut("edit/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Edit(int id, [FromBody] DifferenceFormDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await differenceService.EditAsync(id, model, User.Id());

        return Ok(DifferenceEditedSuccessfully);
    }

    [HttpDelete("delete/{id}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Delete(int id)
    {
        await differenceService.DeleteAsync(id);

        var deliveryId = await differenceService.GetDeliveryIdAsync(id);
        await deliveryService.ChangeDeliveryStatusIfNeeded(deliveryId);

        return Ok(DifferenceDeletedSuccessfully);
    }

    [HttpPatch("restore/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Restore(int id)
    {
        await differenceService.RestoreAsync(id);

        var deliveryId = await differenceService.GetDeliveryIdAsync(id);
        await deliveryService.ChangeDeliveryStatusIfNeeded(deliveryId);

        return Ok(DifferenceRestored);
    }

    [HttpPost("start/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> StartProcessing(int id)
    {
        await differenceService.StartProcessing(id);

        var deliveryId = await differenceService.GetDeliveryIdAsync(id);
        await deliveryService.ChangeDeliveryStatusIfNeeded(deliveryId);

        return Ok(DifferenceSuccessfullyStartedProcessing);
    }

    [HttpPost("finish/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> FinishProcessing(
        int id,
        [FromBody] DifferenceAdminCommentDto adminCommentDto
    )
    {
        await differenceService.FinishProcessing(id, adminCommentDto);

        var deliveryId = await differenceService.GetDeliveryIdAsync(id);
        await deliveryService.ChangeDeliveryStatusIfNeeded(deliveryId);

        return Ok(DifferenceSuccessfullyFinishedProcessing);
    }

    [HttpPost("noDifferences/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> NoDifferences(
        int id,
        [FromBody] DifferenceAdminCommentDto adminCommentDto
    )
    {
        await differenceService.NoDifferences(id, adminCommentDto);

        var deliveryId = await differenceService.GetDeliveryIdAsync(id);
        await deliveryService.ChangeDeliveryStatusIfNeeded(deliveryId);

        return Ok(DifferenceSuccessfullySetToNoDifferences);
    }
}
