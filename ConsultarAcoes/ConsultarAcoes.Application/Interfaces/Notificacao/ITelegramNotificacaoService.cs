namespace ConsultarAcoes.Application.Interfaces.Notificacao
{
    public interface ITelegramNotificacaoService
    {
        Task EnviarMensagem(string mensagem);
    }
}
