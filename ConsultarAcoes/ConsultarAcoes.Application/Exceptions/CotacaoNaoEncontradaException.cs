namespace ConsultarAcoes.Application.Exceptions
{
    public class CotacaoNaoEncontradaException : Exception 
    {
        public CotacaoNaoEncontradaException(string ticker) : base($"Cotação não encontrada para o ticker {ticker}")
        {
                
        }
    }
}
