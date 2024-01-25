using DotNetEnv;
namespace NoteTree.Authentication.Services;

public class ConfigProvider
{
    private readonly IConfiguration _configuration;
    public readonly string RedisConnectionString;
    public readonly string PostgreSqlConnectionString;
    public readonly string MongoDbConnectionString;
    public readonly string MongoDbDatabaseName;
    public readonly string HashAlgorithm;
    public readonly string CipherKey;
    public readonly TimeSpan SessionExpiry;

    public ConfigProvider(IConfiguration configuration)
    {
        _configuration = configuration;
        Env.Load();

        RedisConnectionString = Environment.GetEnvironmentVariable("RedisConnectionString")
            ?? _configuration.GetConnectionString("Redis")
            ?? throw new("ConnectionStrings:RedisConnectionString");

        PostgreSqlConnectionString = Environment.GetEnvironmentVariable("PostgreSqlConnectionString")
            ?? _configuration.GetConnectionString("PostgreSQL")
            ?? throw new("ConnectionStrings:PostgreSqlConnectionString");

        MongoDbConnectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString")
            ?? _configuration.GetConnectionString("MongoDB")
            ?? throw new("ConnectionStrings:MongoDbConnectionString");

        MongoDbDatabaseName = _configuration["MongoDb:DatabaseName"]
            ?? throw new("MongoDb:DatabaseName");

        HashAlgorithm = _configuration["HashAlgorithm"]
            ?? throw new("HashAlgorithm");

        SessionExpiry = TimeSpan.Parse(_configuration["SessionExpiry"]
            ?? throw new("SessionExpiry"));

        CipherKey = Environment.GetEnvironmentVariable("CIPHER_KEY")
            ?? throw new("CIPHER_KEY");
    }
}