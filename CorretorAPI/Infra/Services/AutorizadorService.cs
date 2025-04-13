using CorretorAPI.Domain.Services.Interfaces;
using Newtonsoft.Json;
using System.Text.Json;

namespace CorretorAPI.Infra.Services
{
    public class AutorizadorService : IAutorizadorService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AutorizadorService> _logger;

        public AutorizadorService(HttpClient httpClient, ILogger<AutorizadorService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<AutorizacaoResult> AutorizarAsync()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://util.devi.tools/api/v2/authorize");
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                request.Headers.Add("Accept", "application/json");

                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    _logger.LogWarning("Acesso não autorizado ao serviço externo. Status: {StatusCode}", response.StatusCode);
                    return new AutorizacaoResult { Autorizado = false, StatusCode = 401 };
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Erro ao consultar serviço autorizador. Status code: {StatusCode}", response.StatusCode);
                    return new AutorizacaoResult { Autorizado = false, StatusCode = 500 };
                }

                var content = await response.Content.ReadAsStringAsync();
                var autorizacao = JsonConvert.DeserializeObject<AutorizacaoResponse>(content);
                bool autorizado = autorizacao?.Status == "success";
                int statusCode = autorizado ? 200 : 401;

                return new AutorizacaoResult
                {
                    Autorizado = autorizado,
                    StatusCode = statusCode
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar serviço autorizador");
                return new AutorizacaoResult { Autorizado = false, StatusCode = 500 };
            }
        }

        private class AutorizacaoResponse
        {
            public string Status { get; set; }
        }

    }
    public class AutorizacaoResult
    {
        public bool Autorizado { get; set; }
        public int StatusCode { get; set; }
    }
}


