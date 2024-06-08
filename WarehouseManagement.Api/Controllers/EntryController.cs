using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Entry;
using WarehouseManagement.Core.DTOs.Requests;
using static WarehouseManagement.Common.MessageConstants.Keys.EntryMessageKey;

namespace WarehouseManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntryController : ControllerBase
    {
        private readonly IEntryService entryService;

        public EntryController(IEntryService entryService)
        {
            this.entryService = entryService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<EntryDto>))]
        public async Task<IActionResult> All([FromBody] EntryRequest request)
        {
            var entries = await entryService.GetAllAsync(request.ZoneId, request.Statuses);

            return Ok(entries);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(EntryDto))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var entry = await entryService.GetByIdAsync(id);

            return Ok(entry);
        }

        [HttpGet("all-with-deleted")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<EntryDto>))]
        public async Task<IActionResult> AllWithDeleted([FromBody] EntryRequest request)
        {
            var entries = await entryService.GetAllWithDeletedAsync(
                request.ZoneId,
                request.Statuses
            );

            return Ok(entries);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Add([FromBody] EntryFormDto model)
        {
            await entryService.CreateAsync(model, User.Id());

            return Ok(EntryCreatedSuccessfuly);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Edit(int id, [FromBody] EntryFormDto model)
        {
            await entryService.EditAsync(id, model, User.Id());

            return Ok(EntryEditedSuccessfully);
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            await entryService.DeleteAsync(id, User.Id());

            return Ok(EntryDeletedSuccessfully);
        }

        [HttpPut("restore/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Restore(int id)
        {
            await entryService.RestoreAsync(id);

            return Ok(EntryRestored);
        }
    }
}
