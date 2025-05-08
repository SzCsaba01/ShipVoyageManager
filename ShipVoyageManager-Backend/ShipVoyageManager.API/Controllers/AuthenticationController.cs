using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipVoyageManager.Data.Contracts.Helpers;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Authentication;
using ShipVoyageManager.Service.Contracts;
using System.Security.Claims;

namespace ShipVoyageManager.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpGet("GetCurrentUser")]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(role);
    }

    [HttpPost("Login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] AuthenticationRequestDto request)
    {
        var response = await _authenticationService.LoginAsync(request);

        HttpContext.Response.Cookies.Append("token", response.Token,
            new CookieOptions
            {
                Expires = DateTime.Now.AddHours(AppConstants.JWT_TOKEN_VALIDATION_TIME),
                HttpOnly = true,
                IsEssential = true,
                //Secure = true,
                //SameSite = SameSiteMode.None
            });

        return Ok(response.Role);
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPost("Logout")]
    public Task<IActionResult> Logout()
    {
        HttpContext.Response.Cookies.Delete("token");

        return Task.FromResult<IActionResult>(Ok());
    }
}
