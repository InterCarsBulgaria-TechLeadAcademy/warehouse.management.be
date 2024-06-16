using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Delivery;

namespace WarehouseManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DeliveryController : ControllerBase
{
    private readonly IDeliveryService deliveryService;

    public DeliveryController(IDeliveryService deliveryService)
    {
        this.deliveryService = deliveryService;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(DeliveryDto))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetDelivery(int id)
    {
        var model = await deliveryService.GetByIdAsync(id);

        return Ok(model);
    }

    [HttpGet("all")]
    [ProducesResponseType(200, Type = typeof(ICollection<DeliveryDto>))]
    public async Task<IActionResult> GetDeliveries()
    {
        var model = await deliveryService.GetAllAsync();

        return Ok(model);
    }

    [HttpGet("add")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Add()
    {
        return Ok();
    }

    [HttpGet("edit /{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Edit(int id, [FromBody] DeliveryFormDto model)
    {
        return Ok();
    }
}
