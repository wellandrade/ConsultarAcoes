using ConsultarAcoes.Application.Exceptions;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace ConsultarAcoes.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (IntegracaoExternaException ex)
            {
                _logger.LogError(ex, "Erro de integração com o serviço {Servico}. StatusCode {StatusCode} ", ex.Api, ex.StatusCode);

                context.Response.StatusCode = (int)HttpStatusCode.BadGateway;

                await context.Response.WriteAsJsonAsync(new
                {
                    erro = ex.Message,
                    api = ex.Api,
                    statusCodeOrigem = ex.StatusCode
                });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado: {Message}", ex.Message);

                context.Response.StatusCode = (int)HttpStatusCode.BadGateway;

                await context.Response.WriteAsJsonAsync(new
                {
                    erro = "Erro interno."
                });

            }

        }
    }
}
