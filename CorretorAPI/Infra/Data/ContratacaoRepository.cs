using CorretorAPI.Domain.Models;
using CorretorAPI.Domain.Repository.Interfaces;

namespace CorretorAPI.Infra.Data
{
    public class ContratacaoRepository : IContratacaoRepository
    {
        private readonly List<Contratacao> _contratacoes;

        public ContratacaoRepository()
        {
            _contratacoes = new List<Contratacao>();
        }

        public async Task<IEnumerable<Contratacao>> GetAllAsync()
        {
            return await Task.FromResult(_contratacoes);
        }

        public async Task AddAsync(Contratacao contratacao)
        {
            _contratacoes.Add(contratacao);
            await Task.CompletedTask;
        }
    }
}
