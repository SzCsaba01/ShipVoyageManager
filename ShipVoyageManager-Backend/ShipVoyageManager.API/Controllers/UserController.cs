using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.User;
using ShipVoyageManager.Service.Contracts;

namespace ShipVoyageManager.API.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("GetFilteredUsersPaginated")]
    public async Task<IActionResult> GetFilteredUsersPaginatedAsync([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string search = "")
    {
        var result = await _userService.GetFilteredUsersPaginatedAsync(page, pageSize, search);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("VerifyIfResetPasswordTokenExists")]
    public async Task<IActionResult> VerifyIfResetPasswordTokenExistsAsync([FromQuery] string resetPasswordToken)
    {
        var isResetPasswordTokenValid = await _userService.VerifyIfResetPasswordTokenExistsAsync(resetPasswordToken);

        return Ok(isResetPasswordTokenValid);
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public async Task<IActionResult> RegisterAsync([FromBody] UserRegistrationDto user)
    {
        await _userService.RegisterUserAsync(user);

        var message = new { message = "You have successfully registered! Verify your email to be able to log in!" };

        return Ok(message);
    }

    [AllowAnonymous]
    [HttpPut("VerifyEmailByRegistrationToken")]
    public async Task<IActionResult> VerifyEmailByRegistrationTokenAsync([FromQuery] string registrationToken)
    {
        await _userService.VerifyEmailByRegistrationTokenAsync(registrationToken);

        var message = new { message = "You have successfully verified your email!" };

        return Ok(message);
    }

    [AllowAnonymous]
    [HttpPut("SendResetPasswordEmail")]
    public async Task<IActionResult> SendResetPasswordEmailAsync([FromQuery] string email)
    {
        await _userService.SendResetPasswordTokenByEmailAsync(email);

        var message = new { message = "We have sent you an email with which you can reset your password!" };

        return Ok(message);
    }

    [AllowAnonymous]
    [HttpPut("ChangePassword")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] UserChangePasswordDto user)
    {
        await _userService.ChangePasswordAsync(user);

        var message = new { message = "You have successfully changed your password!" };

        return Ok(message);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("DeleteUser")]
    public async Task<IActionResult> DeleteUserByUsernameAsync([FromQuery] string username)
    {
        await _userService.DeleteUserByUsernameAsync(username);

        var message = new { message = $"You have successfully deleted {username}!" };

        return Ok(message);
    }
}
