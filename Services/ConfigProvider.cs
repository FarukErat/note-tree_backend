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

        RedisConnectionString = Env.GetString("RedisConnectionString")
            ?? _configuration.GetConnectionString("Redis")
            ?? throw new("ConnectionStrings:RedisConnectionString");

        PostgreSqlConnectionString = Env.GetString("PostgreSqlConnectionString")
            ?? _configuration.GetConnectionString("PostgreSQL")
            ?? throw new("ConnectionStrings:PostgreSqlConnectionString");

        MongoDbConnectionString = Env.GetString("MongoDbConnectionString")
            ?? _configuration.GetConnectionString("MongoDB")
            ?? throw new("ConnectionStrings:MongoDbConnectionString");

        MongoDbDatabaseName = _configuration["MongoDb:DatabaseName"]
            ?? throw new("MongoDb:DatabaseName");

        HashAlgorithm = _configuration["HashAlgorithm"]
            ?? throw new("HashAlgorithm");

        SessionExpiry = TimeSpan.Parse(_configuration["SessionExpiry"]
            ?? throw new("SessionExpiry"));

        CipherKey = Env.GetString("CIPHER_KEY")
            ?? throw new("CIPHER_KEY");
    }
}