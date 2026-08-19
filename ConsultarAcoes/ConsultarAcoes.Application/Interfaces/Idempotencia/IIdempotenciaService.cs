namespace ConsultarAcoes.Application.Interfaces.Idempotencia
{
    public interface IIdempotenciaService
    {
        bool TryGet<T>(string key, out T? value);

        void Set<T>(string key, T value);
    }
}
