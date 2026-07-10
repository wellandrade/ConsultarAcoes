using ConsultarAcoes.API.Middlewares;
using ConsultarAcoes.Infra.IoC;
using ConsultarAcoes.Infra.Notificacao;
using OpenTelemetry.Metrics;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructure();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter();
    });

builder.Services.Configure<TelegramOptions>(builder.Configuration.GetSection("Telegram"));

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();
app.UseMiddleware<ExceptionMiddleware>();

app.MapHealthChecks("/health");
app.MapPrometheusScrapingEndpoint("/metrics");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
