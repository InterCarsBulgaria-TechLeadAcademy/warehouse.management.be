using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

    public DifferenceController(IDifferenceService differenceService)
    {
        this.differenceService = differenceService;
    }

    [HttpGet("all")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<DifferenceDto>))]
    public async Task<IActionResult> All([FromQuery] PaginationParameters paginationParams)
    {
        var models = await differenceService.GetAllAsync(paginationParams);

        return Ok(models);
    }

    [HttpGet("all-with-deleted")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<DifferenceDto>))]
    public async Task<IActionResult> AllWithDeleted([FromQuery] PaginationParameters paginationParams)
    {
        var models = await differenceService.GetAllWithDeletedAsync(paginationParams);

        return Ok(models);
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

        return Ok(DifferenceDeletedSuccessfully);
    }

    [HttpPatch("restore/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Restore(int id)
    {
        await differenceService.RestoreAsync(id);

        return Ok(DifferenceRestored);
    }

    [HttpPost("start")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> StartProcessing([FromBody] int id)
    {
        await differenceService.StartProcessing(id);

        return Ok(DifferenceSuccessfullyStartedProcessing);
    }

    [HttpPost("finish")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> FinishProcessing([FromBody] int id, [FromBody] string adminComment)
    {
        await differenceService.FinishProcessing(id, adminComment);

        return Ok(DifferenceSuccessfullyFinishedProcessing);
    }

    [HttpPost("noDifferences")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> NoDifferences([FromBody] int id, [FromBody] string adminComment)
    {
        await differenceService.NoDifferences(id, adminComment);

        return Ok(DifferenceSuccessfullySetToNoDifferences);
    }
}
