using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5025");

builder.Services.AddOpenApi();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()  
    .WriteTo.File(
        $"logs/log-{DateTime.UtcNow:yyyy-MM-dd}.txt",  
        rollingInterval: RollingInterval.Day,  
        retainedFileCountLimit: 14
    )
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger);


var app = builder.Build();
app.MapControllers();

app.Run();
