using ConsultarAcoes.Application.DTO;
using ConsultarAcoes.Application.Interfaces.Messageria.IMessagePubliser;
using ConsultarAcoes.Application.Interfaces.Notificacao;
using ConsultarAcoes.Application.Interfaces.Proxies;
using ConsultarAcoes.Application.UseCases.Cotacoes.ObterCotacao;
using ConsultarAcoes.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ConsultarAcoes.Application.UseCases.Cotacoes.NotificarCotacao
{
    public class NotificarCotacaoUseCase : INotificarCotacaoUseCase
    {
        private readonly ICotacaoProxy _cotacaoProxy;
        private readonly ITelegramNotificacaoService _notificacaoService;
        private readonly ILogger<NotificarCotacaoUseCase> _logger;
        private readonly IMessagePublisher _messagePublisher;

        private bool _validarEnvioDeMensagemDuplicada = false;
        private bool _validarEnvioDeMensagemPorOrdem = false;

        public NotificarCotacaoUseCase(ICotacaoProxy cotacaoProxy, ITelegramNotificacaoService notificacaoService, ILogger<NotificarCotacaoUseCase> logger,
            IMessagePublisher messagePublisher)
        {
            _cotacaoProxy = cotacaoProxy;
            _notificacaoService = notificacaoService;
            _logger = logger;
            _messagePublisher = messagePublisher;
        }

        public async Task<NotificarCotacaoResponse?> Executar()
        {
            var listaTickers = ObterListarTickers();

            if (!listaTickers.Any())
            {
                throw new Exception("Nenhum ticker localizado.");
            }

            var cotacao = (Cotacao)null;
            var mensagem = "";

            var messageId = "";
            var correlationId = "";
            var cotacaoConsultada = (CotacaoConsultada)null;
            string? sessionId = "";

            foreach (var item in listaTickers)
            {
                try
                {
                    _logger.LogInformation("Iniciando a consulta da cotação. Ticker : {Ticker}", item);

                    cotacao = await _cotacaoProxy.ObterCotacao(item);

                    _logger.LogInformation("Cotação consultada com sucesso. Ticker: {Ticker} | Valor: {Valor }", item, cotacao?.CotacaoAtual);

                    if (cotacao is null)
                    {
                        continue;
                    }

                    mensagem = TratarMensagem(cotacao);

                    //TODO: para estudo, nao chamar no telegram, mas popular fila de mensagem
                    await _notificacaoService.EnviarMensagem(mensagem, item);

                    //correlationId = Guid.NewGuid().ToString();
                    //messageId = Guid.NewGuid().ToString();

                    //if (_validarEnvioDeMensagemDuplicada)
                    //{
                    //    messageId = "Teste-1234";
                    //}

                    //if (_validarEnvioDeMensagemPorOrdem)
                    //{
                    //    sessionId = item;
                    //}

                    //cotacaoConsultada = new CotacaoConsultada(item, mensagem, DateTime.UtcNow);
                    //await _messagePublisher.PublishAsync(cotacaoConsultada, messageId, correlationId, sessionId);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar a cotação para o ticker: {Ticker}", item);
                }
            }

            await _notificacaoService.EnviarMensagem("Todas as cotações foram processadas.");

            return new NotificarCotacaoResponse("Processamento finalizado");
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
                "KEPL3",
                "GARE11"
            };
        }
    }
}
