using ConsultarAcoes.Application.Interfaces.Proxies;
using ConsultarAcoes.Application.Observabilidade;
using ConsultarAcoes.Domain.Entities;

namespace ConsultarAcoes.Application.UseCases.Cotacoes.ObterCotacao
{
    public class ObterCotacaoUseCase : IObterCotacaoUseCase
    {
        private readonly ICotacaoProxy _cotacaoProxy;

        public ObterCotacaoUseCase(ICotacaoProxy cotacaoProxy)
        {
            _cotacaoProxy = cotacaoProxy;
        }

        public async Task<ObterCotacaoResponse?> Executar(ObterCotacaoRequest request)
        {
            using var activity = TracingHelper.IniciarSpan("ObterCotacaoUseCase.Executar", ("ticker", request.ticker), ("quantidade", request.quantidade));

            try
            {
                var cotacao = (Cotacao?)null;

                for (int i = 0; i < request.quantidade; i++)
                {
                    if (Random.Shared.Next(1, 100) == 1)
                    {
                        throw new Exception("Erro ao obter cotação");
                    }

                    cotacao = _cotacaoProxy.ObterCotacaoMock(request.ticker);
                }

                if (cotacao is null)
                {
                    return null;
                }

                return new ObterCotacaoResponse(request.ticker, cotacao.CotacaoAtual, cotacao.VariacaoPercentual, cotacao.FechamentoAnterior, cotacao.MaximaDia, cotacao.MinimaDia, cotacao.DataAtualizacao);

            }
            catch (Exception ex)
            {
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                activity?.AddException(ex);

                throw;
            }
        }
    }
}
