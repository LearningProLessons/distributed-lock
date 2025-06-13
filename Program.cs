using System.Net;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Register Redis distributed lock factory (RedLock)
builder.Services.AddSingleton<IDistributedLockFactory>(_ =>
{
    var redisEndpoints = new[]
    {
        new RedLockEndPoint { EndPoint = new DnsEndPoint("redis", 6379) } // نام سرویس Redis در داکر-کامپوز
    };
    return RedLockFactory.Create(redisEndpoints);
});

// Register a singleton state to track leadership
builder.Services.AddSingleton<LeaderState>();

var app = builder.Build();

var leaderState = app.Services.GetRequiredService<LeaderState>();
var lockFactory = app.Services.GetRequiredService<IDistributedLockFactory>();

// Background task to try acquiring leadership lock periodically
_ = Task.Run(async () =>
{
    while (true)
    {
        using var lockHandle = await lockFactory.CreateLockAsync("leader-lock", TimeSpan.FromSeconds(10));
        if (lockHandle.IsAcquired)
        {
            leaderState.IsLeader = true;
            Console.WriteLine("🔵 I am the leader!");
            await Task.Delay(5000);
        }
        else
        {
            leaderState.IsLeader = false;
            Console.WriteLine("🟡 Standby...");
            await Task.Delay(3000);
        }
    }
});

// ساده‌ترین endpoint برای تست وضعیت لاک
app.MapGet("/lock", (LeaderState leader) =>
{
    return leader.IsLeader ? Results.Ok("I'm the leader and I serve this.") : Results.StatusCode(503);
});

// Endpoint اصلی که فقط نود Leader اجازه داره جواب بده
app.MapGet("/process", (LeaderState leader) =>
{
    if (!leader.IsLeader)
        return Results.StatusCode(503);

    var id = Guid.NewGuid();
    Console.WriteLine($"✔️ Leader handled the request: {id}");
    return Results.Ok($"Handled by leader: {id}");
});

app.Run();

// کلاس وضعیت لیدر (volatile bool برای thread-safety ساده)
class LeaderState
{
    public volatile bool IsLeader;
}
