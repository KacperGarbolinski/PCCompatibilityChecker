using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PCCompatibilityChecker.Clients;
using PCCompatibilityChecker.Services;

namespace PCCompatibilityChecker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            try
            {
                var host = HostBuilder.BuildHost();
                var app = host.Services.GetRequiredService<CompatibilityApp>();
                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd uruchamiania aplikacji: {ex.Message}");
                Console.WriteLine("Naciśnij Enter, aby zakończyć...");
                Console.ReadLine();
            }
        }
    }
}