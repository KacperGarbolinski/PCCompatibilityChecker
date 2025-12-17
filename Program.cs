using System.Text;
using System.Text.Json;

namespace PCCompatibilityChecker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            bool exitRequested = false;

            while (!exitRequested)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════╗");
                Console.WriteLine("║    🖥️  PC COMPATIBILITY CHECKER         ║");
                Console.WriteLine("╚══════════════════════════════════════════╝");
                Console.WriteLine();

                Console.WriteLine("Wybierz akcję:");
                Console.WriteLine("1. Sprawdź CPU + Płyta główna");
                Console.WriteLine("2. Sprawdź RAM + Płyta główna");
                Console.WriteLine("3. Sprawdź cały zestaw");
                Console.WriteLine("4. Pokaż dostępne części");
                Console.WriteLine("5. Zapytaj AI o radę 🤖");
                Console.WriteLine("6. Wyjdź");
                Console.WriteLine();
                Console.Write("Twój wybór: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CheckCpuMotherboard();
                        break;

                    case "2":
                        CheckRamMotherboard();
                        break;

                    case "3":
                        CheckFullCompatibility();
                        break;

                    case "4":
                        ShowAvailableParts();
                        break;

                    case "5": // ZAPYTAJ AI O RADĘ - POPRAWIONE!
                        Console.Clear();
                        Console.WriteLine("╔══════════════════════════════════════╗");
                        Console.WriteLine("║        🤖 KONSULTACJA Z AI          ║");
                        Console.WriteLine("╚══════════════════════════════════════╝");
                        Console.WriteLine();

                        Console.Write("❓ Twoje pytanie o kompatybilność: ");
                        string question = Console.ReadLine() ?? "";

                        if (string.IsNullOrWhiteSpace(question))
                        {
                            Console.WriteLine("\n⚠️  Pytanie nie może być puste!");
                        }
                        else
                        {
                            Console.WriteLine("\n⏳ Łączę się z AI...");

                            // AI LOGIC
                            using var client = new HttpClient();
                            client.BaseAddress = new Uri("http://localhost:11434/");
                            client.Timeout = TimeSpan.FromSeconds(30);

                            var payload = new
                            {
                                model = "llama3.2",
                                prompt = $"Jesteś ekspertem od kompatybilności części komputerowych. {question} Odpowiedz krótko po polsku.",
                                stream = false
                            };

                            try
                            {
                                // REQUEST JSON - dla screenshotu
                                Console.WriteLine("\n══════════════════════════════════════");
                                Console.WriteLine("📤 REQUEST JSON do LLM:");
                                Console.WriteLine("══════════════════════════════════════");
                                Console.WriteLine(JsonSerializer.Serialize(payload, new JsonSerializerOptions
                                {
                                    WriteIndented = true
                                }));

                                var json = JsonSerializer.Serialize(payload);
                                var content = new StringContent(json, Encoding.UTF8, "application/json");

                                var response = await client.PostAsync("api/generate", content);

                                if (response.IsSuccessStatusCode)
                                {
                                    var jsonResponse = await response.Content.ReadAsStringAsync();

                                    // RESPONSE JSON - dla screenshotu
                                    Console.WriteLine("\n══════════════════════════════════════");
                                    Console.WriteLine("📥 RESPONSE JSON z LLM:");
                                    Console.WriteLine("══════════════════════════════════════");
                                    Console.WriteLine(jsonResponse);

                                    // Parse response
                                    using var doc = JsonDocument.Parse(jsonResponse);
                                    if (doc.RootElement.TryGetProperty("response", out var responseProp))
                                    {
                                        var answer = responseProp.GetString();

                                        Console.WriteLine("\n══════════════════════════════════════");
                                        Console.WriteLine("🤖 ODPOWIEDŹ AI:");
                                        Console.WriteLine("══════════════════════════════════════");
                                        Console.WriteLine(answer);
                                        Console.WriteLine("══════════════════════════════════════\n");

                                        // Zapisz do logu
                                        LogAiConversation(question, answer ?? "Brak odpowiedzi");
                                    }
                                    else
                                    {
                                        Console.WriteLine("\n❌ Brak pola 'response' w odpowiedzi");
                                        Console.WriteLine("Pełna odpowiedź:");
                                        Console.WriteLine(jsonResponse);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"\n❌ Błąd HTTP: {response.StatusCode}");
                                    var error = await response.Content.ReadAsStringAsync();
                                    Console.WriteLine($"Szczegóły: {error}");
                                    Console.WriteLine("\n💡 Upewnij się, że Ollama jest uruchomiona: ollama serve");
                                }
                            }
                            catch (HttpRequestException)
                            {
                                Console.WriteLine("\n❌ Nie można połączyć się z Ollama");
                                Console.WriteLine("💡 Uruchom Ollama: ollama serve");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"\n❌ Błąd: {ex.Message}");
                            }
                        }

                        Console.Write("\n🔽 Naciśnij Enter, aby kontynuować...");
                        Console.ReadLine();
                        break;

                    case "6":
                        exitRequested = true;
                        Console.WriteLine("\nDo zobaczenia! 👋");
                        break;

                    default:
                        Console.WriteLine("\n❌ Nieprawidłowy wybór!");
                        Console.WriteLine("Naciśnij dowolny klawisz...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static void CheckCpuMotherboard()
        {
            Console.Clear();
            Console.WriteLine("══════════════════════════════════════");
            Console.WriteLine("   SPRAWDZANIE CPU + PŁYTA GŁÓWNA     ");
            Console.WriteLine("══════════════════════════════════════");
            Console.WriteLine("\nTa funkcja sprawdza kompatybilność procesora z płytą główną...\n");

            DisplaySampleCompatibilityData();
            WaitForKey();
        }

        private static void CheckRamMotherboard()
        {
            Console.Clear();
            Console.WriteLine("══════════════════════════════════════");
            Console.WriteLine("   SPRAWDZANIE RAM + PŁYTA GŁÓWNA    ");
            Console.WriteLine("══════════════════════════════════════");
            Console.WriteLine("\nTa funkcja sprawdza kompatybilność pamięci RAM z płytą główną...\n");

            DisplaySampleCompatibilityData();
            WaitForKey();
        }

        private static void CheckFullCompatibility()
        {
            Console.Clear();
            Console.WriteLine("══════════════════════════════════════");
            Console.WriteLine("      SPRAWDZANIE CAŁEGO ZESTAWU     ");
            Console.WriteLine("══════════════════════════════════════");
            Console.WriteLine("\nTa funkcja sprawdza kompatybilność wszystkich części...\n");

            DisplaySampleCompatibilityData();
            WaitForKey();
        }

        private static void ShowAvailableParts()
        {
            Console.Clear();
            Console.WriteLine("══════════════════════════════════════");
            Console.WriteLine("        DOSTĘPNE CZĘŚCI PC          ");
            Console.WriteLine("══════════════════════════════════════");
            Console.WriteLine();

            Console.WriteLine("┌──────────────┬──────────────────────────────┬──────────────────────┐");
            Console.WriteLine("│     Typ      │            Nazwa             │   Specyfikacja       │");
            Console.WriteLine("├──────────────┼──────────────────────────────┼──────────────────────┤");
            Console.WriteLine("│    CPU       │ Intel Core i5-13600K         │ Socket LGA1700       │");
            Console.WriteLine("│    CPU       │ AMD Ryzen 5 7600X            │ Socket AM5           │");
            Console.WriteLine("│    RAM       │ Corsair Vengeance 32GB       │ DDR5-6000, CL36      │");
            Console.WriteLine("│    RAM       │ G.Skill Trident Z 16GB       │ DDR4-3600, CL16      │");
            Console.WriteLine("│ Płyta gł.    │ MSI MAG B760                 │ LGA1700, DDR5        │");
            Console.WriteLine("│ Płyta gł.    │ ASUS TUF GAMING B650         │ AM5, DDR5            │");
            Console.WriteLine("│ Karta graf.  │ NVIDIA RTX 4070              │ 12GB GDDR6X          │");
            Console.WriteLine("│ Karta graf.  │ AMD RX 7800 XT               │ 16GB GDDR6           │");
            Console.WriteLine("│ Zasilacz     │ Seasonic Focus GX-750        │ 750W 80+ Gold        │");
            Console.WriteLine("│ Obudowa      │ Fractal Design Meshify C     │ ATX, Good airflow    │");
            Console.WriteLine("└──────────────┴──────────────────────────────┴──────────────────────┘");

            WaitForKey();
        }

        private static void DisplaySampleCompatibilityData()
        {
            Console.WriteLine("Przykładowe wyniki kompatybilności:");
            Console.WriteLine();
            Console.WriteLine("✅ Intel Core i5-13600K + MSI MAG B760: KOMPATYBILNE");
            Console.WriteLine("✅ AMD Ryzen 5 7600X + ASUS TUF B650: KOMPATYBILNE");
            Console.WriteLine("❌ Intel Core i5-13600K + ASUS TUF B650: NIEKOMPATYBILNE");
            Console.WriteLine("✅ Corsair DDR5-6000 + MSI MAG B760: KOMPATYBILNE");
            Console.WriteLine("✅ NVIDIA RTX 4070 + Seasonic 750W: KOMPATYBILNE");
            Console.WriteLine("⚠️  AMD RX 7800 XT + 550W PSU: WYMAGANE 600W+");
        }

        private static void WaitForKey()
        {
            Console.WriteLine("\n\n🔽 Naciśnij Enter, aby kontynuować...");
            Console.ReadLine();
        }

        private static void LogAiConversation(string question, string answer)
        {
            try
            {
                string logPath = "ai_conversations.log";
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n" +
                                 $"Pytanie: {question}\n" +
                                 $"Odpowiedź: {answer}\n" +
                                 new string('-', 50) + "\n";

                File.AppendAllText(logPath, logEntry);
                Console.WriteLine($"\n📝 Log zapisany do: {logPath}");
            }
            catch
            {
                // Ignoruj błędy logowania
            }
        }
    }
}