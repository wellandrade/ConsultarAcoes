using Azure.Messaging.ServiceBus;

namespace ConsultarAcoes.Worker.TopicSubscription
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ServiceBusClient _client;
        private readonly ServiceBusReceiver _receiver;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;

            var connectionString = configuration["AzureServiceBus:ConnectionString"];

            _client = new ServiceBusClient(connectionString);

            _receiver = _client.CreateReceiver("cotacoes", "telegram");
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker iniciado");

            while (!cancellationToken.IsCancellationRequested)
            {
                var mensagem = await _receiver.ReceiveMessageAsync(maxWaitTime: TimeSpan.FromSeconds(5), cancellationToken: cancellationToken);

                if (mensagem is null)
                {
                    continue;
                }

                await _receiver.CompleteMessageAsync(mensagem, cancellationToken);
            }
        }


        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.DisposeAsync();

            await base.StopAsync(cancellationToken);
        }

    }
}
