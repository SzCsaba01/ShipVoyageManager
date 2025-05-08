using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipVoyageManager.Service.Contracts;

namespace ShipVoyageManager.API.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class VisitedCountriesController : ControllerBase
{
    private readonly IVisitedCountryService _visitedCountryService;

    public VisitedCountriesController(IVisitedCountryService visitedCountryService)
    {
        _visitedCountryService = visitedCountryService;
    }


    [HttpGet("GetVisitedCountries")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetVisitedCountries()
    {
        var visitedCountries = await _visitedCountryService.GetAllVisitedCountriesAsync();
        return Ok(visitedCountries);
    }

}
