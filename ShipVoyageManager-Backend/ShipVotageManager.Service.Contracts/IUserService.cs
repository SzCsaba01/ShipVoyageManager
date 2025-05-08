using ShipVoyageManager.Data.Contracts.Helpers.DTO.User;
using ShipVoyageManager.Data.Object;

namespace ShipVoyageManager.Service.Contracts;
public interface IUserService
{
    public Task<FilteredPaginatedUsersDto> GetFilteredUsersPaginatedAsync(int page, int pageSize, string search);
    public Task VerifyEmailByRegistrationTokenAsync(string registrationToken);
    public Task<bool> VerifyIfResetPasswordTokenExistsAsync(string resetPasswordToken);
    public Task RegisterUserAsync(UserRegistrationDto user);
    public Task SendResetPasswordTokenByEmailAsync(string email);
    public Task ChangePasswordAsync(UserChangePasswordDto changePasswordDto);
    public Task DeleteUserByUsernameAsync(string username);
}
