using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Port;
using ShipVoyageManager.Service.Contracts;

namespace ShipVoyageManager.API.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class VoyageController : ControllerBase
{
    private readonly IVoyageService _voyageService;

    public VoyageController(IVoyageService voyageService)
    {
        _voyageService = voyageService;
    }

    [HttpGet("GetFilteredVoyagesPaginated")]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> GetFilteredVoyagesPaginated(int page, int pageSize)
    {
        var voyages = await _voyageService.GetVoyagesPaginatedAsync(page, pageSize);

        return Ok(voyages);
    }

    [HttpPost("AddVoyage")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddVoyage([FromBody] VoyageDto voyageDto)
    {
        await _voyageService.AddVoyageAsync(voyageDto);

        return Ok();
    }

    [HttpPut("UpdateVoyage")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateVoyage([FromBody] VoyageDto voyageDto)
    {
        await _voyageService.UpdateVoyageAsync(voyageDto);

        var message = new { message = "Voyage updated successfully." };

        return Ok(message);
    }

    [HttpDelete("DeleteVoyageById")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteVoyageById(Guid id)
    {
        await _voyageService.DeleteVoyageAsync(id);

        var message = new { message = "Voyage deleted successfully." };

        return Ok(message);
    }
}
