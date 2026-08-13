namespace ConsultarAcoes.Application.UseCases.Cotacoes.ObterCotacao
{
    public record ObterCotacaoResponse(string ticker, decimal cotacaoAtual, decimal variacaoPercentual, decimal fechamentoAnterior, decimal maximaDia, decimal minimaDia, DateTime dataAtualizacao);
}
