using Microsoft.Extensions.DependencyInjection;
using PCCompatibilityChecker.Clients;
using PCCompatibilityChecker.Models;
using Spectre.Console;
using System.Text.Json;

using ModelsComponent = PCCompatibilityChecker.Models.Component;

namespace PCCompatibilityChecker
{
    public class CompatibilityApp
    {
        private readonly ICompatibilityClient _compatibilityClient;
        private readonly IChatClient _chatClient;
        private readonly IGitHubClient _gitHubClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private List<ModelsComponent> _components = new List<ModelsComponent>();

        public CompatibilityApp(
            ICompatibilityClient compatibilityClient,
            IChatClient chatClient,
            IGitHubClient gitHubClient,
            IHttpClientFactory httpClientFactory)
        {
            _compatibilityClient = compatibilityClient;
            _chatClient = chatClient;
            _gitHubClient = gitHubClient;
            _httpClientFactory = httpClientFactory;
        }

        public async Task RunAsync()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            AnsiConsole.Write(new Rule("[green]PC COMPATIBILITY CHECKER[/]"));
            Console.WriteLine();

            await LoadComponents();

            bool exitRequested = false;

            while (!exitRequested)
            {
                Console.WriteLine("\n=== MENU GŁÓWNE ===");
                Console.WriteLine("1. Sprawdź CPU + Płyta główna");
                Console.WriteLine("2. Sprawdź RAM + Płyta główna");
                Console.WriteLine("3. Sprawdź cały zestaw");
                Console.WriteLine("4. Pokaż dostępne części");
                Console.WriteLine("5. Zapytaj AI o radę");
                Console.WriteLine("6. GitHub API Demo");
                Console.WriteLine("7. Wyjdź");
                Console.Write("Twój wybór: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await CheckCpuMotherboard();
                        break;
                    case "2":
                        await CheckRamMotherboard();
                        break;
                    case "3":
                        await CheckFullCompatibility();
                        break;
                    case "4":
                        ShowAvailableParts();
                        break;
                    case "5":
                        await AskAiForAdvice();
                        break;
                    case "6":
                        await ShowGitHubApiDemo();
                        break;
                    case "7":
                        exitRequested = true;
                        Console.WriteLine("Do zobaczenia!");
                        break;
                    default:
                        Console.WriteLine("Nieprawidłowy wybór!");
                        break;
                }
            }
        }

        private async Task LoadComponents()
        {
            try
            {
                _components = await _compatibilityClient.GetComponentsAsync();
                Console.WriteLine($"Załadowano {_components.Count} komponentów");
            }
            catch
            {
                Console.WriteLine("Używam danych mockowanych");
            }
        }

        private async Task CheckCpuMotherboard()
        {
            Console.Clear();
            Console.WriteLine("=== SPRAWDZANIE CPU + PŁYTA GŁÓWNA ===\n");

            var cpus = _components.Where(c => c.Type == "CPU").ToList();
            var motherboards = _components.Where(c => c.Type == "Motherboard").ToList();

            if (!cpus.Any() || !motherboards.Any())
            {
                Console.WriteLine("Brak wymaganych komponentów!");
                WaitForKey();
                return;
            }

            Console.WriteLine("Wybierz procesor:");
            for (int i = 0; i < cpus.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {cpus[i].Name} (Socket: {cpus[i].Socket})");
            }
            Console.Write($"Twój wybór (1-{cpus.Count}): ");

            if (!int.TryParse(Console.ReadLine(), out int cpuIndex) || cpuIndex < 1 || cpuIndex > cpus.Count)
            {
                Console.WriteLine("Nieprawidłowy wybór!");
                WaitForKey();
                return;
            }
            var selectedCpu = cpus[cpuIndex - 1];

            Console.WriteLine("\nWybierz płytę główną:");
            for (int i = 0; i < motherboards.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {motherboards[i].Name} (Socket: {motherboards[i].Socket})");
            }
            Console.Write($"Twój wybór (1-{motherboards.Count}): ");

            if (!int.TryParse(Console.ReadLine(), out int mbIndex) || mbIndex < 1 || mbIndex > motherboards.Count)
            {
                Console.WriteLine("Nieprawidłowy wybór!");
                WaitForKey();
                return;
            }
            var selectedMotherboard = motherboards[mbIndex - 1];

            Console.WriteLine("\nSprawdzam kompatybilność...");
            var result = await _compatibilityClient.CheckCpuMotherboardAsync(selectedCpu.Id, selectedMotherboard.Id);

            Console.WriteLine($"\n=== WYNIK ===\n{result.Message}");

            if (result.Issues.Any())
            {
                Console.WriteLine("\nProblemy:");
                foreach (var issue in result.Issues)
                {
                    Console.WriteLine($"- {issue}");
                }
            }

            WaitForKey();
        }

