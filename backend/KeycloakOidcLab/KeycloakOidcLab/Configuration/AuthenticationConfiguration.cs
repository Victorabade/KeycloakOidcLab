using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace KeycloakOidcLab.Configuration;

public static class AuthenticationConfiguration
{
    private const string BffCookieScheme = "bff-cookie";
    public const string BffOidcScheme = "bff-oidc";
    public const string BffScheme = BffCookieScheme;

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "http://localhost:8080/realms/oidc-lab";
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "http://localhost:8080/realms/oidc-lab",
                    ValidateAudience = true,
                    ValidAudience = "account",
                    ValidateLifetime = true
                };
            });
        services
            .AddAuthentication()
            .AddCookie(BffCookieScheme, options =>
            {
                options.LoginPath = "/login-bff";
                options.LogoutPath = "/logout-bff";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.None;
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
            })
            .AddOpenIdConnect(BffOidcScheme, options =>
            {
                options.Authority = "http://localhost:8080/realms/oidc-lab";
                options.RequireHttpsMetadata = false;

                options.ClientId = "oidc-lab-bff";
                options.ClientSecret = "CeYEBwHJrHmbqzuiczshqmh6rsEipTHc";

                options.ResponseType = "code";
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "http://localhost:8080/realms/oidc-lab",
                    ValidateAudience = true,
                    ValidAudience = "oidc-lab-bff",
                    ValidateLifetime = true
                };

                options.SignInScheme = BffCookieScheme;
            });
        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme, BffCookieScheme)
                .RequireAuthenticatedUser()
                .Build();
        });


        return services;
    }
}