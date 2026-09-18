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

// --- CONFIGURACIÓN DE TAMAÑO DE SOLICITUD Y FORMULARIO ---
long maxRequestSize = 300 * 1024 * 1024; // 300 MB

// Configuración general del servidor
builder.Services.Configure<KestrelServerOptions>(options => options.Limits.MaxRequestBodySize = maxRequestSize);
builder.Services.Configure<IISServerOptions>(options => options.MaxRequestBodySize = maxRequestSize);

// Configuración específica para formularios multipart
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = maxRequestSize;
    options.ValueLengthLimit = int.MaxValue;
    options.MemoryBufferThreshold = int.MaxValue;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
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

// Middleware de Buffering (está en el lugar correcto)
app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});

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