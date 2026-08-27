using Azure.Messaging.ServiceBus;

namespace ConsultarAcoes.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ServiceBusReceiver _serviceBusReceiver;

        private readonly HashSet<string> _mensagensProcessadas = new();

        private bool _simularFalhaIdempotencia = false;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;

            var connectionString = configuration["AzureServiceBus:ConnectionString"];
            var queueName = configuration["AzureServiceBus:QueueName"];

            _serviceBusClient = new ServiceBusClient(connectionString);
            _serviceBusReceiver = _serviceBusClient.CreateReceiver(queueName, new ServiceBusReceiverOptions());
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker iniciado");
            ServiceBusReceivedMessage? mensagem = null;
            var maxTentativas = 3;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    mensagem = await _serviceBusReceiver.ReceiveMessageAsync(maxWaitTime: TimeSpan.FromSeconds(5), cancellationToken: cancellationToken);
                    if (mensagem is null)
                    {
                        continue;
                    }

                    if (_mensagensProcessadas.Contains(mensagem.MessageId))
                    {
                        _logger.LogWarning("Mensagem duplicada recebida. Ignorando processamento. MessageId: {MessageId}", mensagem.MessageId);
                        await _serviceBusReceiver.CompleteMessageAsync(mensagem, cancellationToken); // Marca a mensagem como processada com sucesso para evitar reprocessamento
                        continue;
                    }

                    _logger.LogInformation($"Mensagem recebida {mensagem.Body.ToString()}");

                    await ProcessarMensagem(mensagem, cancellationToken);

                    _mensagensProcessadas.Add(mensagem.MessageId); // Marca mensagem como processada com sucesso e adiciona o ID da mensagem ao HashSet para evitar reprocessamento

                    if (_simularFalhaIdempotencia)
                    {
                        _simularFalhaIdempotencia = false; // Simula falha apenas na primeira execução
                        throw new Exception("Simulação de falha para testar idempotência");
                    }

                    await _serviceBusReceiver.CompleteMessageAsync(mensagem, cancellationToken); // Marca a mensagem como processada com sucesso

                    _logger.LogInformation("Mensagem processada com sucesso");
                }
                catch(ArgumentException ex)
                {
                    await _serviceBusReceiver.DeadLetterMessageAsync(mensagem, deadLetterReason: "Mensagem invalida", deadLetterErrorDescription: ex.Message, cancellationToken: cancellationToken);

                    _logger.LogInformation($"Mensagem enviada para DLQ. Motivo: {ex.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar mensagem");

                    if (mensagem?.DeliveryCount >= maxTentativas)
                    {
                        await _serviceBusReceiver.DeadLetterMessageAsync(mensagem, deadLetterReason: "MaxRetryApplication", deadLetterErrorDescription: "Mensagem não processada após várias tentativas", cancellationToken: cancellationToken);
                        _logger.LogError($"Mensagem devolvida para fila após {mensagem.DeliveryCount} tentativas");

                        continue;
                    }

                    if (mensagem is not null)
                    {
                        await _serviceBusReceiver.AbandonMessageAsync(mensagem, cancellationToken: cancellationToken); // Marca a mensagem como não processada, permitindo que seja reprocessada)
                        _logger.LogError("Mensagem devolvida para fila");
                    }

                }
            }
        }

        private async Task ProcessarMensagem(ServiceBusReceivedMessage mensagem, CancellationToken cancellationToken)
        {
            var conteudoMensagem = mensagem.Body.ToString();

            _logger.LogInformation("Processando: {Body}", conteudoMensagem);

            //if (conteudoMensagem.Contains("BBSE") || conteudoMensagem.Contains("BBSE"))
            //{
            //    throw new ArgumentException("Conteudo da mensagem invalido");
            //}

            // Aqui entra sua regra de negócio
            await Task.Delay(500, cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _serviceBusReceiver.DisposeAsync();
            await _serviceBusClient.DisposeAsync();

            await base.StopAsync(cancellationToken);
        }

    }
}
