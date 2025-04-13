using CorretorAPI.Application.DTOs;
using CorretorAPI.Domain.Exceptions;
using CorretorAPI.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CorretorAPI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContratacaoController : ControllerBase
    {
        private readonly IContratacaoService _contratacaoService;
        private readonly ILogger<ContratacaoController> _logger;

        public ContratacaoController(IContratacaoService contratacaoService, ILogger<ContratacaoController> logger)
        {
            _contratacaoService = contratacaoService;
            _logger = logger;
        }

        [HttpGet("contratacao")]
        [ProducesResponseType(typeof(IEnumerable<ContratacaoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ContratacaoDto>>> Get()
        {
            var contratacoes = await _contratacaoService.GetAllContratacoes();
            return Ok(contratacoes);
        }

        [HttpPost("contratar")]
        [ProducesResponseType(typeof(ContratacaoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ContratacaoDto>> Contratar(ContratacaoDto contratacaoDto)
        {
            try
            {
                var result = await _contratacaoService.RealizarContratacao(contratacaoDto);
                return CreatedAtAction(nameof(Get), result);
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex, "Erro de validação: {Message}", ex.Message);
                return StatusCode(ex.StatusCode, new ErrorResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar contratação");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse { Message = "Ocorreu um erro ao processar a solicitação." });
            }
        }

        private class ErrorResponse
        {
            public string Message { get; set; }
        }
    }
}
