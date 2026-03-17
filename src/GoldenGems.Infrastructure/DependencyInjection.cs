using System.Text;
using GoldenGems.Application.Interfaces.Admin;
using GoldenGems.Application.Interfaces.Auth;
using GoldenGems.Application.Interfaces.People;
using GoldenGems.Application.Services.Admin;
using GoldenGems.Application.Services.Auth;
using GoldenGems.Application.Services.People;
using GoldenGems.Domain.Entities.Security;
using GoldenGems.Domain.Interfaces;
using GoldenGems.Infrastructure.Authentication;
using GoldenGems.Infrastructure.Data;
using GoldenGems.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace GoldenGems.Infrastructure;

/// <summary>
/// Extensión para registrar todos los servicios de Infrastructure y Application en el contenedor DI.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<GoldenGemsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IActionRepository, ActionRepository>();
        services.AddScoped<IActionTypeRepository, ActionTypeRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();
        services.AddScoped<IRegionRepository, RegionRepository>();

        // Password Hasher
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();

        // Application Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IActionService, ActionService>();
        services.AddScoped<IUserValidationService, UserValidationService>();
        services.AddScoped<IDocumentTypeService, DocumentTypeService>();
        services.AddScoped<IRegionService, RegionService>();

        // JWT
        var jwtSection = configuration.GetSection("Jwt");
        var jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings();

        jwtSettings.SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? jwtSettings.SecretKey;
        jwtSettings.Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? jwtSettings.Issuer;
        jwtSettings.Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? jwtSettings.Audience;

        var expirationFromEnv = Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_MINUTES");
        if (int.TryParse(expirationFromEnv, out var expirationMinutes))
        {
            jwtSettings.AccessTokenExpirationMinutes = expirationMinutes;
        }

        if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
        {
            throw new InvalidOperationException("Jwt:SecretKey o JWT_SECRET no están configurados.");
        }

        services.Configure<JwtSettings>(options =>
        {
            options.Issuer = jwtSettings.Issuer;
            options.Audience = jwtSettings.Audience;
            options.SecretKey = jwtSettings.SecretKey;
            options.AccessTokenExpirationMinutes = jwtSettings.AccessTokenExpirationMinutes;
        });

        services.AddScoped<ITokenService, JwtTokenService>();

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = signingKey,
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }
}
