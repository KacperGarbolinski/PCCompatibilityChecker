using PCCompatibilityChecker.Models;

namespace PCCompatibilityChecker.Clients;

public interface ICompatibilityClient
{
    // TYLKO 4 PROSTE METODY!

    // 1. Pobierz wszystkie komponenty
    Task<List<Component>> GetComponentsAsync();

    // 2. Sprawdü czy CPU pasuje do p≥yty
    Task<CompatibilityResult> CheckCpuMotherboardAsync(string cpuId, string motherboardId);

    // 3. Sprawdü czy RAM pasuje do p≥yty
    Task<CompatibilityResult> CheckRamMotherboardAsync(string ramId, string motherboardId);

    // 4. Sprawdü ca≥y zestaw
    Task<CompatibilityResult> CheckFullBuildAsync(BuildRequest build);
}
