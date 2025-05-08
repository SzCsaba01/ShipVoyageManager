using Microsoft.Extensions.Configuration;
using ShipVoyageManager.Data.Contracts;
using ShipVoyageManager.Data.Contracts.Helpers;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Email;
using ShipVoyageManager.Service.Contracts;
using System.Net;
using System.Net.Mail;

namespace ShipVoyageManager.Service.Buisness;

public class EmailService : IEmailService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;

    public EmailService(IUserRepository userRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _config = config;
    }

    public async Task SendResetPasswordEmailAsync(string username, string uri, string email)
    {
        var emailDto = new EmailDto
        {
            To = email,
            Subject = "Reset Password",
            Body = CreateForgotPasswordMessageBodyByUri(username, uri)
        };

        await SendEmailAsync(emailDto);
    }

    public async Task SendEmailVerificationAsync(string username, string uri, string email)
    {
        var emailDto = new EmailDto
        {
            To = email,
            Subject = "Email Verification",
            Body = CreateEmailVerificationMessageBody(username, uri)
        };

        await SendEmailAsync(emailDto);
    }

    private async Task SendEmailAsync(EmailDto emailDto)
    {
        string fromMail = _config["EmailCredentials:Email"];
        string fromPassword = _config["EmailCredentials:Password"];
        MailMessage message = new MailMessage();

        message.From = new MailAddress(fromMail);
        message.Subject = emailDto.Subject;
        message.To.Add(new MailAddress(emailDto.To));
        message.Body = emailDto.Body;
        message.IsBodyHtml = true;

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(fromMail, fromPassword),
            EnableSsl = true,
        };

        await smtpClient.SendMailAsync(message);
    }

    private string CreateForgotPasswordMessageBodyByUri(string username, string Uri)
    {
        var bodyBuilder =
             $"Dear {username}, <br><br>" +
             $"You have {AppConstants.RESET_PASSWORD_TOKEN_VALIDATION_TIME} minutes to click the link and reset your password " +
             $"<a href={Uri}>link</a>";

        return bodyBuilder;
    }

    private string CreateEmailVerificationMessageBody(string username, string Uri)
    {
        var bodyBuilder =
            $"Dear {username}, <br><br>" +
            $"Verify your email by using the " +
            $"<a href={Uri}>link</a>";

        return bodyBuilder;
    }
}
