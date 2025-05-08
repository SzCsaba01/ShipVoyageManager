namespace ShipVoyageManager.Data.Contracts.Helpers.DTO.User;
public class UserRegistrationDto
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string RepeatPassword { get; set; }
    public string Email { get; set; }
}
