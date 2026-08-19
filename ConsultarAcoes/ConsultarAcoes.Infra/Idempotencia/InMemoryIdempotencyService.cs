using ConsultarAcoes.Application.Interfaces.Idempotencia;
using System.Collections.Concurrent;

namespace ConsultarAcoes.Infra.Idempotencia
{
    public class InMemoryIdempotencyService : IIdempotenciaService
    {
        private readonly ConcurrentDictionary<string, object> _cache = new();

        public bool TryGet<T>(string key, out T? value)
        {
            if (_cache.TryGetValue(key, out var result) && result is T typedResult)
            {
                value = typedResult;
                return true;
            }

            value = default;
            return false;
        }

        public void Set<T>(string key, T value)
        {
           _cache.TryAdd(key, value!);
        }
    }
}
