using ConsultarAcoes.Application.Interfaces.Idempotencia;
using ConsultarAcoes.Application.Interfaces.Messageria.IMessagePubliser;
using ConsultarAcoes.Application.Interfaces.Notificacao;
using ConsultarAcoes.Application.Interfaces.Proxies;
using ConsultarAcoes.Application.UseCases.Cotacoes.NotificarCotacao;
using ConsultarAcoes.Application.UseCases.Cotacoes.ObterCotacao;
using ConsultarAcoes.Infra.Idempotencia;
using ConsultarAcoes.Infra.Messageria.ServiceBus;
using ConsultarAcoes.Infra.Notificacao;
using ConsultarAcoes.Infra.Proxies;
using Microsoft.Extensions.DependencyInjection;

namespace ConsultarAcoes.Infra.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<ICotacaoProxy, BrapiProxy>();
            services.AddScoped<INotificarCotacaoUseCase, NotificarCotacaoUseCase>();
            services.AddScoped<IObterCotacaoUseCase, ObterCotacaoUseCase>();
            services.AddScoped<ITelegramNotificacaoService, TelegramNotificacaoService>();
            
            services.AddSingleton<IIdempotenciaService, InMemoryIdempotencyService>();
            services.AddSingleton<IMessagePublisher, AzureServiceBusPublisher>();

            return services;
        }
    }
}
