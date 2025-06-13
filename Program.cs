using System.Net;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;

var builder = WebApplication.CreateBuilder(args);

// REDIS LOCK
builder.Services.AddSingleton<IDistributedLockFactory>(_ =>
{
    var redisEndpoints = new[]
    {
        new RedLockEndPoint { EndPoint = new DnsEndPoint("redis", 6379) }
    };
    return RedLockFactory.Create(redisEndpoints);
});

builder.Services.AddSingleton<LeaderState>();

var app = builder.Build();

var leaderState = app.Services.GetRequiredService<LeaderState>();
var lockFactory = app.Services.GetRequiredService<IDistributedLockFactory>();

// Try to acquire leader lock every few seconds
_ = Task.Run(async () =>
{
    while (true)
    {
        var lockHandle = await lockFactory.CreateLockAsync("leader-lock", TimeSpan.FromSeconds(10));
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

// Endpoints
app.MapGet("/lock", (LeaderState leader) =>
{
    return leader.IsLeader ? Results.Ok("I'm the leader and I serve this.") : Results.StatusCode(503);
});

app.Run();

class LeaderState
{
    public volatile bool IsLeader;
}
