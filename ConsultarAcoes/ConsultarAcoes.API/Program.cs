using ConsultarAcoes.API.Middlewares;
using ConsultarAcoes.Application.Observabilidade;
using ConsultarAcoes.Infra.IoC;
using ConsultarAcoes.Infra.Notificacao;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructure();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

//var keyValue = builder.Configuration["KeyVault:Url"];
//if (!string.IsNullOrWhiteSpace(keyValue))
//{
//    builder.Configuration.AddAzureKeyVault(new Uri(keyValue), new DefaultAzureCredential());
//}

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource =>
    {
        resource.AddService("ConsultarAcoes.API");
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter();
    })
    .WithTracing(tracing =>
    {
        tracing
            .AddSource(Observabilidade.NomeFonte)
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri("http://localhost:4317");
            });
    });

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.SingleLine = true;
    options.TimestampFormat = "HH:mm:ss";
});

builder.Services.Configure<TelegramOptions>(builder.Configuration.GetSection("Telegram"));

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<TraceLogginsMiddleware>();

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

