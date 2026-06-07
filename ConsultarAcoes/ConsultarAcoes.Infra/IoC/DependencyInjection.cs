using ConsultarAcoes.Application.Interfaces.Notificacao;
using ConsultarAcoes.Application.Interfaces.Proxies;
using ConsultarAcoes.Application.UseCases.Cotacoes.ObterCotacao;
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
            services.AddScoped<IObterCotacaoUseCase, ObterCotacaoUseCase>();
            services.AddScoped<ITelegramNotificacaoService, TelegramNotificacaoService>();

            return services;
        }
    }
}
