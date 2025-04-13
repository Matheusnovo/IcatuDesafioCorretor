using CorretorAPI.Application.DTOs;

namespace CorretorAPI.Domain.Services.Interfaces
{
    public interface IContratacaoService
    {
        Task<IEnumerable<ContratacaoDto>> GetAllContratacoes();
        Task<ContratacaoDto> RealizarContratacao(ContratacaoDto contratacaoDto);
    }
}
