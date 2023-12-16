var builder = WebApplication.CreateBuilder(args);
var startup = new NoteTree.Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);
var app = builder.Build();
startup.Configure(app);
