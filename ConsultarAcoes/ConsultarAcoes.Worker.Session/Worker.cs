using Azure.Messaging.ServiceBus;

namespace ConsultarAcoes.Worker.Session
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ServiceBusClient _client;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;

            var connectionString = configuration["AzureServiceBus:ConnectionString"];
            var queueName = configuration["AzureServiceBus:QueueName"];

            _client = new ServiceBusClient(connectionString);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker iniciado");

            while (!cancellationToken.IsCancellationRequested)
            {
                await using var sessionReceiver = await _client.AcceptNextSessionAsync("ordens-sessions", cancellationToken: cancellationToken);
                _logger.LogInformation("********** Sessao rodando {0} ********** ", sessionReceiver.SessionId);

                while (!cancellationToken.IsCancellationRequested)
                {
                    var mensagem = await sessionReceiver.ReceiveMessageAsync(maxWaitTime: TimeSpan.FromSeconds(5), cancellationToken: cancellationToken);

                    if (mensagem is null)
                    {
                        break;
                    }

                    _logger.LogInformation("Session aceita {0} | Mensagem {1} ", mensagem.SessionId, mensagem.Body.ToString());
                }
            }
        }


        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.DisposeAsync();

            await base.StopAsync(cancellationToken);
        }

    }
}