        private async Task CheckRamMotherboard()
        {
            Console.Clear();
            Console.WriteLine("=== SPRAWDZANIE RAM + PŁYTA GŁÓWNA ===\n");

            var rams = _components.Where(c => c.Type == "RAM").ToList();
            var motherboards = _components.Where(c => c.Type == "Motherboard").ToList();

            if (!rams.Any() || !motherboards.Any())
            {
                Console.WriteLine("Brak wymaganych komponentów!");
                WaitForKey();
                return;
            }

            Console.WriteLine("Wybierz pamięć RAM:");
            for (int i = 0; i < rams.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {rams[i].Name} (Typ: {rams[i].MemoryType})");
            }
            Console.Write($"Twój wybór (1-{rams.Count}): ");

            if (!int.TryParse(Console.ReadLine(), out int ramIndex) || ramIndex < 1 || ramIndex > rams.Count)
            {
                Console.WriteLine("Nieprawidłowy wybór!");
                WaitForKey();
                return;
            }
            var selectedRam = rams[ramIndex - 1];

            Console.WriteLine("\nWybierz płytę główną:");
            for (int i = 0; i < motherboards.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {motherboards[i].Name} (RAM: {motherboards[i].MemoryType})");
            }
            Console.Write($"Twój wybór (1-{motherboards.Count}): ");

            if (!int.TryParse(Console.ReadLine(), out int mbIndex) || mbIndex < 1 || mbIndex > motherboards.Count)
            {
                Console.WriteLine("Nieprawidłowy wybór!");
                WaitForKey();
                return;
            }
            var selectedMotherboard = motherboards[mbIndex - 1];

            Console.WriteLine("\nSprawdzam kompatybilność...");
            var result = await _compatibilityClient.CheckRamMotherboardAsync(selectedRam.Id, selectedMotherboard.Id);

            Console.WriteLine($"\n=== WYNIK ===\n{result.Message}");

            WaitForKey();
        }

