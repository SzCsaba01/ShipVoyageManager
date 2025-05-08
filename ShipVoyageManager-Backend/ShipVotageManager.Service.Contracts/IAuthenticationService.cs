using ShipVoyageManager.Data.Contracts.Helpers.DTO.Authentication;

namespace ShipVoyageManager.Service.Contracts;
public interface IAuthenticationService
{
    public Task<AuthenticationResponseDto> LoginAsync(AuthenticationRequestDto request);
}
