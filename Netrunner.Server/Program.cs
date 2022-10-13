using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Logging;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var env = builder.Environment;

// prevent from mapping "sub" claim to nameidentifier.
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
IdentityModelEventSource.ShowPII = true;


// Add services to the container.

services.AddTransient<IClaimsTransformation, ClaimsTransformer>();

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
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();