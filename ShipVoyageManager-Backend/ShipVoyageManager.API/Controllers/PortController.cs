using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Port;
using ShipVoyageManager.Service.Contracts;

namespace ShipVoyageManager.API.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PortController : ControllerBase
{
    private readonly IPortService _portService;

    public PortController(IPortService portService)
    {
        _portService = portService;
    }

    [HttpGet("GetFilteredPortsPaginated")]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> GetFilteredPortsPaginated([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string search = "")
    {
        var result = await _portService.GetFilteredPortsPaginatedAsync(page, pageSize, search);

        return Ok(result);
    }

    [HttpGet("GetAllPorts")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllPorts()
    {
        var ports = await _portService.GetAllPortsAsync();

        return Ok(ports);
    }

    [HttpPost("AddPort")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddPort([FromBody] PortDto portDto)
    {
        await _portService.AddPortAsync(portDto);

        return Ok();
    }

    [HttpPut("UpdatePort")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePort([FromBody] PortDto portDto)
    {
        await _portService.UpdatePortAsync(portDto);

        var message = new { message = "Port updated successfully." };

        return Ok(message);
    }

    [HttpDelete("DeletePortById")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePortById(Guid id)
    {
        await _portService.DeletePortByIdAsync(id);

        var message = new { message = "Port deleted successfully." };

        return Ok(message);
    }
}
