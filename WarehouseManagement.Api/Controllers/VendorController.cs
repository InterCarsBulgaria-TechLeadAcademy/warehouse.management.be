using System.Security.Claims;
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

        [HttpPut("edit/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Edit(int id, [FromBody] VendorFormDto model)
        {
            if (!await vendorService.ExistByIdAsync(id))
            {
                return NotFound($"Vendor with ID {id} not found.");
            }

            string userId = User.Id();

            if (await vendorService.OtherVendorWithNameExistIdAsync(id, model.Name))
            {
                return BadRequest($"Vendor with name {model.Name} already exist");
            }

            if (await vendorService.OtherVendorWithSystemNumberExistAsync(id, model.SystemNumber))
            {
                return BadRequest($"Vendor with system number {model.SystemNumber} already exist");
            }

            await vendorService.EditAsync(id, model, userId);

            return Ok("Vendor edited successfully");
        }
    }
}
