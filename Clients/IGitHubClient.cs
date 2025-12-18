using PCCompatibilityChecker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PCCompatibilityChecker.Clients
{
    public interface IGitHubClient
    {
        Task<GitHubUser?> GetUserAsync(string username);
        Task<List<GitHubRepo>?> GetUserReposAsync(string username);
        Task<List<GitHubRepo>?> SearchReposAsync(string query, int perPage = 10);
        Task<GitHubRepo?> GetRepoAsync(string owner, string repoName);
    }
}