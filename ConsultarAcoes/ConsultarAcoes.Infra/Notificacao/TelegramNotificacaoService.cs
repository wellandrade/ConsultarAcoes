using ConsultarAcoes.Application.Interfaces.Notificacao;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace ConsultarAcoes.Infra.Notificacao
{
    public class TelegramNotificacaoService : ITelegramNotificacaoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _urlBase = "https://api.telegram.org/bot";
        private readonly TelegramOptions _options;

        public TelegramNotificacaoService(IOptions<TelegramOptions> options)
        {
            _httpClient = new HttpClient();
            _options = options.Value;
        }

        public async Task EnviarMensagem(string mensagem, string sigla = "")
        {
            foreach (var destinatario in _options.ListaDestinatarios)
            {
                if (!string.IsNullOrWhiteSpace(sigla) && destinatario.SiglasBloqueadas.Contains(sigla, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                var response = await _httpClient.PostAsJsonAsync($"{_urlBase}{destinatario.Token}/sendMessage", new
                {
                    chat_id = destinatario.ChatId,
                    text = mensagem
                });
            }
        }
    }
}
