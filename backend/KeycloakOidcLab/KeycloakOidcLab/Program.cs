using System.Security.Claims;
using KeycloakOidcLab.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddJwtAuthentication();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

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
}).RequireAuthorization();;

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run("http://localhost:5000");
