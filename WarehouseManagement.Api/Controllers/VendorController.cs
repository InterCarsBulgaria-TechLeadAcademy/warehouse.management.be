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
        public async Task<IActionResult> GetVendor(int id)
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

        [HttpPut("add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Add([FromBody] VendorFormDto model)
        {
            if (await vendorService.ExistByNameAsync(model.Name))
            {
                return BadRequest($"Vendor with name {model.Name} already exist");
            }

            if (await vendorService.ExistBySystemNumberAsync(model.SystemNumber))
            {
                return BadRequest($"Vendor with system number {model.SystemNumber} already exist");
            }

            await vendorService.AddAsync(model, User.Id());

            return Ok(
                $"Vendor with name {model.Name} and system number {model.SystemNumber} was added suscesfully"
            );
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

            if (await vendorService.AnotherVendorWithNameExistAsync(id, model.Name))
            {
                return BadRequest($"Another vendor with name {model.Name} already exist");
            }

            if (await vendorService.AnotherVendorWithSystemNumberExistAsync(id, model.SystemNumber))
            {
                return BadRequest($"Another with system number {model.SystemNumber} already exist");
            }

            await vendorService.EditAsync(id, model, User.Id());

            return Ok("Vendor edited successfully");
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await vendorService.DeleteAsync(id, User.Id());
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            return Ok("Vendor was deleted successfully");
        }
    }
}
