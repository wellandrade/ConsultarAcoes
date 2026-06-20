using ConsultarAcoes.Function.Clients;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ConsultarAcoes.Function.Function
{
    public class NotificarCotacoesFunction
    {
        private readonly CotacaoApiClient _cotacaoApiClient;
        private readonly ILogger<NotificarCotacoesFunction> _logger;

        public NotificarCotacoesFunction(CotacaoApiClient cotacaoApiClient, ILogger<NotificarCotacoesFunction> logger)
        {
            _cotacaoApiClient = cotacaoApiClient;
            _logger = logger;
        }

        [Function("NotificarCotacoesFunction11h")]
        public async Task Executar11h([TimerTrigger("0 30 13 * * 1-5")] TimerInfo timerInfo)
        {
            try
            {
                _logger.LogInformation("Iniciando execução da Function Notificações | NotificarCotacoesFunction11h");

                await _cotacaoApiClient.NotificarCotacoes();

                _logger.LogInformation("API executada com sucesso | NotificarCotacoesFunction11h");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar a Function | NotificarCotacoesFunction11h");
                throw;
            }
        }

        [Function("NotificarCotacoesFunction15h30")]
        public async Task Executar15h30([TimerTrigger("0 30 18 * * 1-5")] TimerInfo timerInfo)
        {
            try
            {
                _logger.LogInformation("Iniciando execução da Function Notificações | NotificarCotacoesFunction15h30");

                await _cotacaoApiClient.NotificarCotacoes();

                _logger.LogInformation("API executada com sucesso | NotificarCotacoesFunction15h30");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar a Function | NotificarCotacoesFunction15h30"); throw;
                throw;
            }
        }
    }
}
