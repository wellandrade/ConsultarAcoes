using Azure.Messaging.ServiceBus;

namespace ConsultarAcoes.Worker.DLQ
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ServiceBusClient _client;
        private readonly ServiceBusReceiver _deadLetter;
        private readonly ServiceBusSender _sender;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;

            var connectionString = configuration["AzureServiceBus:ConnectionString"];
            var queueName = configuration["AzureServiceBus:QueueName"];

            _client = new ServiceBusClient(connectionString);
            _deadLetter = _client.CreateReceiver(queueName, new ServiceBusReceiverOptions()
            {
                SubQueue = SubQueue.DeadLetter
            });

            _sender = _client.CreateSender(queueName);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker iniciado");
            ServiceBusReceivedMessage? mensagem = null;

            while (!cancellationToken.IsCancellationRequested)
            {
                mensagem = await _deadLetter.ReceiveMessageAsync(maxWaitTime: TimeSpan.FromSeconds(5), cancellationToken: cancellationToken);
                if (mensagem is null)
                {
                    continue;
                }

                _logger.LogInformation("""
                    Mensagem da DLQ:
                    MessageId: {MessageId}
                    Body: {Body}
                    DeliveryCount: {DeliveryCount}
                    DeadLetterReason: {DeadLetterReason}
                    DeadLetterErrorDescription: {DeadLetterErrorDescription}
                    """, mensagem.MessageId, mensagem.Body.ToString(), mensagem.DeliveryCount, mensagem.DeadLetterReason, mensagem.DeadLetterErrorDescription);

                var novaMensagem = new ServiceBusMessage(mensagem.Body); // cria nova mensagem

                await _sender.SendMessageAsync(novaMensagem, cancellationToken);  // envia mensagem para a fila principal (ordens)

                await _deadLetter.CompleteMessageAsync(mensagem, cancellationToken); // remove a mensagem da DLQ após o reenvio para a fila principal
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _deadLetter.DisposeAsync();
            await _sender.DisposeAsync();
            await _client.DisposeAsync();

            await base.StopAsync(cancellationToken);
        }

    }
}
