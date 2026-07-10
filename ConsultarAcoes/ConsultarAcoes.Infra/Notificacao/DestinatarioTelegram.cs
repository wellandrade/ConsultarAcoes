namespace ConsultarAcoes.Infra.Notificacao
{
    public sealed class DestinatarioTelegram
    {
        public string Nome { get; private set; }
        public string ChatId { get; private set; }
        public string Token { get; private set; }
        public List<string> SiglasBloqueadas { get; private set; }

        public DestinatarioTelegram(string nome, string chatId, string token)
        {
            Nome = nome;
            ChatId = chatId;
            Token = token;
            SiglasBloqueadas = new List<string>();
        }
    }
}
