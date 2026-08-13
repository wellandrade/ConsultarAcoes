using ConsultarAcoes.Application.UseCases.Cotacoes.NotificarCotacao;
using ConsultarAcoes.Application.UseCases.Cotacoes.ObterCotacao;
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

            if (cotacao is null)
            {
                // return NotFound($"Cotação não encontrada para o ticker: {ticker}");
            }

            return Ok(cotacao);
        }
        
        [HttpGet("{ticker}")]
        public async Task<IActionResult> ObterCotacao([FromRoute]string ticker, [FromQuery] int qtd = 1, CancellationToken cancellationToken = default)
        {
            var request = new ObterCotacaoRequest
            {
                ticker = ticker,
                quantidade = qtd
            };

            var cotacao = await _obterCotacaoUseCase.Executar(request);

            if (cotacao is null)
            {
                // return NotFound($"Cotação não encontrada para o ticker: {ticker}");
            }

            return Ok(cotacao);
        }
    }
}
