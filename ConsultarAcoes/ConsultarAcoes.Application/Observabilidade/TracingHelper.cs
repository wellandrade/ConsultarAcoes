using System.Diagnostics;

namespace ConsultarAcoes.Application.Observabilidade
{
    public static class TracingHelper
    {
        public static Activity? IniciarSpan(string nomeSpan, params (string Nome, object? Valor)[] tags)
        {
            using var activity = Observabilidade.activitySource.StartActivity(nomeSpan, ActivityKind.Internal);

            foreach (var tag in tags)
            {
                activity?.SetTag(tag.Nome, tag.Valor);
            }

            return activity;
        }
    }
}
