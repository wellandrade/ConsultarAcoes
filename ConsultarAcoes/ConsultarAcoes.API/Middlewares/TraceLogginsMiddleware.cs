using System.Diagnostics;

namespace ConsultarAcoes.API.Middlewares
{
    public class TraceLogginsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TraceLogginsMiddleware> _logger;

        public TraceLogginsMiddleware(RequestDelegate next, ILogger<TraceLogginsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var traceId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                { "TraceId", traceId }
            }))
            {
                _logger.LogInformation("Iniciando requisição {Metodo} {Rota} com TraceId {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    traceId);
                
                await _next(context);
                
                _logger.LogInformation("Finalizando requisição. StatusCode: {StatusCode}",
                    context.Response.StatusCode);
            }
        }
    }
}
