namespace ShipVoyageManager.Data.Contracts.Helpers;
public static class AppConstants
{
    public const string FE_APP_VERIFY_EMAIL_URL = "https://localhost:4200/verify-email";
    public const string FE_APP_CHANGE_PASSWORD_URL = "https://localhost:4200/change-password";
    public const int RESET_PASSWORD_TOKEN_VALIDATION_TIME = 30;
    public const int JWT_TOKEN_VALIDATION_TIME = 24;
    public const string USER_ROLE = "User";
    public const string ADMIN_ROLE = "Admin";
}
