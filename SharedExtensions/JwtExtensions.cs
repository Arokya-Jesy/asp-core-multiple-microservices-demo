using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace SharedExtensions
{
    public static class JwtExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind JWT settings from configuration (supports appsettings.json, environment variables, etc.)
            var jwtSettings = new JwtSettings();
            configuration.GetSection("JwtSettings").Bind(jwtSettings);

            // Validate configuration
            ValidateJwtSettings(jwtSettings);

            // Register JwtSettings as options for dependency injection
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = key,
                        ClockSkew = TimeSpan.Zero // No tolerance for expired tokens
                    };
                });

            return services;
        }

        private static void ValidateJwtSettings(JwtSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.SecretKey))
                throw new InvalidOperationException("JWT SecretKey must be configured via appsettings.json or JWT_SECRETKEY environment variable");
            
            if (string.IsNullOrWhiteSpace(settings.Issuer))
                throw new InvalidOperationException("JWT Issuer must be configured via appsettings.json or JWT_ISSUER environment variable");
            
            if (string.IsNullOrWhiteSpace(settings.Audience))
                throw new InvalidOperationException("JWT Audience must be configured via appsettings.json or JWT_AUDIENCE environment variable");

            if (settings.SecretKey.Length < 32)
                throw new InvalidOperationException("JWT SecretKey must be at least 32 characters long");
        }
    }
}
