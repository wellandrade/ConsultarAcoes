using ConsultarAcoes.Application.UseCases.Cotacoes.NotificarCotacao;
using ConsultarAcoes.Application.UseCases.Cotacoes.ObterCotacao;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;

namespace ConsultarAcoes.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultarAcoesController : ControllerBase
    {
        private readonly INotificarCotacaoUseCase _notificarCotacaoUseCase;
        private readonly IObterCotacaoUseCase _obterCotacaoUseCase;
        public ConsultarAcoesController(INotificarCotacaoUseCase notificarCotacaoUseCase, IObterCotacaoUseCase obterCotacaoUseCase)
        {
            _notificarCotacaoUseCase = notificarCotacaoUseCase;
            _obterCotacaoUseCase = obterCotacaoUseCase;
        }

        [HttpPost("NotificarCarteira")]
        public async Task<IActionResult> NotificarCarteira()
        {
            var cotacao = await _notificarCotacaoUseCase.Executar();

            return Ok(cotacao);
        }

        [HttpGet("{ticker}")]
        public async Task<IActionResult> ObterCotacao([FromRoute] string ticker, [FromQuery] int qtd, [FromHeader(Name = "Idempotency-Key")] string idempotencyKey, CancellationToken cancellationToken = default)
        {
            var request = new ObterCotacaoRequest
            {
                ticker = ticker,
                quantidade = qtd
            };

            var cotacao = await _obterCotacaoUseCase.Executar(request, idempotencyKey);

            return Ok(cotacao);
        }

        [HttpGet("Instancia")]
        public IActionResult Instancia()
        {
            return Ok(new
            {
                Machine = Environment.MachineName,
                Data = DateTime.UtcNow
            });
        }

        private static readonly List<string> _tickers = new List<string>();

        [HttpPost("memoria/{ticker}")]
        public async Task<IActionResult> AdicionarTicker(string ticker)
        {
            _tickers.Add(ticker);

            return Ok(new
            {
                Machine = Environment.MachineName,
                Tickers = _tickers
            });
        }

        [HttpGet("memoria")]
        public async Task<IActionResult> ObterTickers()
        {
            return Ok(new
            {
                Machine = Environment.MachineName,
                Tickers = _tickers
            });
        }

    }
}
