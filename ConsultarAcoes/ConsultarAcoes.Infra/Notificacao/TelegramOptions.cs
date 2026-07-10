namespace ConsultarAcoes.Infra.Notificacao
{
    public sealed class TelegramOptions
    {
        public List<DestinatarioTelegram> ListaDestinatarios { get; private set; } = new List<DestinatarioTelegram>();
    }
}
