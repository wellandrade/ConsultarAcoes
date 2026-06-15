namespace ConsultarAcoes.Function.Clients
{
    public class CotacaoApiClient
    {
        private readonly HttpClient _httpClient;

        public CotacaoApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task NotificarCotacoes()
        {
            var response = await _httpClient.PostAsync("https://consultaracoes-api-f9dpgqckhbage9bm.canadacentral-01.azurewebsites.net/api/ConsultarAcoes/NotificarCarteira", null);
        }
    }
}
