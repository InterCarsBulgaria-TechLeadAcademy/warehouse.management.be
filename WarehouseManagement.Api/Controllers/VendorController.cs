using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Vendor;

namespace WarehouseManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly IVendorService vendorService;

        public VendorController(IVendorService vendorService)
        {
            this.vendorService = vendorService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(VendorDto))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetMarker(int id)
        {
            var model = await vendorService.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return Ok(model);
        }

        [HttpGet("all")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<VendorDto>))]
        public async Task<IActionResult> All()
        {
            var model = await vendorService.GetAllAsync();

            return Ok(model);
        }
    }
}
