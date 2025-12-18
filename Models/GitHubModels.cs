using System;

namespace PCCompatibilityChecker.Models
{
    public class GitHubUser
    {
        public string Login { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Blog { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public int PublicRepos { get; set; }
        public int Followers { get; set; }
        public int Following { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GitHubRepo
    {
        public string Name { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string HtmlUrl { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public int Stars { get; set; }
        public int Forks { get; set; }
        public int OpenIssues { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}