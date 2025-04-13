using CorretorAPI.Infra.Services;

namespace CorretorAPI.Domain.Services.Interfaces
{
    public interface IAutorizadorService
    {
        Task<AutorizacaoResult> AutorizarAsync();
    }
}
