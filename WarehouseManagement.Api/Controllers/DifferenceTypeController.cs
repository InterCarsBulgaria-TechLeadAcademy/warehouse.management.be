using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.DifferenceType;
using static WarehouseManagement.Common.MessageConstants.Keys.DifferenceTypeMessageKeys;

namespace WarehouseManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DifferenceTypeController : ControllerBase
{
    private readonly IDifferenceTypeService differenceTypeService;

    public DifferenceTypeController(IDifferenceTypeService differenceTypeService)
    {
        this.differenceTypeService = differenceTypeService;
    }

    [HttpGet("all")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<DifferenceTypeDto>))]
    public async Task<IActionResult> All()
    {
        var models = await differenceTypeService.GetAllAsync();

        return Ok(models);
    }

    [HttpGet("all-with-deleted")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<DifferenceTypeDto>))]
    public async Task<IActionResult> AllWithDeleted()
    {
        var models = await differenceTypeService.GetAllWithDeletedAsync();

        return Ok(models);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(DifferenceTypeDto))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await differenceTypeService.GetByIdAsync(id);

        return Ok(model);
    }

    [HttpPost("add")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Add(DifferenceTypeFormDto model)
    {
        await differenceTypeService.CreateAsync(model);

        return Ok(model);
    }

    [HttpPut("edit/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Edit(int id, [FromBody] DifferenceTypeFormDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(DifferenceTypeInvalidData);
        }

        await differenceTypeService.EditAsync(id, model, User.Id());

        return Ok(DifferenceTypeEditedSuccessfully);
    }

    [HttpDelete("delete/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        await differenceTypeService.DeleteAsync(id);

        return Ok(DifferenceTypeDeletedSuccessfully);
    }

    [HttpPut("restore/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Restore(int id)
    {
        await differenceTypeService.RestoreAsync(id, User.Id());

        return Ok(DifferenceTypeRestored);
    }
}
