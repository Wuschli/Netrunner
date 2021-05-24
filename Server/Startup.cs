//using System;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.ResponseCompression;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using System.Linq;
//using System.Text;
//using AspNetCore.Identity.MongoDbCore.Extensions;
//using AspNetCore.Identity.MongoDbCore.Infrastructure;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;
//using Netrunner.Server.Configs;
//using Netrunner.Server.Identity;
//using Netrunner.Server.Identity.Data;
//using Netrunner.Server.Mapping;
//using Netrunner.Server.Services;
//using Netrunner.Server.Services.Auth;
//using Netrunner.Shared.Services;

//namespace Netrunner.Server
//{
//    public class Startup
//    {
//        private const string ConfigName = "Netrunner";
//        public IConfiguration Configuration { get; }

//        public Startup(IConfiguration configuration)
//        {
//            Configuration = configuration;
//        }

//        public void ConfigureServices(IServiceCollection services)
//        {
//            services.AddOptions<NetrunnerConfig>()
//                .BindConfiguration(ConfigName);

//            services.AddSingleton(
//                sp =>
//                    sp.GetRequiredService<IOptions<NetrunnerConfig>>().Value
//            );

//            var config = Configuration.GetSection(ConfigName).Get<NetrunnerConfig>();

//            var mongoDbIdentityConfiguration = new MongoDbIdentityConfiguration
//            {
//                MongoDbSettings = new MongoDbSettings
//                {
//                    ConnectionString = config.Database.ConnectionString,
//                    DatabaseName = config.Database.DatabaseName
//                },
//                IdentityOptionsAction = options =>
//                {
//                    options.Password.RequireDigit = false;
//                    options.Password.RequiredLength = 8;
//                    options.Password.RequireNonAlphanumeric = false;
//                    options.Password.RequireUppercase = false;
//                    options.Password.RequireLowercase = false;

//                    // Lockout settings
//                    options.Lockout.AllowedForNewUsers = false;
//                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
//                    options.Lockout.MaxFailedAccessAttempts = 10;

//                    // ApplicationUser settings
//                    options.User.RequireUniqueEmail = false;
//                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@.-_";
//                }
//            };

//            services.ConfigureMongoDbIdentity<ApplicationUser, ApplicationRole, Guid>(mongoDbIdentityConfiguration)
//                .AddSignInManager()
//                .AddDefaultTokenProviders();

//            services.AddAuthentication(options =>
//                {
//                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//                })
//                .AddJwtBearer(options =>
//                {
//                    options.RequireHttpsMetadata = true;
//                    options.SaveToken = true;
//                    if (!string.IsNullOrWhiteSpace(config.Jwt.Secret))
//                    {
//                        options.TokenValidationParameters = new TokenValidationParameters
//                        {
//                            ValidateIssuer = true,
//                            ValidIssuer = config.Jwt.Issuer,
//                            ValidateIssuerSigningKey = true,
//                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config.Jwt.Secret)),
//                            ValidateAudience = true,
//                            ValidAudience = config.Jwt.Audience,
//                            ValidateLifetime = true,
//                            ClockSkew = TimeSpan.FromMinutes(1),
//                        };
//                    }
//                    else
//                    {
//                        options.TokenValidationParameters = new TokenValidationParameters
//                        {
//                            ValidateIssuer = true,
//                            ValidIssuer = config.Jwt.Issuer,
//                            ValidateIssuerSigningKey = false,
//                            ValidateAudience = true,
//                            ValidAudience = config.Jwt.Audience,
//                            ValidateLifetime = true,
//                            ClockSkew = TimeSpan.FromMinutes(1),
//                        };
//                    }
//                });


//            services.AddCors(o => o.AddDefaultPolicy(builder =>
//            {
//                builder
//                    .AllowAnyOrigin()
//                    .AllowAnyHeader()
//                    .AllowAnyMethod();
//            }));

//            services.AddResponseCompression(opts =>
//            {
//                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
//                    new[] {"application/octet-stream"});
//            });

//            services.AddAutoMapper(cfg => { cfg.AddProfile<MappingProfile>(); });

//            services.AddSingleton<IJwtAuthManager, JwtAuthManager>();
//            services.AddScoped<IUserManager, UserManager>();

//            services.AddWampService<IChallengeService, ChallengeService>();
//            services.AddWampService<IPingService, PingService>();
//            services.AddWampService<IAuthService, AuthService>();
//        }
//    }
//}