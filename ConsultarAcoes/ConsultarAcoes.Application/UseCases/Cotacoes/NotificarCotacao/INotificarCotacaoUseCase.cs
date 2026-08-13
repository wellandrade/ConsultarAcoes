using ConsultarAcoes.Application.UseCases.Cotacoes.ObterCotacao;

namespace ConsultarAcoes.Application.UseCases.Cotacoes.NotificarCotacao
{
    public interface INotificarCotacaoUseCase
    {
        Task<NotificarCotacaoResponse?> Executar();
    }
}
