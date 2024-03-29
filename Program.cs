// TODO: Consider using Dapper instead of EF Core
// TODO: Consider using SQL for Note model
// TODO: Consider removing Cipher service
// TODO: Re-design dependencies for better DDD
// TODO: Use Redis Json to allow for query
// TODO: session id: lax, HttpOnly and secure
// TODO: CSRF token: strict, not HttpOnly(as it will be sent via payload not header) and secure

var builder = WebApplication.CreateBuilder(args);
var startup = new NoteTree.Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);
var app = builder.Build();
startup.Configure(app);
