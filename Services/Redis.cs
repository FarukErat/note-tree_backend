using StackExchange.Redis;
using Newtonsoft.Json;
using Workout.Interfaces;
using Workout.Models;
using Workout.Constants;

namespace Workout.Services;

public class Redis : ICacheService
{
    private readonly ConnectionMultiplexer _redisConnection;
    private readonly IDatabase _database;
    private readonly TimeSpan _sessionExpiry;
    public Redis(ConfigProvider configProvider)
    {
        _redisConnection = ConnectionMultiplexer.Connect(configProvider.RedisConnectionString);
        _database = _redisConnection.GetDatabase();
        _sessionExpiry = configProvider.SessionExpiry;
    }

    public async Task SaveUserAsync(User user, HttpContext context)
    {
        // create session
        Session session = new()
        {
            UserId = user.Id!,
            UserName = user.UserName,
            Role = user.Role,
            IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
            UserAgent = context.Request.Headers["User-Agent"].ToString(),
            LastSeen = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            ExpireAt = DateTime.UtcNow.Add(_sessionExpiry),
        };
        string sessionId = Guid.NewGuid().ToString();

        // serialize session
        string sessionJson = JsonConvert.SerializeObject(session);

        // save session to redis
        await _database.StringSetAsync(sessionId, sessionJson, _sessionExpiry);

        // set cookie
        context.Response.Cookies.Append(Cookies.SessionId, sessionId, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Secure = true,
            Expires = DateTime.UtcNow.Add(_sessionExpiry)
        });
    }

    public async Task<Session?> GetSessionByIdAsync(string sessionId)
    {
        string? sessionJson = await _database.StringGetAsync(sessionId);

        if (sessionJson == null)
        {
            return null;
        }

        return JsonConvert.DeserializeObject<Session>(sessionJson);
    }

    public async Task UpdateSessionByIdAsync(string sessionId, Session session)
    {
        string sessionJson = JsonConvert.SerializeObject(session);
        await _database.StringSetAsync(sessionId, sessionJson, session.ExpireAt - DateTime.UtcNow);
    }

    public async Task DeleteSessionByIdAsync(string sessionId)
    {
        await _database.KeyDeleteAsync(sessionId);
    }
}