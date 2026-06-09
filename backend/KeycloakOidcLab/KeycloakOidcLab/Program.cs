using System.Security.Claims;
using KeycloakOidcLab.Configuration;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddJwtAuthentication();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapGet("/api/me", (ClaimsPrincipal user) =>
{
    var nome = user.FindFirst("name")?.Value ?? user.FindFirst("preferred_username")?.Value ?? "desconhecido";
    var email = user.FindFirstValue("email");
    
    return Results.Ok(new
    {
        Nome = nome,
        Email = email,
        Claims = user.Claims.Select(c => new { c.Type, c.Value })
    });
}).RequireAuthorization();

app.MapGet("/login-bff", () =>
{
    return Results.Challenge(
        properties: new AuthenticationProperties { RedirectUri = "http://localhost:5173" },
        authenticationSchemes: new[] { AuthenticationConfiguration.BffOidcScheme }
    );
});

app.MapGet("/logout-bff", async (HttpContext context) =>
{
    await context.SignOutAsync(AuthenticationConfiguration.BffOidcScheme);
    return Results.Redirect("http://localhost:5173");
});

app.Run();
