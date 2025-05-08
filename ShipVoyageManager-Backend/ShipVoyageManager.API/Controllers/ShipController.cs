using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Ship;
using ShipVoyageManager.Service.Contracts;

namespace ShipVoyageManager.API.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ShipController : ControllerBase
{
    private readonly IShipService _shipService;

    public ShipController(IShipService shipService)
    {
        _shipService = shipService;
    }

    [HttpGet("GetFilteredShipsPaginated")]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> GetFilteredShipsPaginated([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string search = "")
    {
        var ships = await _shipService.GetFilteredShipsPaginatedAsync(page, pageSize, search);

        return Ok(ships);
    }

    [HttpPut("GetShipsOutOfDateRange")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetShipsOutOfDateRange([FromBody] GetShipsOutOfDateRangeDto request)
    {
        var ships = await _shipService.GetShipsOutOfDateRangeAsync(request.StartDate, request.EndDate);

        return Ok(ships);
    }

    [HttpPost("AddShip")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddShip([FromBody] ShipDto shipDto)
    {
        await _shipService.AddShipAsync(shipDto);

        return Ok();
    }

    [HttpPut("UpdateShip")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateShip([FromBody] ShipDto shipDto)
    {
        await _shipService.UpdateShipAsync(shipDto);

        var message = new {message = "Ship updated successfully." };

        return Ok(message);
    }

    [HttpDelete("DeleteShipById")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteShipById([FromQuery] Guid id)
    {
        await _shipService.DeleteShipByIdAsync(id);

        var message = new { message = "Ship deleted successfully." };

        return Ok(message);
    }
}
