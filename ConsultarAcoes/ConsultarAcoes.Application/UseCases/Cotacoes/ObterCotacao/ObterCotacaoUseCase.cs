using ConsultarAcoes.Application.Interfaces.Notificacao;
using ConsultarAcoes.Application.Interfaces.Proxies;
using ConsultarAcoes.Domain.Entities;

namespace ConsultarAcoes.Application.UseCases.Cotacoes.ObterCotacao
{
    public class ObterCotacaoUseCase : IObterCotacaoUseCase
    {
        private readonly ICotacaoProxy _cotacaoProxy;
        private readonly ITelegramNotificacaoService _notificacaoService;

        public ObterCotacaoUseCase(ICotacaoProxy cotacaoProxy, ITelegramNotificacaoService notificacaoService)
        {
            _cotacaoProxy = cotacaoProxy;
            _notificacaoService = notificacaoService;
        }

        public async Task<ObterCotacaoResponse?> Executar(ObterCotacaoRequest request)
        {
            var listaTickers = ObterListarTickers();

            if (!listaTickers.Any())
            {
                throw new Exception("Nenhum ticker localizado.");
            }

            var cotacao = (Cotacao)null;
            var mensagem = "";

            foreach (var item in listaTickers)
            {
                try
                {
                    cotacao = await _cotacaoProxy.ObterCotacao(item);

                    if (cotacao is null)
                    {
                        continue;
                    }

                    mensagem = TratarMensagem(cotacao);

                    await _notificacaoService.EnviarMensagem(mensagem);

                }
                catch (Exception ex)
                {
                    var teste = ex;
                }
                await Task.Delay(TimeSpan.FromSeconds(2));
            }

            await _notificacaoService.EnviarMensagem("Todas as cotações foram processadas.");

            return new ObterCotacaoResponse("Processamento finalizado");
        }

        private string TratarMensagem(Cotacao cotacao)
        {
            var diferenca = cotacao.CotacaoAtual - cotacao.FechamentoAnterior;
            var titulo = ObterTituloVariacao(cotacao.VariacaoPercentual);

            var mensagem =
            $"""
                {titulo} 

                📌 Ativo: {cotacao.Ticker}
                💰 Cotação: R$ {cotacao.CotacaoAtual:N2}
                📊 Variação: {cotacao.VariacaoPercentual:N2}%
                📈 Máxima do dia: R$ {cotacao.MaximaDia:N2}
                📉 Mínima do dia: R$ {cotacao.MinimaDia:N2}

                📋 Fechamento Anterior: R$ {cotacao.FechamentoAnterior:N2}
                📊 Diferença: R$ {diferenca:N2} ({cotacao.VariacaoPercentual:N2}%)

                🕐 Atualizado: {cotacao.DataAtualizacao:dd/MM/yyyy HH:mm}
                """;

            return mensagem;
        }

        private static string ObterTituloVariacao(decimal variacaoPercentual)
        {
            switch (variacaoPercentual)
            {
                case <= -3:
                    return "🚨 QUEDA FORTE";
                case <= -2:
                    return "⚠️ QUEDA RELEVANTE";
                case >= 3:
                    return "🔥 ALTA FORTE";      
                case >= 2:
                    return "✅ ALTA RELEVANTE";  
                default:
                    return "📈 COTAÇÃO";
            }
        }

        private static List<string> ObterListarTickers()
        {
            return new List<string>
            {
                "CXSE3",
                "BBSE3",
                "BBAS3",
                "TAEE11",
                "CMGI4",
                "PETR4",
                "VALE3",
                "ITSA4",
                "RANI3",
                "KEPL3"
            };
        }
    }
}
