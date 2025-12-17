using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;  // ← WAŻNE: ten using jest wymagany
using Spectre.Console;
using PCCompatibilityChecker.Clients;
using PCCompatibilityChecker.Services;
using PCCompatibilityChecker.Models;

// 1. SPEŁNIA: Używa Microsoft.Extensions.Hosting
var host = new HostBuilder()
    .ConfigureServices(services =>
    {
        // 2. SPEŁNIA: DI przez Microsoft.Extensions.Http
        services.AddHttpClient<ICompatibilityClient, CompatibilityClient>(client =>
        {
            client.BaseAddress = new Uri("https://6640d4dca7500fcf1a9f8e1c.mockapi.io/api/v1/");
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        services.AddSingleton<OllamaService>();
    })
    .Build();

// 3. SPEŁNIA: Pobieranie klienta z Host (DI)
var compatibilityClient = host.Services.GetRequiredService<ICompatibilityClient>();
var aiService = host.Services.GetRequiredService<OllamaService>();

// Interfejs CLI
Console.Clear();
AnsiConsole.Write(new FigletText("PC Checker").Color(Color.Green));
AnsiConsole.MarkupLine("[yellow]Sprawdź kompatybilność części komputerowych[/]\n");

bool running = true;

while (running)
{
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Wybierz akcję:")
            .PageSize(10)
            .AddChoices(new[]
            {
                "1. Sprawdź CPU + Płyta główna",
                "2. Sprawdź RAM + Płyta główna",
                "3. Sprawdź cały zestaw",
                "4. Pokaż dostępne części",
                "5. Zapytaj AI o radę",
                "6. Wyjdź"
            }));

    switch (choice)
    {
        case "1. Sprawdź CPU + Płyta główna":
            await CheckCpuMotherboard(compatibilityClient);
            break;

        case "2. Sprawdź RAM + Płyta główna":
            await CheckRamMotherboard(compatibilityClient);
            break;

        case "3. Sprawdź cały zestaw":
            await CheckFullBuild(compatibilityClient);
            break;

        case "4. Pokaż dostępne części":
            await ShowComponents(compatibilityClient);
            break;

        case "5. Zapytaj AI o radę":
            await AskForAiAdvice(aiService);
            break;

        case "6. Wyjdź":
            running = false;
            AnsiConsole.MarkupLine("[red]Do zobaczenia![/]");
            break;
    }

    if (running)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Naciśnij dowolny klawisz...[/]");
        Console.ReadKey();
        Console.Clear();
    }
}

// Metody pomocnicze
static async Task CheckCpuMotherboard(ICompatibilityClient client)
{
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[bold]Sprawdzanie kompatybilności CPU i płyty głównej[/]");

    var cpuId = AnsiConsole.Ask<string>("Podaj ID CPU (np. '1'):");
    var mbId = AnsiConsole.Ask<string>("Podaj ID płyty głównej (np. '3'):");

    var result = await client.CheckCpuMotherboardAsync(cpuId, mbId);

    AnsiConsole.WriteLine();
    DisplayResult(result);
}

static async Task CheckRamMotherboard(ICompatibilityClient client)
{
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[bold]Sprawdzanie kompatybilności RAM i płyty głównej[/]");

    var ramId = AnsiConsole.Ask<string>("Podaj ID RAM (np. '5'):");
    var mbId = AnsiConsole.Ask<string>("Podaj ID płyty głównej (np. '3'):");

    var result = await client.CheckRamMotherboardAsync(ramId, mbId);

    AnsiConsole.WriteLine();
    DisplayResult(result);
}

static async Task CheckFullBuild(ICompatibilityClient client)
{
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[bold]Sprawdzanie całego zestawu[/]");

    var build = new BuildRequest();

    if (AnsiConsole.Confirm("Czy chcesz sprawdzić CPU?"))
        build.CpuId = AnsiConsole.Ask<string>("Podaj ID CPU:");

    if (AnsiConsole.Confirm("Czy chcesz sprawdzić płytę główną?"))
        build.MotherboardId = AnsiConsole.Ask<string>("Podaj ID płyty:");

    if (AnsiConsole.Confirm("Czy chcesz sprawdzić RAM?"))
        build.RamId = AnsiConsole.Ask<string>("Podaj ID RAM:");

    var result = await client.CheckFullBuildAsync(build);

    AnsiConsole.WriteLine();
    DisplayResult(result);

    if (result.Issues.Count > 0)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[yellow]Problemy:[/]");
        foreach (var issue in result.Issues)
        {
            AnsiConsole.MarkupLine($"[red]• {issue}[/]");
        }
    }
}

static async Task ShowComponents(ICompatibilityClient client)
{
    var components = await client.GetComponentsAsync();

    var table = new Table();
    table.AddColumn("ID");
    table.AddColumn("Nazwa");
    table.AddColumn("Typ");
    table.AddColumn("Producent");

    foreach (var component in components)
    {
        table.AddRow(
            component.Id,
            component.Name,
            component.Type,
            component.Manufacturer
        );
    }

    AnsiConsole.Write(table);
    AnsiConsole.MarkupLine($"[grey]Łącznie: {components.Count} części[/]");
}

static async Task AskForAiAdvice(OllamaService aiService)
{
    AnsiConsole.WriteLine();
    var situation = AnsiConsole.Ask<string>("Opisz swój problem z kompatybilnością:");

    AnsiConsole.MarkupLine("[yellow]AI myśli...[/]");
    var advice = await aiService.GetCompatibilityAdviceAsync(situation);

    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[bold cyan]💡 Rada AI:[/]");
    AnsiConsole.WriteLine(new string('═', 50));
    Console.WriteLine(advice);
    AnsiConsole.WriteLine(new string('═', 50));
}

static void DisplayResult(CompatibilityResult result)
{
    if (result.IsCompatible)
    {
        AnsiConsole.MarkupLine("[bold green]✅ KOMPATYBILNE[/]");
    }
    else
    {
        AnsiConsole.MarkupLine("[bold red]❌ NIEKOMPATYBILNE[/]");
    }

    AnsiConsole.WriteLine(new string('─', 50));
    Console.WriteLine(result.Message);
    AnsiConsole.WriteLine(new string('─', 50));
}