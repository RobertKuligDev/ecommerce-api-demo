using EcomDemo.Api;
using EcomDemo.Api.Endpoints;
using EcomDemo.Application;
using EcomDemo.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Compact;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Structured JSON logging (production-grade observability)
builder.Host.UseSerilog((_, cfg) => cfg
    .MinimumLevel.Information()
    .WriteTo.Console(new RenderedCompactJsonFormatter()));

// Composition root — each layer registers its own services
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddApi();

var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
    builder.Configuration["Jwt:Key"] ?? "ecom-demo-public-secret-32b-please-rotate"));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = "ecom-demo",
        ValidAudience = "ecom-demo",
        IssuerSigningKey = jwtKey,
        ClockSkew = TimeSpan.Zero
    });
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ecom.API — public demo",
        Version = "v1",
        Description = "Simplified public demo of the production E-commerce Platform API. " +
                      "Same API contract, in-memory persistence. Production: modular Clean Architecture, " +
                      "EF Core + PostgreSQL, Angular 18 SPA."
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

var api = app.MapGroup("/api");
api.MapAccounts();
api.MapProducts();
api.MapBaskets();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .ExcludeFromDescription();

app.Run();