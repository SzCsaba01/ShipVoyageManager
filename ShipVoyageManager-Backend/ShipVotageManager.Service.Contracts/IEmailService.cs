namespace ShipVoyageManager.Service.Contracts;
public interface IEmailService
{
    public Task SendResetPasswordEmailAsync(string username, string token, string email);
    public Task SendEmailVerificationAsync(string username, string token, string email);
}
