using CorretorAPI.Domain.Validations;
using Xunit;

namespace CorretorAPI.Tests.Unit
{
    public class CpfValidatorTests
    {
        [Theory]
        [InlineData("174.161.440-60", true)]
        [InlineData("17416144060", true)]
        [InlineData("296.147.330-63", true)]
        [InlineData("29614733063", true)]
        [InlineData("000.000.000-00", false)]
        [InlineData("111.111.111-11", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("12345", false)]
        public void IsValid_ShouldReturnExpectedResult(string cpf, bool expected)
        {
            var result = CpfValitador.IsValid(cpf);

            Assert.Equal(expected, result);
        }
    }
}
