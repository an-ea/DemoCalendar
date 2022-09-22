using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using Calendar.Api.DependencyInjection;
using Calendar.Api.Infrastructure.Filters;
using Calendar.Api.Mapping;
using Calendar.Api.Validation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddDependencies();
var connectionString = configuration.GetValue<string>("ConnectionString");
var useInMemoryDb = string.IsNullOrEmpty(connectionString);

if (useInMemoryDb)
    services.AddInMemoryDatabaseContext();
else
    services.AddDatabaseContext(connectionString);

services.AddControllers(opt =>
    {
        opt.Filters.Add<GlobalExceptionFilter>();
        opt.Filters.Add<ValidationFilterAttribute>();
    })
    .AddFluentValidation(cfg =>
    {
        cfg.RegisterValidatorsFromAssemblyContaining<NewEventModelValidator>();
        cfg.LocalizationEnabled = false;
    })
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);


services.AddEndpointsApiExplorer();
AddCustomSwagger();

services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

ConfigureAuthService();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();


try
{
    app.Services.InitDatabase(useInMemoryDb);
}
catch (Exception e)
{
    logger.LogError(e, "Error during initialization.");
}


app.UseSwagger()
    .UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Calendar.API V1");
        options.OAuthClientId("calendarswaggerui");
        options.OAuthAppName("Calendar Swagger UI");
    });

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();


try
{
    app.Run();
}
catch (Exception e)
{
    logger.LogCritical(e, "Fatal exception.");
}



void AddCustomSwagger()
{
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Calendar HTTP API",
            Version = "v1",
            Description = "The Calendar Service HTTP API"
        });

        var identityUrl = configuration.GetValue<string>("IdentityUrl");

        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                Implicit = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri($"{identityUrl}/connect/authorize"),
                    TokenUrl = new Uri($"{identityUrl}/connect/token"),
                    Scopes = new Dictionary<string, string>
                    {
                        { "api", "Calendar API" }
                    }
                }
            }
        });

        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

        options.OperationFilter<AuthorizeCheckOperationFilter>();
    });

    services.AddFluentValidationRulesToSwagger();
}

void ConfigureAuthService()
{
    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

    var identityUrl = configuration.GetValue<string>("IdentityUrl");

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    }).AddJwtBearer(options =>
    {
        options.Authority = identityUrl;
        options.RequireHttpsMetadata = false;
        options.Audience = "api";
    });
}


public partial class Program { }