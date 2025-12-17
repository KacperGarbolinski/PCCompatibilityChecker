using System.Text;
using System.Text.Json;

namespace PCCompatibilityChecker.Services;

public class OllamaService
{
    private readonly HttpClient _httpClient;
    private readonly string _model = "llama3.2";

    public OllamaService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:11434"),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public async Task<string> GetCompatibilityAdviceAsync(string situation)
    {
        try
        {
            // Log request
            Console.WriteLine("\n[DEBUG] Wysyłanie zapytania do Ollama API...");

            var prompt = $"Jesteś ekspertem od kompatybilności części komputerowych. " +
                        $"Użytkownik pyta: '{situation}'. " +
                        $"Odpowiedz krótko (2-3 zdania) po polsku.";

            var request = new
            {
                model = _model,
                prompt = prompt,
                stream = false
            };

            var jsonRequest = JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine($"Request JSON:\n{jsonRequest}\n");

            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/generate", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response JSON:\n{responseJson}\n");

            var result = JsonSerializer.Deserialize<OllamaResponse>(responseJson);

            if (result == null || string.IsNullOrWhiteSpace(result.Response))
            {
                return "Nie udało się uzyskać odpowiedzi od AI.";
            }

            return $"💡 **Rada AI:** {result.Response.Trim()}";
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[ERROR] Błąd HTTP: {ex.Message}");
            return "AI niedostępne. Upewnij się, że Ollama jest uruchomiona (http://localhost:11434).";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}");
            return "Wystąpił błąd podczas komunikacji z AI.";
        }
    }

    private class OllamaResponse
    {
        public string Response { get; set; } = string.Empty;
    }
}