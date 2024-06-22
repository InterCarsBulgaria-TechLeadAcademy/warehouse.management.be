using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Common.MessageConstants.Keys;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Delivery;
using static WarehouseManagement.Common.MessageConstants.Keys.DeliveryMessageKeys;

namespace WarehouseManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DeliveryController : ControllerBase
{
    private readonly IDeliveryService deliveryService;
    private readonly IVendorService vendorService;

    public DeliveryController(IDeliveryService deliveryService, IVendorService vendorService)
    {
        this.deliveryService = deliveryService;
        this.vendorService = vendorService;
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

    [HttpPost("add")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Add()
    {
        return Ok();
    }

    [HttpPut("edit/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Edit(int id, [FromBody] DeliveryFormDto model)
    {
        if (!await vendorService.ExistByIdAsync(model.VendorId))
        {
            return BadRequest($"{VendorMessageKeys.VendorWithIdNotFound} {model.VendorId}");
        }

        await deliveryService.EditAsync(id, model, User.Id());

        return Ok(DeliveryEditedSuccessfully);
    }
}
