using System.ComponentModel.DataAnnotations;

namespace CorretorAPI.Domain.Models
{
    public class Contratacao
    {
        public Guid Id { get; private set; }
        public string NomeCompleto { get; private set; }
        public string Cpf { get; private set; }
        public DateTime DataNascimento { get; private set; }
        public decimal ValorCobertura { get; private set; }
        public DateTime DataContratacao { get; private set; }

        public Contratacao()
        {
                
        }

        public Contratacao(string nomeCompleto, string cpf, DateTime dataNascimento, decimal cobertura)
        {
            Id = Guid.NewGuid();
            NomeCompleto = nomeCompleto;
            Cpf = cpf;
            DataNascimento = dataNascimento;
            ValorCobertura = cobertura;
            DataContratacao = DateTime.Now;
        }
    }
}
