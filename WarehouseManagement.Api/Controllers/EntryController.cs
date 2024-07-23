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
        private readonly IDeliveryService deliveryService; // TODO: Change delivery status in StartProcessing and FinishProcessing actions if needed when processing entry PR gets approved

        public EntryController(IEntryService entryService, IDeliveryService deliveryService)
        {
            this.entryService = entryService;
            this.deliveryService = deliveryService;
        }

        [HttpGet("all")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<EntryDto>))]
        public async Task<IActionResult> All([FromQuery] EntryRequest request)
        {
            if (request.ZoneId != null)
            {
                return Ok(
                    await entryService.GetAllByZoneAsync((int)request.ZoneId, request.Statuses)
                );
            }

            return Ok(await entryService.GetAllAsync(request.Statuses));
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

        [HttpPost("add")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Add([FromBody] ICollection<EntryFormDto> model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(EntryInvalidData);
            }

            await entryService.CreateAsync(model, User.Id());

            return Ok(EntryCreatedSuccessfuly);
        }

        [HttpPut("edit/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Edit(int id, [FromBody] EntryFormDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(EntryInvalidData);
            }

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

        [HttpGet("start/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> StartProcessing(int id)
        {
            await entryService.StartProcessingAsync(id);

            return Ok(EntryStartedProcessing);
        }

        [HttpGet("finish/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> FinishProcessing(int id)
        {
            await entryService.FinishProcessingAsync(id);

            return Ok(EntryFinishedProcessing);
        }

        [HttpPost("move")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Move([FromQuery] int id, [FromBody] int newZoneId)
        {
            await entryService.MoveAsync(id, newZoneId, User.Id());

            return Ok(EntryMovedSuccessfully);
        }

        [HttpPost("split")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Split([FromQuery] int id, [FromBody] EntrySplitDto splitDto)
        {
            await entryService.SplitAsync(id, splitDto, User.Id());

            return Ok(EntrySplitSuccessfully);
        }
    }
}
