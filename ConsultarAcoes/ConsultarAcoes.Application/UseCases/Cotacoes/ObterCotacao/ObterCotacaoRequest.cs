namespace ConsultarAcoes.Application.UseCases.Cotacoes.ObterCotacao
{
    public class ObterCotacaoRequest
    {
        public string ticker { get; set; }

        public int quantidade { get; set; }
    }
}
