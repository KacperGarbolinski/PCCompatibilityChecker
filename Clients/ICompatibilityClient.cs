using PCCompatibilityChecker.Models;

namespace PCCompatibilityChecker.Clients;

public interface ICompatibilityClient
{

    Task<List<Component>> GetComponentsAsync();

    Task<CompatibilityResult> CheckCpuMotherboardAsync(string cpuId, string motherboardId);

    Task<CompatibilityResult> CheckRamMotherboardAsync(string ramId, string motherboardId);

    Task<CompatibilityResult> CheckFullBuildAsync(BuildRequest build);
}
