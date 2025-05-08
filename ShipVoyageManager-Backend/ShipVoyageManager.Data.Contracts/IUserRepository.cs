using ShipVoyageManager.Data.Contracts.Helpers.DTO.User;
using ShipVoyageManager.Data.Object;

namespace ShipVoyageManager.Data.Contracts;
public interface IUserRepository
{
    public Task<UserEntity?> GetUserByUsernameAsync(string username);
    public Task<UserEntity?> GetUserByUsernameOrEmailAndPasswordAsync(string userCredential, string password);
    public Task<UserEntity?> GetUserByEmailAsync(string email);
    public Task<UserEntity?> GetUserByRegistrationTokenAsync(string registrationToken);
    public Task<UserEntity?> GetUserByResetPasswordTokenAsync(string resetPasswordToken);
    public Task<FilteredPaginatedUsersDto> GetFilteredUsersPaginatedAsync(int page, int pageSize, string search);
    public Task AddUserAsync(UserEntity user);
    public Task UpdateUserAsync(UserEntity user);
    public Task DeleteUserByUsernameAsync(string username);
}
