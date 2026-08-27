using Azure.Messaging.ServiceBus;
using ConsultarAcoes.Application.Interfaces.Messageria.IMessagePubliser;
using System.Text.Json;

namespace ConsultarAcoes.Infra.Messageria.ServiceBus
{
    public class AzureServiceBusPublisher : IMessagePublisher
    {
        private readonly ServiceBusSender _sender;

        public AzureServiceBusPublisher(ServiceBusClient client)
        {
            _sender = client.CreateSender("cotacoes");
        }

        public async Task PublishAsync<T>(T message, string messageId, string correlationId, string sessionId, CancellationToken cancellationToken = default)
        {
            var jsonMensagem = JsonSerializer.Serialize(message);

            var serviceBusMessage = new ServiceBusMessage(jsonMensagem)
            {
                MessageId = messageId, // identifica a mensagem
                CorrelationId = correlationId, // permite rastrear o fluxo de mensagens relacionadas
                SessionId = sessionId // permite agrupar mensagens relacionadas
            };

            try
            {
                await _sender.SendMessageAsync(serviceBusMessage, cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
