using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Core.Contracts;

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
}
