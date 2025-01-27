using Serilog;
using Serilog.Events;
using abs_insurance.Extensions;

#region Configurations
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();
#endregion

var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services.AddControllers();
builder.Services.ConfigureCors();
builder.Services.ConfigureSwagger();
builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureServiceManager();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureRepositoryManager();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureControllerServices();
builder.Services.ConfigureJwt(builder.Configuration);

builder.Services.ConfigurePostgresContext(builder.Configuration);

builder.Services.AddSerilog((services, lc) => lc
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

builder.Services.ConfigureValidators();


#endregion

var app = builder.Build();

#region Middleware

app.UseSerilogRequestLogging();
app.ConfigureExceptionHandler();

// Allow swagger for all environments
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
#endregion