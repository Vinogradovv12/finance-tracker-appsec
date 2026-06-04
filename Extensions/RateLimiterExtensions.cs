using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace FinanceTracker.Api.Extensions;

public static class RateLimiterExtensions
{
    public static IServiceCollection AddCustomRateLimiter(this IServiceCollection services)
    {
        return services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddFixedWindowLimiter(policyName: "fixed", opt =>
            {
                opt.PermitLimit = 10;
                opt.Window = TimeSpan.FromSeconds(10);
                opt.QueueLimit = 0;
            });

            options.AddSlidingWindowLimiter("sliding", opt =>
            {
                opt.PermitLimit = 10;
                opt.Window = TimeSpan.FromSeconds(10);
                opt.SegmentsPerWindow = 5;
            });

            options.AddPolicy("auth-policy", httpContext =>
            {
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var path = httpContext.Request.Path.Value?.ToLower() ?? "unknown";

                int limit = 5;
                var window = TimeSpan.FromMinutes(1);

                if (path.Contains("/register"))
                {
                    limit = 3;
                    window = TimeSpan.FromMinutes(5);
                }

                var partitionKey = $"{ip}|{path}";

                return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = limit,
                    Window = window,
                    QueueLimit = 0
                });
            });

            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.ContentType = "application/json";

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    error = "Too many requests"
                }, token);
            };
        });
    }
}