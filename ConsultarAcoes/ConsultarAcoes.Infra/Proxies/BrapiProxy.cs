using ConsultarAcoes.Application.Interfaces.Proxies;
using ConsultarAcoes.Domain.Entities;
using ConsultarAcoes.Infra.Response;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ConsultarAcoes.Infra.Proxies
{
    public class BrapiProxy : ICotacaoProxy
    {
        // Deixar em um arquivo de configuracao
        private string _token = "irzGYxGP64gP2PF9PhhJwR";
        private const string _baseUrl = "https://brapi.dev/api";
        private HttpClient _httpClient;

        public BrapiProxy()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }

        public async Task<Cotacao?> ObterCotacao(string ticket)
        {
            try
            {
                var url = $"{_baseUrl}/quote/{ticket}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Erro ao obter cotação.");
                }

                var conteudo = await response.Content.ReadAsStringAsync();

                var dados = JsonSerializer.Deserialize<BrapiCotacaoResponse>(conteudo);

                var cotacaoResponse = dados?.Resultado?.FirstOrDefault();

                if (cotacaoResponse is null)
                {
                    return null;
                }

                var cotacao = new Cotacao(cotacaoResponse.Ticker, cotacaoResponse.CotacaoAtual, cotacaoResponse.VariacaoPercentual, cotacaoResponse.FechamentoAnterior, 
                    cotacaoResponse.MaximaDia, cotacaoResponse.MinimaDia, cotacaoResponse.DataAtualizacao);

                return cotacao;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter cotação.", ex);
            }
        }
    }
}
