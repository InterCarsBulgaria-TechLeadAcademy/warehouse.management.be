using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Common.MessageConstants.Keys;
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
        private readonly IMarkerService markerService;
        private readonly IZoneService zoneService;

        public VendorController(
            IVendorService vendorService,
            IMarkerService markerService,
            IZoneService zoneService)
        {
            this.vendorService = vendorService;
            this.markerService = markerService;
            this.zoneService = zoneService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(VendorDetailsDto))]
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
            if (model.DefaultZoneId != null && !await zoneService.ExistsByIdAsync(model.DefaultZoneId.Value))
            {
                return BadRequest($"{ZoneMessageKeys.ZoneWithIdNotFound} {model.DefaultZoneId.Value}");
            }

            var newVendorId = await vendorService.AddAsync(model, User.Id());

            return Ok(newVendorId);
        }

        [HttpPut("edit/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Edit(int id, [FromBody] VendorFormDto model)
        {
            var nonExistingMarkes = await markerService.GetNonExistingMarkerIdsAsync(
                model.MarkerIds
            );

            if (nonExistingMarkes.Any())
            {
                return BadRequest(
                    $"{MarkerMessageKeys.MarkerWithIdNotFound} {string.Join(",", nonExistingMarkes)}"
                );
            }

            if (model.DefaultZoneId != null && !await zoneService.ExistsByIdAsync(model.DefaultZoneId.Value))
            {
                return BadRequest($"{ZoneMessageKeys.ZoneWithIdNotFound} {model.DefaultZoneId.Value}");
            }

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
