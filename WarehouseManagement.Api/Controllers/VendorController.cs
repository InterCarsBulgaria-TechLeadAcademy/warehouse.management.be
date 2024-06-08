using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Vendor;
using static WarehouseManagement.Common.MessageConstants.Keys.VendorMessageKeys;

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

            return Ok(model);
        }

        [HttpGet("all")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<VendorDto>))]
        public async Task<IActionResult> All([FromQuery] PaginationParameters paginationParams)
        {
            var model = await vendorService.GetAllAsync(paginationParams);

            return Ok(model);
        }

        [HttpPost("add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Add([FromBody] VendorFormDto model)
        {
            var newVendorId = await vendorService.AddAsync(model, User.Id());

            return Ok(newVendorId);
        }

        [HttpPut("edit/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Edit(int id, [FromBody] VendorFormDto model)
        {
            await vendorService.EditAsync(id, model, User.Id());

            return Ok(VendorEditedSuccessfully);
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            await vendorService.DeleteAsync(id);

            return Ok(VendorDeletedSuccessfully);
        }

        [HttpPut("restore/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Restore(int id)
        {
            await vendorService.RestoreAsync(id);

            return Ok(VendorRestored);
        }

        [HttpGet("all-deleted")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<VendorDto>))]
        public async Task<IActionResult> AllDeleted()
        {
            var model = await vendorService.GetAllDeletedAsync();

            return Ok(model);
        }
    }
}
