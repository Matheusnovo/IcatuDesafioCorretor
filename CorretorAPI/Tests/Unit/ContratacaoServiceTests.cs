using CorretorAPI.Application.DTOs;
using CorretorAPI.Application.Services;
using CorretorAPI.Domain.Exceptions;
using CorretorAPI.Domain.Models;
using CorretorAPI.Domain.Repository.Interfaces;
using CorretorAPI.Domain.Services.Interfaces;
using CorretorAPI.Infra.Services;
using Moq;
using Xunit;

namespace CorretorAPI.Tests.Unit
{
    public class ContratacaoServiceTests
    {
        private readonly Mock<IContratacaoRepository> _mockRepository;
        private readonly Mock<IAutorizadorService> _mockAutorizadorService;
        private readonly Mock<ILogger<ContratacaoService>> _mockLogger;
        private readonly ContratacaoService _service;

        public ContratacaoServiceTests()
        {
            _mockRepository = new Mock<IContratacaoRepository>();
            _mockAutorizadorService = new Mock<IAutorizadorService>();
            _mockLogger = new Mock<ILogger<ContratacaoService>>();
            _service = new ContratacaoService(_mockRepository.Object, _mockAutorizadorService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task RealizarContratacao_WhenValidData_ShouldAddContratacao()
        {
            var dto = new ContratacaoDto
            {
                NomeCompleto = "Joao da Silva",
                Cpf = "17416144060",
                DataNascimento = "01/01/2000",
                ValorCobertura = 150000
            };

            _mockAutorizadorService.Setup(x => x.AutorizarAsync())
                .ReturnsAsync(new AutorizacaoResult { Autorizado = true, StatusCode = 200 });

            var result = await _service.RealizarContratacao(dto);

            _mockRepository.Verify(x => x.AddAsync(It.IsAny<Contratacao>()), Times.Once);
            Assert.Equal(dto.NomeCompleto, result.NomeCompleto);
            Assert.Equal(dto.Cpf, result.Cpf);
        }

        [Fact]
        public async Task RealizarContratacao_WhenAutorizadorDenies_ShouldThrowBusinessException()
        {
            var dto = new ContratacaoDto
            {
                NomeCompleto = "Joao da Silva",
                Cpf = "17416144060",
                DataNascimento = "01/01/2000",
                ValorCobertura = 150000
            };

            _mockAutorizadorService.Setup(x => x.AutorizarAsync())
                .ReturnsAsync(new AutorizacaoResult { Autorizado = false, StatusCode = 500 });

            var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.RealizarContratacao(dto));
            Assert.Contains("indisponível", exception.Message);
            Assert.Equal(500, exception.StatusCode);
        }

        [Theory]
        [InlineData("", "17416144060", "01/01/2000", 150000, "Nome completo")]
        [InlineData("Joao da Silva", "123", "01/01/2000", 150000, "CPF inválido")]
        [InlineData("Joao da Silva", "17416144060", "01/01/2010", 150000, "18 anos")]
        [InlineData("Joao da Silva", "17416144060", "01/01/2000", 50000, "cobertura")]
        public async Task RealizarContratacao_WhenInvalidData_ShouldThrowBusinessException(
            string nome, string cpf, string dataNascimento, decimal cobertura, string expectedErrorMessage)
        {
            var dto = new ContratacaoDto
            {
                NomeCompleto = nome,
                Cpf = cpf,
                DataNascimento = dataNascimento,
                ValorCobertura = cobertura
            };

            var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.RealizarContratacao(dto));
            Assert.Contains(expectedErrorMessage, exception.Message);
            Assert.Equal(400, exception.StatusCode);
        }
    }
}
