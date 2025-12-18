using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PCCompatibilityChecker.Clients;
using PCCompatibilityChecker.Services;

namespace PCCompatibilityChecker
{
    public static class HostBuilder
    {
        public static IHost BuildHost()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddHttpClient<ICompatibilityClient, CompatibilityClient>(client =>
                    {
                        client.BaseAddress = new Uri("https://api.example.com/");
                        client.Timeout = TimeSpan.FromSeconds(30);
                        client.DefaultRequestHeaders.Add("User-Agent", "PCCompatibilityChecker");
                    });

                    services.AddHttpClient<IGitHubClient, GitHubClient>(client =>
                    {
                        client.BaseAddress = new Uri("https://api.github.com/");
                        client.Timeout = TimeSpan.FromSeconds(30);
                        client.DefaultRequestHeaders.Add("User-Agent", "PCCompatibilityChecker");
                        client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
                    });

                    services.AddHttpClient<IChatClient, OllamaService>(client =>
                    {
                        client.BaseAddress = new Uri("http://localhost:11434/");
                        client.Timeout = TimeSpan.FromSeconds(30);
                    });

                    services.AddSingleton<CompatibilityApp>();
                })
                .Build();
        }
    }
}