using NewAttendanceCalculationAPI.AttendanceCalculationAPI.Services.AttendanceServices;
using NewAttendanceCalculationAPI.EntityFramework;
using NewAttendanceCalculationAPI.Helpers.AttendanceHelper.Services;
using NewAttendanceCalculationAPI.Services.AttendanceServices;
using NewAttendanceCalculationAPI.Services.BiometricDeviceServices;
using NewAttendanceCalculationAPI.Services.OdooServices;
using NewAttendanceCalculationAPI.Services.OllamaServices;
using AutoMapper;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Data;
using System.Reflection;
using NewAttendanceCalculationAPI.Services.HttpClientServices.Dto;
using NewAttendanceCalculationAPI.Services.OdooServices.Dto;
using NewAttendanceCalculationAPI.Services.HttpClientServices;
using NewAttendanceCalculationAPI.Services.BiometricDeviceServices.Dto;
using NewAttendanceCalculationAPI.Helpers.Dto;
using NewAttendanceCalculationAPI.Helpers;
using AttendanceCalculationAPI.Profiles;

var builder = WebApplication.CreateBuilder(args);


// AI Code

var modelPath = Path.Combine(Directory.GetCurrentDirectory(), "phi-2.Q8_0.gguf");




// Clear default providers and use Serilog exclusively
builder.Logging.ClearProviders();


// Configure Serilog - Single configuration point
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Start with Debug to capture all logs
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.MSSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "Logs",
            AutoCreateSqlTable = true,
            BatchPostingLimit = 1, // For testing - remove in production
            BatchPeriod = TimeSpan.FromSeconds(1) // For testing - remove in production
        },
        columnOptions: GetColumnOptions(),
        restrictedToMinimumLevel: LogEventLevel.Information) // Ensure Info and above go to DB
    .CreateLogger();

// Register Serilog
builder.Host.UseSerilog();

////Add support to logging with SERILOG
//builder.Host.UseSerilog((context, configuration) =>
//    configuration.ReadFrom.Configuration(context.Configuration));


// Add Hangfire services
builder.Services.AddHangfire(config =>
{
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));

});
builder.Services.AddHangfireServer(); // Starts the Hangfire background job server

// Add services to the container.

//declare the DbContext that we are using it and connect it with the connection string 
builder.Services.AddDbContext<HRSystemServiceContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers().AddNewtonsoftJson(s=> { s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); });

builder.Services.AddScoped<IAttendanceHelperService, AttendanceHelperService>();
builder.Services.AddScoped<IOdooDataFetchingService, OdooDataFetchingService>();
builder.Services.AddScoped<IOdooPushingDataService, OdooPushingDataService>();
builder.Services.AddScoped<IAttendanceCalculationService, AttendanceCalculationService>();
builder.Services.AddScoped<IBiometricDeviceDataFetchingService ,BiometricDeviceDataFetchingService>();
builder.Services.AddSingleton<IOdooTokenService,OdooTokenService>();
builder.Services.AddTransient<HttpClientService>();
builder.Services.AddHttpClient<IOdooPushingDataService, OdooPushingDataService>();

builder.Services.AddHttpClient<OllamaService>();
//

//
builder.Services.AddHttpClient();


builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<HttpClientSettings>>().Value);

builder.Services.Configure<HttpClientSettings>(builder.Configuration.GetSection("HttpClientSettings"));
builder.Services.Configure<OdooLoginConfig>(builder.Configuration.GetSection("OdooLoginConfig"));
builder.Services.Configure<BiometricApiConfig>(builder.Configuration.GetSection("BiometricApiConfig"));





builder.Services.Configure<ApiEndpoints>(
    builder.Configuration.GetSection("ApiEndpoints"));



// Register Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Attendance Calculation API",
        Version = "v1"
    });

    // Optional: Include XML comments for better documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});


//builder.Services.AddScoped<ICommanderRepo, MockCommanderRepo>(); //This is mock will not return a real data
builder.Services.AddScoped<HelperService>();

// Manually register AutoMapper
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new MappingProfile());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

var app = builder.Build();

// Add this early to catch startup errors
app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
    };
});

try
{
    Log.Information("Application starting up");
// Use Hangfire dashboard (optional, for monitoring)
app.UseHangfireDashboard("/ServiceDashbaord");


RecurringJob.AddOrUpdate<AttendanceCalculationService>( "daily-attendance", service => service.GetAttendanceLogs(), Cron.Daily(3),
    TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time"));




    // Enable Swagger in development mode
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        });
    }


    app.Use(async (context, next) =>
    {
        // Enable buffering to allow reading the request body multiple times
        context.Request.EnableBuffering();

        // Read the request body
        var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0; // Reset the stream position

        using (LogContext.PushProperty("RequestBody", requestBody))
        using (LogContext.PushProperty("IpAddress", context.Connection.RemoteIpAddress?.ToString()))
        {
            await next();
        }
    });

    // Configure the HTTP request pipeline.

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
    throw;
}
finally
{
    Log.CloseAndFlush();
}


#region Helpers

// Helper method to configure column options
static ColumnOptions GetColumnOptions()
{
    var columnOptions = new ColumnOptions();

    // Standard columns
    columnOptions.Store.Remove(StandardColumn.Properties);
    columnOptions.Store.Remove(StandardColumn.MessageTemplate);
    columnOptions.Store.Add(StandardColumn.LogEvent);

    // Custom columns
    columnOptions.AdditionalColumns = new List<SqlColumn>
    {
        new SqlColumn { ColumnName = "UserId", DataType = SqlDbType.NVarChar, DataLength = 50, AllowNull = true },
        new SqlColumn { ColumnName = "IpAddress", DataType = SqlDbType.NVarChar, DataLength = 50, AllowNull = true },
        new SqlColumn { ColumnName = "ActionName", DataType = SqlDbType.NVarChar, DataLength = 100, AllowNull = true },
        new SqlColumn { ColumnName = "RequestBody", DataType = SqlDbType.NVarChar, DataLength = -1, AllowNull = true },
        new SqlColumn { ColumnName = "SourceContext", DataType = SqlDbType.NVarChar, DataLength = 128, AllowNull = true }
    };

    return columnOptions;
}

#endregion


