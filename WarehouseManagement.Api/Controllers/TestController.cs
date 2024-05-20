using Microsoft.AspNetCore.Mvc;

namespace WarehouseManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    [HttpGet("testjson")]
    public IActionResult GetTestJson()
    {
        var testData = new
        {
            Id = 1,
            Name = "Test Item",
            Description = "This is a test item",
            Attributes = new List<string> { "Attribute1", "Attribute2", "Attribute3" },
            CreatedAt = DateTime.UtcNow
        };

        return Ok(testData);
    }
}
