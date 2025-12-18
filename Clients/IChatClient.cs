using System.Threading.Tasks;

namespace PCCompatibilityChecker.Clients
{
    public interface IChatClient
    {
        Task<string> GetAdviceAsync(string question);
    }
}