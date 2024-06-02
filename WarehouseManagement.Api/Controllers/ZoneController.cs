using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
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
            try
            {
                var model = await this.zoneService.GetByIdAsync(id);

                return Ok(model);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("all")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ZoneDto>))]
        public async Task<IActionResult> GetAll()
        {
            var zones = await this.zoneService.GetAllAsync();

            return Ok(zones);
        }

        [HttpPost("add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Add([FromBody] ZoneFormDto model)
        {
            try
            {
                await this.zoneService.CreateAsync(model, User.Id());

                return Ok("Zone with name {model.Name} was successfully added");
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }
        }

        [HttpPut("edit/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Edit(int id, [FromBody] ZoneFormDto model)
        {
            try
            {
                await zoneService.EditAsync(id, model, User.Id());

                return Ok("Zone successfully edited");
            }
            catch (KeyNotFoundException ke)
            {
                return NotFound(ke.Message);
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await zoneService.DeleteAsync(id, User.Id());
            }
            catch (KeyNotFoundException ke)
            {
                return NotFound(ke.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Zone was deleted successfully");
        }

        [HttpPut("restore/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var name = await zoneService.RestoreAsync(id);

                return Ok($"Zone {name} was restored");
            }
            catch (KeyNotFoundException ke)
            {
                return NotFound(ke.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("deleted")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ZoneDto>))]
        public async Task<IActionResult> AllDeleted()
        {
            var model = await zoneService.GetAllWithDeletedAsync();
            return Ok(model);
        }
    }
}
