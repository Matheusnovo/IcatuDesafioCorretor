using CorretorAPI.Domain.Models;

namespace CorretorAPI.Domain.Repository.Interfaces
{
    public interface IContratacaoRepository
    {
        Task<IEnumerable<Contratacao>> GetAllAsync();
        Task AddAsync(Contratacao contratacao);
    }
}
