using ShipVoyageManager.Data.Object;

namespace ShipVoyageManager.Service.Contracts;
public interface ITokenService
{
    public Task<string> GetAuthentificationJwtAsync(UserEntity user);
    public Task<string> GenerateRandomTokenAsync();
}
