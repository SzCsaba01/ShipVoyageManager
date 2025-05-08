using System.ComponentModel.DataAnnotations;

namespace ShipVoyageManager.Data.Object;
public class UserEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Username is required")]
    [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters long")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public required string Password {get; set; }

    public string? ResetPasswordToken { get; set; }

    public DateTime? ResetPasswordTokenExpirationDate { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [MaxLength(50, ErrorMessage = "Email cannot be longer than 50 characters")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Registration date is required")]
    public DateTime RegistrationDate { get; set; }

    [Required(ErrorMessage = "Registration token is required")]
    public string RegistrationToken { get; set; }

    [Required(ErrorMessage = "Is email confirmed is required")]
    public bool IsEmailConfirmed { get; set; }

    [Required(ErrorMessage = "Role is required")]
    public Guid RoleId { get; set; }

    public RoleEntity Role { get; set; }
}