        private async Task CheckFullCompatibility()
        {
            Console.Clear();
            Console.WriteLine("=== SPRAWDZANIE CAŁEGO ZESTAWU ===\n");

            var buildRequest = new BuildRequest();

            Console.WriteLine("Wybierz części do sprawdzenia (wpisz numery oddzielone spacją):");
            Console.WriteLine("1. Procesor");
            Console.WriteLine("2. Płyta główna");
            Console.WriteLine("3. Pamięć RAM");
            Console.WriteLine("4. Karta graficzna");
            Console.Write("Twoje wybory: ");

            var choices = Console.ReadLine()?.Split(' ') ?? Array.Empty<string>();

            foreach (var choice in choices)
            {
                switch (choice)
                {
                    case "1":
                        var cpus = _components.Where(c => c.Type == "CPU").ToList();
                        if (cpus.Any())
                        {
                            Console.WriteLine("\nWybierz procesor:");
                            for (int i = 0; i < cpus.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {cpus[i].Name}");
                            }
                            Console.Write($"Twój wybór (1-{cpus.Count}): ");

                            if (int.TryParse(Console.ReadLine(), out int cpuIndex) && cpuIndex >= 1 && cpuIndex <= cpus.Count)
                            {
                                buildRequest.CpuId = cpus[cpuIndex - 1].Id;
                            }
                        }
                        break;

                    case "2":
                        var motherboards = _components.Where(c => c.Type == "Motherboard").ToList();
                        if (motherboards.Any())
                        {
                            Console.WriteLine("\nWybierz płytę główną:");
                            for (int i = 0; i < motherboards.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {motherboards[i].Name}");
                            }
                            Console.Write($"Twój wybór (1-{motherboards.Count}): ");

                            if (int.TryParse(Console.ReadLine(), out int mbIndex) && mbIndex >= 1 && mbIndex <= motherboards.Count)
                            {
                                buildRequest.MotherboardId = motherboards[mbIndex - 1].Id;
                            }
                        }
                        break;

                    case "3":
                        var rams = _components.Where(c => c.Type == "RAM").ToList();
                        if (rams.Any())
                        {
                            Console.WriteLine("\nWybierz pamięć RAM:");
                            for (int i = 0; i < rams.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {rams[i].Name}");
                            }
                            Console.Write($"Twój wybór (1-{rams.Count}): ");

                            if (int.TryParse(Console.ReadLine(), out int ramIndex) && ramIndex >= 1 && ramIndex <= rams.Count)
                            {
                                buildRequest.RamId = rams[ramIndex - 1].Id;
                            }
                        }
                        break;

                    case "4":
                        var gpus = _components.Where(c => c.Type == "GPU").ToList();
                        if (gpus.Any())
                        {
                            Console.WriteLine("\nWybierz kartę graficzną:");
                            for (int i = 0; i < gpus.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {gpus[i].Name}");
                            }
                            Console.Write($"Twój wybór (1-{gpus.Count}): ");

                            if (int.TryParse(Console.ReadLine(), out int gpuIndex) && gpuIndex >= 1 && gpuIndex <= gpus.Count)
                            {
                                buildRequest.GpuId = gpus[gpuIndex - 1].Id;
                            }
                        }
                        break;
                }
            }

            Console.WriteLine("\nSprawdzam zestaw...");
            var result = await _compatibilityClient.CheckFullBuildAsync(buildRequest);

            Console.WriteLine($"\n=== WYNIK ===\n{result.Message}");

            if (result.Issues.Any())
            {
                Console.WriteLine("\nProblemy:");
                foreach (var issue in result.Issues)
                {
                    Console.WriteLine($"- {issue}");
                }
            }

            WaitForKey();
        }

        private void ShowAvailableParts()
        {
            Console.Clear();
            Console.WriteLine("=== DOSTĘPNE CZĘŚCI ===\n");

            if (!_components.Any())
            {
                Console.WriteLine("Brak załadowanych części!");
                WaitForKey();
                return;
            }

            foreach (var component in _components)
            {
                Console.WriteLine($"{component.Type}: {component.Name}");
                Console.WriteLine($"  Producent: {component.Manufacturer}");

                if (!string.IsNullOrEmpty(component.Socket))
                    Console.WriteLine($"  Socket: {component.Socket}");
                if (!string.IsNullOrEmpty(component.MemoryType))
                    Console.WriteLine($"  Typ RAM: {component.MemoryType}");
                if (component.Cores > 0)
                    Console.WriteLine($"  Rdzenie: {component.Cores}");
                if (!string.IsNullOrEmpty(component.Vram))
                    Console.WriteLine($"  VRAM: {component.Vram}");

                Console.WriteLine();
            }

            Console.WriteLine($"Łącznie: {_components.Count} komponentów");
            WaitForKey();
        }

        private async Task AskAiForAdvice()
        {
            Console.Clear();
            Console.WriteLine("=== KONSULTACJA Z AI ===\n");

            Console.Write("Twoje pytanie o kompatybilność: ");
            string question = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(question))
            {
                Console.WriteLine("Pytanie nie może być puste!");
                WaitForKey();
                return;
            }

            Console.WriteLine("\nŁączę się z AI...");

            var requestJson = new
            {
                model = "llama3.2",
                prompt = $"Jesteś ekspertem od kompatybilności części komputerowych. {question} Odpowiedz krótko po polsku.",
                stream = false
            };

            Console.WriteLine("\n=== REQUEST JSON ===");
            Console.WriteLine(JsonSerializer.Serialize(requestJson, new JsonSerializerOptions { WriteIndented = true }));

