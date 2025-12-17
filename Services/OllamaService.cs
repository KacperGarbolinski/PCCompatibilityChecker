using System.Net.Http.Json;
using System.Text.Json;

namespace PCCompatibilityChecker.Services
{
    public class OllamaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _model;

        public OllamaService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:11434/"),
                Timeout = TimeSpan.FromSeconds(30)
            };
            _model = "llama3.2";
        }

        public async Task<string> GetAdviceAsync(string question)
        {
            try
            {
                Console.WriteLine($"📤 Wysyłanie do AI: {question}");

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
                    Console.WriteLine($"📥 Otrzymano odpowiedź: {content.Length} znaków");

                    using var doc = JsonDocument.Parse(content);
                    if (doc.RootElement.TryGetProperty("response", out var responseElement))
                    {
                        return responseElement.GetString() ?? "Brak odpowiedzi";
                    }
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Błąd HTTP: {response.StatusCode} - {error}");
                }

                return "Nie udało się uzyskać odpowiedzi od AI";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Wyjątek: {ex.Message}");
                return $"Błąd połączenia z AI: {ex.Message}";
            }
        }
    }
}