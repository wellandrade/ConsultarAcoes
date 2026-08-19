using ConsultarAcoes.Application.Exceptions;
using ConsultarAcoes.Application.Interfaces.Idempotencia;
using ConsultarAcoes.Application.Interfaces.Proxies;
using ConsultarAcoes.Application.Observabilidade;
using ConsultarAcoes.Domain.Entities;
using System.Diagnostics;

namespace ConsultarAcoes.Application.UseCases.Cotacoes.ObterCotacao
{
    public class ObterCotacaoUseCase : IObterCotacaoUseCase
    {
        private readonly ICotacaoProxy _cotacaoProxy;
        private readonly IIdempotenciaService _idempotencyService;

        public ObterCotacaoUseCase(ICotacaoProxy cotacaoProxy, IIdempotenciaService idempotencyService)
        {
            _cotacaoProxy = cotacaoProxy;
            _idempotencyService = idempotencyService;
        }

        public async Task<ObterCotacaoResponse?> Executar(ObterCotacaoRequest request, string idempotencyKey)
        {
            using var activity = TracingHelper.IniciarSpan("ObterCotacaoUseCase.Executar", ("ticker", request.ticker), ("quantidade", request.quantidade));

            if (_idempotencyService.TryGet<ObterCotacaoResponse>(idempotencyKey, out var resultadoAnterior))
            {
                activity?.SetTag("idempotency.hit", true);
                return resultadoAnterior;
            }

            Thread.Sleep(5000); // Simula um atraso de 1 segundo para demonstrar a idempotência

            activity?.SetTag("idempotency.hit", false);

            var qtdSucesso = 0;
            var qtdErro = 0;

            var cotacao = (Cotacao?)null;

            for (int i = 0; i < request.quantidade; i++)
            {
                try
                {
                    if (Random.Shared.Next(1, 100) == 1)
                    {
                        throw new CotacaoNaoEncontradaException(request.ticker);
                    }

                    cotacao = _cotacaoProxy.ObterCotacaoMock(request.ticker);
                    qtdSucesso++;
                }
                catch (CotacaoNaoEncontradaException ex)
                {
                    qtdErro++;
                    AdicionarEventoErro(activity, "Cotação não encontrada", request.ticker, ex);
                }
                catch(Exception ex)
                {
                    qtdErro++;
                    AdicionarEventoErro(activity, "Erro inesperado", request.ticker, ex);
                }
            }

            activity?.SetTag("sucesso", qtdSucesso);
            activity?.SetTag("erro", qtdErro);

            if (cotacao is null)
            {
                return null;
            }

            var response = new ObterCotacaoResponse(request.ticker, cotacao.CotacaoAtual, cotacao.VariacaoPercentual, cotacao.FechamentoAnterior, cotacao.MaximaDia, cotacao.MinimaDia, cotacao.DataAtualizacao);

            _idempotencyService.Set(idempotencyKey, response);

            return response;
        }

        private static void AdicionarEventoErro(Activity? activity, string nomeEvento, string ticker, Exception ex)
        {
            activity?.AddEvent(new ActivityEvent(nomeEvento, tags: new ActivityTagsCollection
            {
                ["ticker"] = ticker,
                ["erro"] = ex.Message
            }));
        }
    }
}
