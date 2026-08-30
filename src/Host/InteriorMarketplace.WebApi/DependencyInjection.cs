using System.Text;
using FluentValidation;
using InteriorMarketplace.BuildingBlocks.Application;
using InteriorMarketplace.Modules.Identity;
using InteriorMarketplace.Modules.Notifications;
using InteriorMarketplace.Modules.Projects.Adapters.Inbound;
using InteriorMarketplace.Modules.Projects.Adapters.Outbound;
using InteriorMarketplace.Modules.Projects.Application;
using InteriorMarketplace.Modules.VendorOffers.Adapters.Inbound;
using InteriorMarketplace.Modules.VendorOffers.Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace InteriorMarketplace.WebApi;

internal static class DependencyInjection
{
    private const string DefaultDevelopmentKey =
        "development-only-secret-key-change-me-123456";

    public static IServiceCollection AddMarketplace(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var jwtKey = configuration["Jwt:Key"] ?? DefaultDevelopmentKey;

        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddHttpContextAccessor();
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddMarketplaceAuthentication(jwtKey);
        services.AddAuthorization();
        services.AddDbContext<MarketplaceDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Marketplace")));
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IOfferRepository, OfferRepository>();
        services.AddScoped<ProjectService>();
        services.AddScoped<OfferService>();
        services.AddScoped<ICurrentUser, HttpCurrentUser>();
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<INotificationPort, LoggingNotificationAdapter>();

        var imageStorageRoot = Path.Combine(
            environment.ContentRootPath,
            "wwwroot",
            "images");
        services.AddSingleton<IImageStorage>(new LocalImageStorage(imageStorageRoot));

        services.AddValidatorsFromAssemblyContaining<ProjectRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<OfferRequestValidator>();
        services.AddSingleton(new JwtDevelopmentTokenService(jwtKey));

        return services;
    }

    private static void AddMarketplaceAuthentication(
        this IServiceCollection services,
        string jwtKey)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = JwtDevelopmentTokenService.Issuer,
                    ValidAudience = JwtDevelopmentTokenService.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey))
                };
            });
    }
}
