namespace ConsultarAcoes.Domain.Entities
{
    public class Cotacao
    {
        public Cotacao(string ticker, decimal cotacaoAtual, decimal variacaoPercentual, decimal fechamentoAnterior, decimal maximaDia, decimal minimaDia, DateTime dataAtualizacao)
        {
            Ticker = ticker;
            CotacaoAtual = cotacaoAtual;
            VariacaoPercentual = variacaoPercentual;
            FechamentoAnterior = fechamentoAnterior;
            MaximaDia = maximaDia;
            MinimaDia = minimaDia;
            DataAtualizacao = dataAtualizacao;

            EhValido();
        }

        public string Ticker { get; private set; }
        public decimal CotacaoAtual { get; private set; }
        public decimal VariacaoPercentual { get; private set; }
        public decimal FechamentoAnterior { get; private set; }
        public decimal MaximaDia { get; private set; }
        public decimal MinimaDia { get; private set; }
        public DateTime DataAtualizacao { get; private set; }

        public void EhValido()
        {
            if (string.IsNullOrEmpty(Ticker))
            {
                throw new Exception("Ticker da empresa invalido");
            }
        }
    }
}
