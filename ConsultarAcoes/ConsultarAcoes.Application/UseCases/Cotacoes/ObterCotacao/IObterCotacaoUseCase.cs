namespace ConsultarAcoes.Application.UseCases.Cotacoes.ObterCotacao
{
    public interface IObterCotacaoUseCase
    {
        Task<ObterCotacaoResponse?> Executar(ObterCotacaoRequest request);
    }
}
