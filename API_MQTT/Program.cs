using Common.Authorization;
using Common.Settings;
using Entity.Entities;
using Infrastructure;
using log4net;
using log4net.Config;
using Common.Constant;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using Refit;
using System.Text.Json.Serialization;

[assembly: XmlConfigurator(ConfigFile = "log4net.config")]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

var strAppSettings = builder.Configuration.GetSection("AppSettings");
var appSettings = strAppSettings.Get<AppSettings>();

var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

builder.Services.AddDbContext<MQTTContext>((serviceProvider, options) =>
{
    var connectionString = builder.Configuration.GetConnectionString("IoT_KVN");
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.CommandTimeout(int.MaxValue);
    });
}, ServiceLifetime.Transient);

builder.Services.RegisterInfrastructureServices(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v2",
        Title = "API MQTT",
        Description = "API for MQTT",
        TermsOfService = new Uri("https://app.swaggerhub.com/eula")
    });

    s.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Name = "API-KEY",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "API key authentication header using the scheme."
    });

    s.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        s.IncludeXmlComments(xmlPath);
    }

    s.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });

builder.Services.Configure<FormOptions>(options =>
{
    options.MemoryBufferThreshold = int.MaxValue;
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = long.MaxValue;
    options.MultipartBoundaryLengthLimit = int.MaxValue;
    options.MultipartHeadersCountLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "APIDocV1");
    c.DocumentTitle = "API Documentation for RAG MQTT";
    c.RoutePrefix = "api-docs";
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

app.UseMiddleware<GuidAuthenticationMiddleware>();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers();

app.Run();