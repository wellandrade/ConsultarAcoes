using ConsultarAcoes.Application.Interfaces.Notificacao;
using System.Net.Http.Json;

namespace ConsultarAcoes.Infra.Notificacao
{
    public class TelegramNotificacaoService : ITelegramNotificacaoService
    {
        private readonly HttpClient _httpClient;

        private const string _token = "8532947811:AAHnJSyrzZhnQRt5iBA4EceQ432BnFzzW4U";
        private List<string> _listChatIds = new List<string> { "8708594503", "8961141383" };

        public TelegramNotificacaoService()
        {
            _httpClient = new HttpClient();
        }

        public async Task EnviarMensagem(string mensagem)
        {

            var url = $"https://api.telegram.org/bot{_token}/sendMessage";

            foreach (var chatId in _listChatIds.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                await _httpClient.PostAsJsonAsync(url, new
                {
                    chat_id = chatId,
                    text = mensagem
                });
            }
        }
    }
}
