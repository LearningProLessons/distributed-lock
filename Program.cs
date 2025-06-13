using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using RedLockNet;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Redis Lock Factory Registration
builder.Services.AddSingleton<IDistributedLockFactory>(_ =>
{
    var redisEndpoints = new[]
    {
        new RedLockEndPoint { EndPoint = new DnsEndPoint("redis1", 6379) },
        new RedLockEndPoint { EndPoint = new DnsEndPoint("redis2", 6379) },
        new RedLockEndPoint { EndPoint = new DnsEndPoint("redis3", 6379) },
    };

    return RedLockFactory.Create(redisEndpoints);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// 🧪 Lock-protected endpoint
app.MapGet("/lock", async (IDistributedLockFactory lockFactory) =>
{
    await using var redLock = await lockFactory.CreateLockAsync(
        "my-dist-lock",       // lock resource
        TimeSpan.FromSeconds(10) // expiry time
    );

    if (redLock.IsAcquired)
    {
        Console.WriteLine("✅ Lock acquired by this node.");
        await Task.Delay(5000); // simulate some work
        return Results.Ok("Lock acquired and job done.");
    }

    Console.WriteLine("❌ Failed to acquire lock.");
    return Results.StatusCode(423); // 423 Locked
});

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
