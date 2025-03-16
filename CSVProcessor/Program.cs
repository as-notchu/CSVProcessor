using CsvHelper;
using CSVProcessor.Options;
using CSVProcessor.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using CsvContext = CSVProcessor.Database.CsvContext;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5025");
builder.Services.AddControllers();
builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection(nameof(DatabaseOptions)));

builder.Services.AddScoped<CsvProcessService>();

builder.Services.AddDbContext<CsvContext>(options =>
{
    var databaseOptions = builder.Configuration.GetSection(nameof(DatabaseOptions)).Get<DatabaseOptions>();
    
    options.UseNpgsql(databaseOptions.ConnectionString);
});

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
