using ConsultarAcoes.Domain.Entities;

namespace ConsultarAcoes.Application.Interfaces.Proxies
{
    public interface ICotacaoProxy
    {
        public Task<Cotacao?> ObterCotacao(string ticket);
    }
}
