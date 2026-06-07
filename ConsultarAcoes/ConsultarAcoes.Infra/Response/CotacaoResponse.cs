using System.Text.Json.Serialization;

namespace ConsultarAcoes.Infra.Response
{
    public record CotacaoResponse
    {
        [JsonPropertyName("symbol")]
        public string Ticker { get; init; }

        [JsonPropertyName("shortName")]
        public string NomeEmpresa { get; init; }

        [JsonPropertyName("regularMarketPrice")]
        public decimal CotacaoAtual { get; init; }

        [JsonPropertyName("regularMarketChangePercent")]    
        public decimal VariacaoPercentual { get; init; }

        [JsonPropertyName("regularMarketPreviousClose")]
        public decimal FechamentoAnterior { get; init; }

        [JsonPropertyName("regularMarketDayHigh")]
        public decimal MaximaDia { get; init; }

        [JsonPropertyName("regularMarketDayLow")]
        public decimal MinimaDia { get; init; }

        [JsonPropertyName("regularMarketTime")]
        public DateTime DataAtualizacao { get; init; }

    }
}
