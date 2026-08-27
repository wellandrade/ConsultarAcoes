namespace ConsultarAcoes.Application.Interfaces.Messageria.IMessagePubliser
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, string messageId, string correlationId, string sessionId, CancellationToken cancellationToken = default);
    }
}
