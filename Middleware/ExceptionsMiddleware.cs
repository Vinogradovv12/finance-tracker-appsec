using FinanceTracker.Api.Common;
using FinanceTracker.Api.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FinanceTracker.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IAppLogger _logger;
    
    public ExceptionMiddleware (RequestDelegate next, IAppLogger ConsoleLogger)
    {
        _next = next;
        _logger = ConsoleLogger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            _logger.Error(ex.Message);

            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Invalid request",
                details = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error(ex.Message);

            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Invalid operation",
                details = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.Warning(ex.Message);

            context.Response.StatusCode = 403;

            await context.Response.WriteAsJsonAsync(new
            {
                error = "Forbiden"
            });
        }
        catch (UserAlreadyExistsException ex)
        {
            // We log the real situation
            _logger.Warning(ex.Message);

            // We are changing the response to a more generic one to avoid leaking information about existing users
            context.Response.StatusCode = 200;  

            var safeResponse = new
            {
                message = "Please check your email and confirm your registration"
            };

            await context.Response.WriteAsJsonAsync(safeResponse);
        }
        catch (InvalidCredentialsException ex)
        {
            _logger.Warning(ex.Message);

            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { message = "Invalid email or password." });
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
            {
                _logger.Warning("Two operations change the same data at the same time");

                context.Response.StatusCode = 409;

                var safeResponse = new
                {
                    message = "Confilct occured. Please retry"
                };

                await context.Response.WriteAsJsonAsync(safeResponse);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);

            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Internal server error"
            });
        }
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseException(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}