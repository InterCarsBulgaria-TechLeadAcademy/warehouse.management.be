using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Api.Middlewares;
using WarehouseManagement.Common.Statuses;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Entry;
using WarehouseManagement.Core.DTOs.Zone;
using static WarehouseManagement.Common.MessageConstants.Keys.ZoneMessageKeys;

namespace WarehouseManagement.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ZoneController : ControllerBase
    {
        private readonly IZoneService zoneService;
        private readonly IEntryService entryService;

        public ZoneController(IZoneService zoneService, IEntryService entryService)
        {
            this.zoneService = zoneService;
            this.entryService = entryService;
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

        [HttpGet("all-with-params")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ZoneDto>))]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParameters paginationParams)
        {
            var zones = await zoneService.GetAllAsync(paginationParams);

            return Ok(zones);
        }

        [HttpPost("add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Add([FromBody] ZoneFormDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ZoneInvalidData);
            }

            await zoneService.CreateAsync(model, User.Id());

            return Ok(ZoneCreatedSuccessfully);
        }

        [HttpPut("edit/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Edit(int id, [FromBody] ZoneFormDto model)
        {
            await zoneService.EditAsync(id, model, User.Id());

            return Ok(ZoneEditedSuccessfully);
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Delete(int id)
        {
            await zoneService.DeleteAsync(id, User.Id());

            return Ok(ZoneDeletedSuccessfully);
        }

        [HttpPut("restore/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Restore(int id)
        {
            await zoneService.RestoreAsync(id);

            return Ok(ZoneRestored);
        }

        [HttpGet("all-with-deleted")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ZoneDto>))]
        public async Task<IActionResult> AllWithDeleted()
        {
            var model = await zoneService.GetAllWithDeletedAsync();

            return Ok(model);
        }

        [HttpGet("all-with-deleted-with-params")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ZoneDto>))]
        public async Task<IActionResult> AllWithDeleted([FromBody] PaginationParameters paginationParams)
        {
            var model = await zoneService.GetAllWithDeletedAsync(paginationParams);

            return Ok(model);
        }

        [HttpGet("entries")]
        [ProducesResponseType(200, Type = typeof(PageDto<EntryDto>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Entries(int zoneId, [FromQuery] EntryStatuses[]? statuses, [FromQuery] PaginationParameters paginationParams)
        {
            var pageDto = await entryService.GetAllByZoneAsync(paginationParams, zoneId, statuses);

            return Ok(pageDto);
        }
    }
}
