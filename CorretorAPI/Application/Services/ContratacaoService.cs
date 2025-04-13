using CorretorAPI.Application.DTOs;
using CorretorAPI.Domain.Exceptions;
using CorretorAPI.Domain.Models;
using CorretorAPI.Domain.Repository.Interfaces;
using CorretorAPI.Domain.Services.Interfaces;
using CorretorAPI.Domain.Validations;
using System.Globalization;

namespace CorretorAPI.Application.Services
{
    public class ContratacaoService : IContratacaoService
    {
        private readonly IContratacaoRepository _contratacaoRepository;
        private readonly IAutorizadorService _autorizadorService;
        private readonly ILogger<ContratacaoService> _logger;

        public ContratacaoService(
            IContratacaoRepository contratacaoRepository,
            IAutorizadorService autorizadorService,
            ILogger<ContratacaoService> logger)
        {
            _contratacaoRepository = contratacaoRepository;
            _autorizadorService = autorizadorService;
            _logger = logger;
        }

        public async Task<IEnumerable<ContratacaoDto>> GetAllContratacoes()
        {
            var contratacoes = await _contratacaoRepository.GetAllAsync();

            return contratacoes.Select(c => new ContratacaoDto
            {
                NomeCompleto = c.NomeCompleto,
                Cpf = c.Cpf,
                DataNascimento = c.DataNascimento.ToString("dd/MM/yyyy"),
                ValorCobertura = c.ValorCobertura
            });
        }

        public async Task<ContratacaoDto> RealizarContratacao(ContratacaoDto contratacaoDto)
        {
            ValidarContratacao(contratacaoDto);

            DateTime dataNascimento = DateTime.ParseExact(contratacaoDto.DataNascimento, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var contratacao = new Contratacao(
                contratacaoDto.NomeCompleto,
                contratacaoDto.Cpf,
                dataNascimento,
                contratacaoDto.ValorCobertura
                );

            var resultadoAutorizacao = await _autorizadorService.AutorizarAsync();
            if (!resultadoAutorizacao.Autorizado)
            {
                string mensagem = resultadoAutorizacao.StatusCode == 401
                    ? "Não autorizado. Serviço autorizador negou a solicitação."
                    : "Não foi possível finalizar a contratação. Serviço autorizador indisponível.";

                _logger.LogWarning("Contratação não autorizada para o CPF: {CPF}. StatusCode: {StatusCode}",
                    contratacaoDto.Cpf, resultadoAutorizacao.StatusCode);

                throw new BusinessException(mensagem, resultadoAutorizacao.StatusCode);
            }

            await _contratacaoRepository.AddAsync(contratacao);

            _logger.LogInformation("Contratação realizada com sucesso. ID: {ID}, CPF: {CPF}", contratacao.Id, contratacao.Cpf);

            return contratacaoDto;
        }

        private void ValidarContratacao(ContratacaoDto contratacaoDto)
        {
            if (string.IsNullOrWhiteSpace(contratacaoDto.NomeCompleto))
                throw new BusinessException("Nome completo é obrigatório.");

            if (!CpfValitador.IsValid(contratacaoDto.Cpf))
                throw new BusinessException("CPF inválido.");

            if (!DateTime.TryParseExact(contratacaoDto.DataNascimento, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataNascimento))
                throw new BusinessException("Data de nascimento inválida. Formato esperado: dd/MM/yyyy");

            int idade = CalcularIdade(dataNascimento);
            if (idade < 18)
                throw new BusinessException("O proponente deve ter no mínimo 18 anos.");

            if (contratacaoDto.ValorCobertura <= 100000)
                throw new BusinessException("Valor da cobertura deve ser maior que R$ 100.000,00.");
        }

        private int CalcularIdade(DateTime dataNascimento)
        {
            var hoje = DateTime.Today;
            var idade = hoje.Year - dataNascimento.Year;

            if (dataNascimento.Date > hoje.AddYears(-idade))
                idade--;

            return idade;
        }
    }
}
