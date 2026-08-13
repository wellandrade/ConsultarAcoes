using System.Diagnostics;

namespace ConsultarAcoes.Application.Observabilidade
{
    public static class Observabilidade
    {
        public const string NomeFonte = "ConsultarAcoes";

        public static readonly ActivitySource activitySource = new ActivitySource(NomeFonte);
    }
}
