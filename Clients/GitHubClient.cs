using PCCompatibilityChecker.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace PCCompatibilityChecker.Clients
{
    public class GitHubClient : IGitHubClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public GitHubClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<GitHubUser?> GetUserAsync(string username)
        {
            try
            {
                var response = await _httpClient.GetAsync($"users/{username}");
                if (!response.IsSuccessStatusCode) return null;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GitHubUser>(json, _jsonOptions);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<GitHubRepo>?> GetUserReposAsync(string username)
        {
            try
            {
                var response = await _httpClient.GetAsync($"users/{username}/repos?sort=updated&per_page=10");
                if (!response.IsSuccessStatusCode) return null;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<GitHubRepo>>(json, _jsonOptions);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<GitHubRepo>?> SearchReposAsync(string query, int perPage = 10)
        {
            try
            {
                var encodedQuery = Uri.EscapeDataString(query);
                var response = await _httpClient.GetAsync($"search/repositories?q={encodedQuery}&sort=stars&order=desc&per_page={perPage}");
                if (!response.IsSuccessStatusCode) return null;
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("items", out var items))
                {
                    return JsonSerializer.Deserialize<List<GitHubRepo>>(items.ToString(), _jsonOptions);
                }
                return new List<GitHubRepo>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<GitHubRepo?> GetRepoAsync(string owner, string repoName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"repos/{owner}/{repoName}");
                if (!response.IsSuccessStatusCode) return null;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GitHubRepo>(json, _jsonOptions);
            }
            catch
            {
                return null;
            }
        }
    }
}