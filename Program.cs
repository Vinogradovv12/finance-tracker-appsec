using System.Text.Json.Serialization;
using FinanceTracker.Api.Services.Implementations;
using FinanceTracker.Api.Services.Interfaces;
using FinanceTracker.Api.Common;
using FinanceTracker.Api.Middleware;
using FinanceTracker.Api.Data;
using Microsoft.EntityFrameworkCore;
using FinanceTracker.Api.Infrastructure.Auth;
using FinanceTracker.Api.Extensions;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        var enumConverter = new JsonStringEnumConverter(allowIntegerValues: false);
        options.JsonSerializerOptions.Converters.Add(enumConverter);
        options.JsonSerializerOptions.UnmappedMemberHandling =
            JsonUnmappedMemberHandling.Disallow;
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<IAppLogger>();

        var errorDetails = string.Join(" | ", context.ModelState
            .Where(e => e.Value!.Errors.Count > 0)
            .Select(e => $"{e.Key}: {string.Join(", ", e.Value!.Errors.Select(er => er.ErrorMessage))}"));

        logger.Warning($"Validation failed: {errorDetails}");

        return new BadRequestObjectResult(new
        {
            error = "Invalid request data"
        });
    };
});

builder.Services.AddCustomRateLimiter();

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(nameof(JwtOptions)));

builder.Services.AddApiAuthentication(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddSingleton<IAppLogger, ConsoleLogger>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();

var app = builder.Build();

app.UseException();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRateLimiter();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Finance Tracker API")
            .WithTheme(ScalarTheme.BluePlanet)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

        options.Authentication = new ScalarAuthenticationOptions
        {
            PreferredSecuritySchemes = ["Bearer"]
        };
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();