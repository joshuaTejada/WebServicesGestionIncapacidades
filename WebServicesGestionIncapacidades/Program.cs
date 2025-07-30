using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http.Features;
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// SSL
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

// Add services
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

long maxRequestBodySize = 30 * 1024 * 1024;

// Aumenta el límite para Kestrel (servidor por defecto en desarrollo)
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = maxRequestBodySize;
});

// Aumenta el límite para IIS (cuando se publica en IIS)
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = maxRequestBodySize;
});

builder.Services.AddSwaggerGen();

// Rate Limiting
builder.Services.AddOptions();
builder.Services.AddMemoryCache();

builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAnyOrigin");
app.UseHttpsRedirection();

// Usa el middleware de AspNetCoreRateLimit
app.UseIpRateLimiting();

app.UseAuthorization();

app.MapControllers();

app.Run();