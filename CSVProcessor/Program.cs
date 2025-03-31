using CsvHelper;
using CSVProcessor.Options;
using CSVProcessor.Services;
using CSVProcessor.SwaggerIgnore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using CsvContext = CSVProcessor.Database.CsvContext;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5025");

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection(nameof(DatabaseOptions)));

builder.Services.AddScoped<CsvProcessService>();

builder.Services.AddScoped<ActorService>();

builder.Services.AddScoped<DataService>();

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.DocumentFilter<RemoveUnitDocumentFilter>();
    c.SupportNonNullableReferenceTypes();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CSVProcessor API",
        Version = "v1",
        Description = "API для загрузки и работы с фильмами"
    });
});


builder.Services.AddDbContext<CsvContext>(options =>
{
    var databaseOptions = builder.Configuration.GetSection(nameof(DatabaseOptions)).Get<DatabaseOptions>();
    
    if (databaseOptions is null) throw new NullReferenceException($"Make Sure To add DatabaseOptions");
    
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


app.UseSwagger();

app.UseSwaggerUI();

app.Run();
