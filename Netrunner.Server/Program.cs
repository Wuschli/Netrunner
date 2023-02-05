using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Netrunner.Server.Helpers;
using Netrunner.Server.Services;
using System.Reflection;
using MQTTnet.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var env = builder.Environment;

IdentityModelEventSource.ShowPII = true;

// Add services to the container.

services.AddTransient<IClaimsTransformation, ClaimsTransformer>();
services.AddSingleton<IUsersService, UsersService>();
services.AddSingleton<IMqttAuthService, MqttAuthService>();
services.AddSingleton<ITokenValidationService, TokenValidationService>();
services.AddAutoMapper(Assembly.GetExecutingAssembly());

services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
    {
        o.Authority = configuration["Jwt:Authority"];
        o.ClaimsIssuer = configuration["Jwt:Authority"];
        o.Audience = configuration["Jwt:Audience"];
        o.RequireHttpsMetadata = !env.IsDevelopment();
    });

services.AddAuthorization(o => { o.AddPolicy("admin", b => { b.RequireRole("admin"); }); });
services.AddControllers(o => { o.Filters.Add<HttpResponseExceptionFilter>(); });
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                },
                Scheme = "oauth2",
                Name = "oauth2",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://auth.netrunner.app/auth/realms/netrunner/protocol/openid-connect/auth"),
                TokenUrl = new Uri("https://auth.netrunner.app/auth/realms/netrunner/protocol/openid-connect/token")
            }
        }
    });
});
services.AddHostedMqttServer(options => { options.WithDefaultEndpoint(); });
services.AddMqttConnectionHandler();
services.AddConnections();

var app = builder.Build();

app.UseRouting();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.OAuthClientId("netrunner"); });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapMqtt("/mqtt");
app.UseMqttServer(server => { app.Services.GetRequiredService<IMqttAuthService>().Configure(server); });

app.Run();