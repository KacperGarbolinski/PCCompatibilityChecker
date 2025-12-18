using System.Net.Http.Json;
using System.Text.Json;
using PCCompatibilityChecker.Clients;

namespace PCCompatibilityChecker.Services
{
    public class OllamaService : IChatClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _model;

        public OllamaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _model = "llama3.2";
        }

        public async Task<string> GetAdviceAsync(string question)
        {
            try
            {
                var request = new
                {
                    model = _model,
                    prompt = $"Jesteś ekspertem od kompatybilności części komputerowych. {question} Odpowiedz krótko (2-3 zdania) po polsku.",
                    stream = false
                };

                var response = await _httpClient.PostAsJsonAsync("api/generate", request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    using var doc = JsonDocument.Parse(content);
                    if (doc.RootElement.TryGetProperty("response", out var responseElement))
                    {
                        return responseElement.GetString() ?? "Brak odpowiedzi";
                    }
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return $"Błąd HTTP: {response.StatusCode} - {error}";
                }

                return "Nie udało się uzyskać odpowiedzi od AI";
            }
            catch (Exception ex)
            {
                return $"Błąd połączenia z AI: {ex.Message}";
            }
        }
    }
}