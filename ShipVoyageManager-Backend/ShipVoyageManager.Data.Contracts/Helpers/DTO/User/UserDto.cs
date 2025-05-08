namespace ShipVoyageManager.Data.Contracts.Helpers.DTO.User;
public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public DateTime RegistrationDate { get; set; }
}
