using ShipVoyageManager.Data.Contracts.Helpers.DTO.Authentication;
using ShipVoyageManager.Data.Contracts;
using ShipVoyageManager.Service.Contracts;
using System.Security.Authentication;

namespace ShipVoyageManager.Service.Buisness;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly ITokenService _tokenService;

    public AuthenticationService
        (
            IUserRepository userRepository,
            IEncryptionService encryptionService,
            ITokenService tokenService
        )
    {
        _userRepository = userRepository;
        _encryptionService = encryptionService;
        _tokenService = tokenService;
    }

    public async Task<AuthenticationResponseDto> LoginAsync(AuthenticationRequestDto request)
    {
        request.Password = _encryptionService.GenerateHashedPassword(request.Password);

        var user = await _userRepository.GetUserByUsernameOrEmailAndPasswordAsync(request.UserCredential, request.Password);

        if (user == null)
        {
            throw new AuthenticationException("Invalid credentials!");
        }

        if (!user.IsEmailConfirmed)
        {
            throw new AuthenticationException("Email is not verified!");
        }

        var token = await _tokenService.GetAuthentificationJwtAsync(user);

        return new AuthenticationResponseDto
        {
            Token = token,
            Role = user.Role.RoleName
        };
    }
}