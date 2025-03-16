using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using CsvContext = CSVProcessor.Database.CsvContext;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5025");

builder.Services.AddDbContext<CsvContext>(options => options.UseNpgsql("ill add smth here later"));

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
