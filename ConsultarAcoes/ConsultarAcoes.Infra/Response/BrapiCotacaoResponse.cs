using System.Text.Json.Serialization;

namespace ConsultarAcoes.Infra.Response
{
    public record BrapiCotacaoResponse
    {
        [JsonPropertyName("results")]
        public List<CotacaoResponse> Resultado { get; init; } = [];
    }
}