            try
            {
                var answer = await _chatClient.GetAdviceAsync(question);

                Console.WriteLine("\n=== ODPOWIEDŹ AI ===");
                Console.WriteLine(answer);

                Console.WriteLine("\n=== RESPONSE JSON ===");
                var responseJson = new
                {
                    response = answer,
                    model = "llama3.2",
                    created_at = DateTime.UtcNow
                };
                Console.WriteLine(JsonSerializer.Serialize(responseJson, new JsonSerializerOptions { WriteIndented = true }));

                LogAiConversation(question, answer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}");
                Console.WriteLine("Upewnij się, że Ollama jest uruchomiona: ollama serve");
            }

            WaitForKey();
        }

        private async Task ShowGitHubApiDemo()
        {
            Console.Clear();
            Console.WriteLine("=== GITHUB API DEMO ===\n");

            Console.WriteLine("1. Wyszukaj użytkownika GitHub");
            Console.WriteLine("2. Pobierz repozytoria użytkownika");
            Console.WriteLine("3. Wyszukaj repozytoria");
            Console.WriteLine("4. Pobierz szczegóły repozytorium");
            Console.WriteLine("5. Powrót");
            Console.Write("Wybierz: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ShowGitHubUserInfo();
                    break;
                case "2":
                    await ShowUserRepositories();
                    break;
                case "3":
                    await SearchGitHubRepos();
                    break;
                case "4":
                    await ShowRepoDetails();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Nieprawidłowy wybór!");
                    WaitForKey();
                    break;
            }
        }

        private async Task ShowGitHubUserInfo()
        {
            Console.Clear();
            Console.WriteLine("=== WYSZUKAJ UŻYTKOWNIKA GITHUB ===\n");

            Console.Write("Podaj nazwę użytkownika GitHub: ");
            var username = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Nazwa użytkownika nie może być pusta!");
                WaitForKey();
                return;
            }

            Console.WriteLine($"\nPobieram dane użytkownika {username}...");
            var user = await _gitHubClient.GetUserAsync(username);

            if (user == null)
            {
                Console.WriteLine($"Nie znaleziono użytkownika '{username}'");
                WaitForKey();
                return;
            }

            Console.WriteLine($"\n=== INFORMACJE O UŻYTKOWNIKU ===");
            Console.WriteLine($"Nazwa: {user.Name}");
            Console.WriteLine($"Login: {user.Login}");
            if (!string.IsNullOrEmpty(user.Company))
                Console.WriteLine($"Firma: {user.Company}");
            if (!string.IsNullOrEmpty(user.Location))
                Console.WriteLine($"Lokalizacja: {user.Location}");
            if (!string.IsNullOrEmpty(user.Blog))
                Console.WriteLine($"Blog: {user.Blog}");
            if (!string.IsNullOrEmpty(user.Bio))
                Console.WriteLine($"Bio: {user.Bio}");

            Console.WriteLine($"\nStatystyki:");
            Console.WriteLine($"- Repozytoria: {user.PublicRepos}");
            Console.WriteLine($"- Obserwujący: {user.Followers}");
            Console.WriteLine($"- Obserwowani: {user.Following}");
            Console.WriteLine($"- Dołączył: {user.CreatedAt:dd.MM.yyyy}");

            WaitForKey();
        }

        private async Task ShowUserRepositories()
        {
            Console.Clear();
            Console.WriteLine("=== REPOZYTORIA UŻYTKOWNIKA ===\n");

            Console.Write("Podaj nazwę użytkownika GitHub: ");
            var username = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Nazwa użytkownika nie może być pusta!");
                WaitForKey();
                return;
            }

            Console.WriteLine($"\nPobieram repozytoria użytkownika {username}...");
            var repos = await _gitHubClient.GetUserReposAsync(username);

            if (repos == null || !repos.Any())
            {
                Console.WriteLine($"Brak repozytoriów dla użytkownika '{username}'");
                WaitForKey();
                return;
            }

            Console.WriteLine($"\n=== REPOZYTORIA ({repos.Count}) ===");

            foreach (var repo in repos.Take(10))
            {
                Console.WriteLine($"\n{repo.Name}");
                if (!string.IsNullOrEmpty(repo.Description))
                    Console.WriteLine($"  Opis: {repo.Description}");
                if (!string.IsNullOrEmpty(repo.Language))
                    Console.WriteLine($"  Język: {repo.Language}");
                Console.WriteLine($"  ⭐: {repo.Stars} | 🔄: {repo.Forks}");
                Console.WriteLine($"  URL: {repo.HtmlUrl}");
            }

            if (repos.Count > 10)
                Console.WriteLine($"\n... i {repos.Count - 10} więcej");

            WaitForKey();
        }

        private async Task SearchGitHubRepos()
        {
            Console.Clear();
            Console.WriteLine("=== WYSZUKAJ REPOZYTORIA ===\n");

            Console.Write("Wpisz frazę do wyszukania: ");
            var query = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(query))
            {
                Console.WriteLine("Fraza wyszukiwania nie może być pusta!");
                WaitForKey();
                return;
            }

            Console.WriteLine($"\nWyszukuję '{query}' na GitHub...");
            var repos = await _gitHubClient.SearchReposAsync(query, 5);

            if (repos == null || !repos.Any())
            {
                Console.WriteLine($"Nie znaleziono repozytoriów dla '{query}'");
                WaitForKey();
                return;
            }

            Console.WriteLine($"\n=== WYNIKI WYSZUKIWANIA ({repos.Count}) ===");

            foreach (var repo in repos)
            {
                Console.WriteLine($"\n{repo.FullName}");
                if (!string.IsNullOrEmpty(repo.Description))
                    Console.WriteLine($"  Opis: {repo.Description}");
                if (!string.IsNullOrEmpty(repo.Language))
                    Console.WriteLine($"  Język: {repo.Language}");
                Console.WriteLine($"  ⭐: {repo.Stars} | 🔄: {repo.Forks} | 🐛: {repo.OpenIssues}");
                Console.WriteLine($"  URL: {repo.HtmlUrl}");
                Console.WriteLine($"  Ostatnia aktualizacja: {repo.UpdatedAt:dd.MM.yyyy}");
            }

            WaitForKey();
        }

        private async Task ShowRepoDetails()
        {
            Console.Clear();
            Console.WriteLine("=== SZCZEGÓŁY REPOZYTORIUM ===\n");

            Console.Write("Podaj właściciela repozytorium: ");
            var owner = Console.ReadLine();

            Console.Write("Podaj nazwę repozytorium: ");
            var repoName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(owner) || string.IsNullOrWhiteSpace(repoName))
            {
                Console.WriteLine("Właściciel i nazwa repozytorium nie mogą być puste!");
                WaitForKey();
                return;
            }

            Console.WriteLine($"\nPobieram szczegóły {owner}/{repoName}...");
            var repo = await _gitHubClient.GetRepoAsync(owner, repoName);

            if (repo == null)
            {
                Console.WriteLine($"Nie znaleziono repozytorium '{owner}/{repoName}'");
                WaitForKey();
                return;
            }

            Console.WriteLine($"\n=== SZCZEGÓŁY REPOZYTORIUM ===");
            Console.WriteLine($"Nazwa: {repo.FullName}");
            if (!string.IsNullOrEmpty(repo.Description))
                Console.WriteLine($"Opis: {repo.Description}");
            if (!string.IsNullOrEmpty(repo.Language))
                Console.WriteLine($"Język: {repo.Language}");

            Console.WriteLine($"\nStatystyki:");
            Console.WriteLine($"- ⭐ Gwiazdki: {repo.Stars}");
            Console.WriteLine($"- 🔄 Forks: {repo.Forks}");
            Console.WriteLine($"- 🐛 Otwarte issue: {repo.OpenIssues}");

            Console.WriteLine($"\nURL: {repo.HtmlUrl}");
            Console.WriteLine($"Ostatnia aktualizacja: {repo.UpdatedAt:dd.MM.yyyy HH:mm}");

            WaitForKey();
        }

        private void WaitForKey()
        {
            Console.WriteLine("\nNaciśnij Enter, aby kontynuować...");
            Console.ReadLine();
        }

        private void LogAiConversation(string question, string answer)
        {
            try
            {
                string logPath = "ai_conversations.log";
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\nPytanie: {question}\nOdpowiedź: {answer}\n{new string('-', 50)}\n";
                File.AppendAllText(logPath, logEntry);
            }
            catch
            {
            }
        }
    }
}