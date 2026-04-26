using System.Net;
using System.Text.Json;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Caching.Memory;
using MultiplyApi.Models.Responses;
using MultiplyApi.Services;

const string RateLimitPolicy = "PerClientFixedWindow";
const int WindowSeconds = 60;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddScoped<IMultiplyService, MultiplyService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy(RateLimitPolicy, context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromSeconds(WindowSeconds)
            }
        )
    );

    options.OnRejected = async (context, cancellationToken) =>
    {
        var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
        var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        if (!cache.TryGetValue($"window_{ip}", out DateTime windowStart))
        {
            windowStart = DateTime.UtcNow;
        }
        var availableAt = windowStart.Add(TimeSpan.FromSeconds(WindowSeconds));
        var retryAfter = Math.Max(1, (int)Math.Ceiling((availableAt - DateTime.UtcNow).TotalSeconds));

        var response = new RateLimitResponse(retryAfter, availableAt);

        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";
        context.HttpContext.Response.Headers.RetryAfter = retryAfter.ToString();

        await context.HttpContext.Response.WriteAsync(
            JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }),
            cancellationToken
        );
    };
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAngular");

app.Use(async (context, next) =>
{
    var cache = context.RequestServices.GetRequiredService<IMemoryCache>();
    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    var cacheKey = $"window_{ip}";

    cache.GetOrCreate(cacheKey, entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(WindowSeconds);
        return DateTime.UtcNow;
    });

    await next();
});

app.UseRateLimiter();
app.MapControllers();

app.Run();