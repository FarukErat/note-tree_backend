// TODO: Make use of HttpContext.User
// TODO: Re-design dependencies for better DDD
// TODO: Use Redis Json to allow for query
// TODO: Include a random property in session and use HMAC to produce CSRF token
// TODO: Use strict CSRF token and lax session id

var builder = WebApplication.CreateBuilder(args);
var startup = new NoteTree.Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);
var app = builder.Build();
startup.Configure(app);
