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

// CORS Services registration in DI container
builder.Services.AddCors();

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

// Enable Swagger in all environments (public demo)
app.UseSwagger();

// SwaggerUI serves embedded resources, not static files
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "EcomDemo API V1");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "EcomDemo API - Swagger UI";
    
});

// CORS Configuration
app.UseCors(policy => policy
    .WithOrigins(
        "http://localhost:4200", // Angular dev server
        "https://ecom-demo.robertkulig-dev.eu" // URL frontend (eg. Vercel/Netlify)
    )
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials() // Required to send tokens JWT by browser
);

app.UseAuthentication();
app.UseAuthorization();

var api = app.MapGroup("/api");
api.MapAccounts();
api.MapProducts();
api.MapBaskets();

// Diagnostic endpoint
app.MapGet("/build-check", () => Results.Ok(new { 
    build = "v2-swagger-fix", 
    timestamp = DateTime.UtcNow 
}));

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .ExcludeFromDescription();

app.Run();