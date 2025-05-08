using Quartz;
using ShipVoyageManager.Data.Access;
using ShipVoyageManager.Data.Contracts;
using ShipVoyageManager.Data.Contracts.Helpers;
using ShipVoyageManager.Service.Buisness;
using ShipVoyageManager.Service.Contracts;
using ShipVoyageManager.Service.Quartz;

namespace ShipVoyageManager.API.Infrastructure;

public static class ServiceExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPortRepository, PortRepository>();
        services.AddScoped<IShipRepository, ShipRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVisitedCountryRepository, VisitedCountryRepository>();
        services.AddScoped<IVoyageRepository, VoyageRepository>();

        services.AddScoped<IPortService, PortService>();
        services.AddScoped<IShipService, ShipService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IVisitedCountryService, VisitedCountryService>();
        services.AddScoped<IVoyageService, VoyageService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailService, EmailService>();

        services.AddAutoMapper(typeof(Mapper));

        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("UpdateVisitedCountriesJob");
            q.AddJob<UpdateVisitedCountriesJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("UpdateVisitedCountriesJob-trigger")
                .WithCronSchedule("0 0 0 * * ?")); // Every day at midnight
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        services.AddCors(options => options.AddPolicy(
            name: "Origins",
            policy => {
                policy.WithOrigins("https://localhost:4200").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
            }));

        return services;
    }
}
