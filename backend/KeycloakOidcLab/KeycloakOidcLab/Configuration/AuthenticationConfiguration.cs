using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace KeycloakOidcLab.Configuration;

public static class AuthenticationConfiguration
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "http://localhost:8080/realms/vistra";
                options.RequireHttpsMetadata = false; // TODO pesquisar flag

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "http://localhost:8080/realms/vistra",
                    ValidateAudience = true,
                    ValidAudience = "account",
                    ValidateLifetime = true
                };
            });
        return services;
    }
}