using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs;
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
        private readonly IDeliveryService deliveryService;

        public EntryController(IEntryService entryService, IDeliveryService deliveryService)
        {
            this.entryService = entryService;
            this.deliveryService = deliveryService;
        }

        [HttpGet("all")]
        [ProducesResponseType(200, Type = typeof(PageDto<EntryDto>))]
        public async Task<IActionResult> All([FromQuery] PaginationParameters paginationParams, [FromQuery] EntryRequest request)
        {
            if (request.ZoneId != null)
            {
                return Ok(
                    await entryService.GetAllByZoneAsync(paginationParams, (int)request.ZoneId, request.Statuses)
                );
            }

            return Ok(await entryService.GetAllAsync(paginationParams, request.Statuses));
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
        public async Task<IActionResult> AllWithDeleted([FromQuery] PaginationParameters paginationParams, [FromQuery] EntryRequest request)
        {
            var pageDto = await entryService.GetAllWithDeletedAsync(
                paginationParams,
                request.ZoneId,
                request.Statuses
            );

            return Ok(pageDto);
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

        [HttpPut("start/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> StartProcessing(int id)
        {
            await entryService.StartProcessingAsync(id);

            var entry = await entryService.GetByIdAsync(id);
            await deliveryService.ChangeDeliveryStatusIfNeeded(entry.DeliveryDetails.Id);

            return Ok(EntryStartedProcessing);
        }

        [HttpPut("finish/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> FinishProcessing(int id)
        {
            await entryService.FinishProcessingAsync(id);

            var entry = await entryService.GetByIdAsync(id);
            await deliveryService.ChangeDeliveryStatusIfNeeded(entry.DeliveryDetails.Id);

            return Ok(EntryFinishedProcessing);
        }

        [HttpPost("move/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Move(int id, [FromBody] int newZoneId)
        {
            await entryService.MoveAsync(id, newZoneId, User.Id());

            return Ok(EntryMovedSuccessfully);
        }

        [HttpPost("split/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Split(int id, [FromBody] EntrySplitDto splitDto)
        {
            await entryService.SplitAsync(id, splitDto, User.Id());

            return Ok(EntrySplitSuccessfully);
        }
    }
}
