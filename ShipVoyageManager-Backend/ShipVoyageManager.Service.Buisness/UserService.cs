using AutoMapper;
using ShipVoyageManager.Data.Contracts;
using ShipVoyageManager.Data.Contracts.Helpers;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.User;
using ShipVoyageManager.Data.Object;
using ShipVoyageManager.Service.Buisness.Exceptions;
using ShipVoyageManager.Service.Contracts;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;

namespace ShipVoyageManager.Service.Buisness;
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly ITokenService _tokenService;
    private readonly IEncryptionService _encryptionService;
    private readonly IMapper _mapper;

    public UserService(
        IUserRepository userRepository,
        IEmailService emailService,
        ITokenService tokenService,
        IEncryptionService encryptionService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _tokenService = tokenService;
        _encryptionService = encryptionService;
        _mapper = mapper;
    }

    public async Task<FilteredPaginatedUsersDto> GetFilteredUsersPaginatedAsync(int page, int pageSize, string search)
    {
        return (await _userRepository.GetFilteredUsersPaginatedAsync(page, pageSize, search));
    }

    public async Task RegisterUserAsync(UserRegistrationDto user)
    {
        if (user.Password != user.RepeatPassword)
        {
            throw new ValidationException("Passwords do not match");
        }

        if (await VerifyExistingUsernameAsync(user.Username))
        {
            throw new ValidationException("Username already exists!");
        }

        if (await VerifyExistingEmailAsync(user.Email))
        {
            throw new ValidationException("Email already exists!");
        }

        var userEntity = _mapper.Map<UserEntity>(user);
        
        userEntity.Password = _encryptionService.GenerateHashedPassword(user.Password);
        userEntity.RegistrationToken = await _tokenService.GenerateRandomTokenAsync() + userEntity.Username;
        userEntity.RegistrationDate = DateTime.Now;

        var uri = $"{AppConstants.FE_APP_VERIFY_EMAIL_URL}/{userEntity.RegistrationToken}";

        await _userRepository.AddUserAsync(userEntity);

        await _emailService.SendEmailVerificationAsync(userEntity.Username, uri, userEntity.Email);
    }

    public async Task VerifyEmailByRegistrationTokenAsync(string registrationToken)
    {
        var user = await _userRepository.GetUserByRegistrationTokenAsync(registrationToken);

        if (user is null)
        {
            throw new ModelNotFoundException("User Not found!");
        }

        user.IsEmailConfirmed = true;

        await _userRepository.UpdateUserAsync(user);
    }

    public async Task<bool> VerifyIfResetPasswordTokenExistsAsync(string resetPasswordToken)
    {
        var user = await _userRepository.GetUserByResetPasswordTokenAsync(resetPasswordToken);

        if (user is null || user.ResetPasswordTokenExpirationDate < DateTime.Now)
        {
            return false;
        }

        return true;
    }

    public async Task SendResetPasswordTokenByEmailAsync(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        if (user is null)
        {
            throw new ModelNotFoundException("Email Not found!");
        }

        if (user.IsEmailConfirmed == false)
        {
            throw new AuthenticationException("Email is not confirmed!");
        }

        user.ResetPasswordToken = await _tokenService.GenerateRandomTokenAsync() + email;
        user.ResetPasswordTokenExpirationDate = DateTime.Now.AddMinutes(AppConstants.RESET_PASSWORD_TOKEN_VALIDATION_TIME);

        await _userRepository.UpdateUserAsync(user);

        var uri = Path.Combine(AppConstants.FE_APP_CHANGE_PASSWORD_URL, user.ResetPasswordToken);

        await _emailService.SendResetPasswordEmailAsync(user.Username, uri, user.Email);
    }

    public async Task ChangePasswordAsync(UserChangePasswordDto changePasswordDto)
    {
        if (changePasswordDto.NewPassword != changePasswordDto.RepeatNewPassword)
        {
            throw new ValidationException("Passwords do not match!");
        }

        var user = await _userRepository.GetUserByResetPasswordTokenAsync(changePasswordDto.ResetPasswordToken);

        if (user is null)
        {
            throw new ModelNotFoundException("User Not found!");
        }

        if (user.ResetPasswordTokenExpirationDate < DateTime.Now)
        {
            throw new ValidationException("Reset Password link has expired!");
        }

        user.Password = _encryptionService.GenerateHashedPassword(changePasswordDto.NewPassword);
        user.ResetPasswordToken = null;
        user.ResetPasswordTokenExpirationDate = null;

        await _userRepository.UpdateUserAsync(user);
    }

    public async Task DeleteUserByUsernameAsync(string username)
    {
        await _userRepository.DeleteUserByUsernameAsync(username);
    }

    private async Task<bool> VerifyExistingUsernameAsync(string username)
    {
        var user = await _userRepository.GetUserByUsernameAsync(username);
        return user is not null;
    }

    private async Task<bool> VerifyExistingEmailAsync(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        return user is not null;
    }
}
