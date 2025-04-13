
namespace CorretorAPI.Application.DTOs
{
    public class ContratacaoDto
    {
        public string NomeCompleto { get; set; }
        public string Cpf { get; set; }
        public string DataNascimento { get; set; }
        public decimal ValorCobertura { get; set; }

        public ContratacaoDto()
        {
            
        }
    }
}
