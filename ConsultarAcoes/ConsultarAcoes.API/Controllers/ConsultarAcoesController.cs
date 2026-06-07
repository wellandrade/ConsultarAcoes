using ConsultarAcoes.Application.UseCases.Cotacoes.ObterCotacao;
using Microsoft.AspNetCore.Mvc;

namespace ConsultarAcoes.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultarAcoesController : ControllerBase
    {
        private readonly IObterCotacaoUseCase _obterCotacaoUseCase;
        public ConsultarAcoesController(IObterCotacaoUseCase obterCotacaoUseCase)
        {
            _obterCotacaoUseCase = obterCotacaoUseCase;
        }

        [HttpPost("NotificarCarteira")]
        public async Task<IActionResult> NotificarCarteira()
        {
            var request = new ObterCotacaoRequest();

            var cotacao = await _obterCotacaoUseCase.Executar(request);

            if (cotacao is null)
            {
                // return NotFound($"Cotação não encontrada para o ticker: {ticker}");
            }

            return Ok(cotacao);
        }
    }
}
