using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Common.Statuses;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Zone;

namespace WarehouseManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZoneController : ControllerBase
    {
        private readonly IZoneService zoneService;

        public ZoneController(IZoneService zoneService)
        {
            this.zoneService = zoneService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(ZoneDto))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var model = await zoneService.GetByIdAsync(id);

            return Ok(model);
        }

        [HttpGet("all")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ZoneDto>))]
        public async Task<IActionResult> GetAll()
        {
            var zones = await zoneService.GetAllAsync();

            return Ok(zones);
        }

        [HttpPost("add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Add([FromBody] ZoneFormDto model)
        {
            await zoneService.CreateAsync(model, User.Id());

            return Ok("Zone with name {model.Name} was successfully added");
        }

        [HttpPut("edit/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Edit(int id, [FromBody] ZoneFormDto model)
        {
            await zoneService.EditAsync(id, model, User.Id());

            return Ok("Zone successfully edited");
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Delete(int id)
        {
            await zoneService.DeleteAsync(id, User.Id());

            return Ok("Zone was deleted successfully");
        }

        [HttpPut("restore/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Restore(int id)
        {
            var name = await zoneService.RestoreAsync(id);

            return Ok($"Zone {name} was restored");
        }

        [HttpGet("all-with-deleted")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ZoneDto>))]
        public async Task<IActionResult> AllWithDeleted()
        {
            var model = await zoneService.GetAllWithDeletedAsync();

            return Ok(model);
        }

        [HttpGet("entries")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ZoneEntryDto>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Entries(
            int zoneId,
            [FromQuery] ZoneEntryStatuses[] statuses
        )
        {
            IEnumerable<ZoneEntryDto> entries = new List<ZoneEntryDto>();

            if (statuses.Contains(ZoneEntryStatuses.Waiting))
            {
                entries = await zoneService.GetWaitingEntries(zoneId);
            }

            if (statuses.Contains(ZoneEntryStatuses.Processing))
            {
                entries = await zoneService.GetProccessingEntries(zoneId);
            }

            if (statuses.Contains(ZoneEntryStatuses.Finished))
            {
                entries = await zoneService.GetFinishedEntries(zoneId);
            }

            return Ok(entries);
        }
    }
}
