namespace ConsultarAcoes.Application.Exceptions
{
    public class IntegracaoExternaException : Exception
    {
        public IntegracaoExternaException(string api, string mensagem, int? statusCode, Exception innerException) : base (mensagem, innerException)
        {
            Api = api;
            StatusCode = statusCode;
        }

        public string Api { get; }
        public int? StatusCode { get; }
    }
}
