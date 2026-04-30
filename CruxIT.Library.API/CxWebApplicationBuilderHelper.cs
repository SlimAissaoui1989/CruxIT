using CruxIT.Library.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace CruxIT.Library.API
{
    public static class CxWebApplicationBuilderHelper
    {
        public static void AddCorpsDefaultPolicyAllowAny(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

        }

        public static void IgnoreCyclesJsonAndSavePropertyNamingPolicy(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });
        }

        public static void AddJWTAuthentication(this WebApplicationBuilder builder, string? tokenTag = null)
        {
            //Jwt configuration starts here
            var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
            var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();

            if (jwtIssuer == null || jwtKey == null)
                return;

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtIssuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                    if (!string.IsNullOrEmpty(tokenTag))
                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                // Read JWT from HttpOnly cookie instead of Authorization header
                                if (context.Request.Cookies.TryGetValue(tokenTag, out var token))
                                {
                                    context.Token = token;
                                }
                                return Task.CompletedTask;
                            }
                        };
                });
        }

        public static void AddSwaggerGenWithBearer(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });
        }

        public static void UseGC(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<GCService>(p => new GCService());
        }
    }
}
