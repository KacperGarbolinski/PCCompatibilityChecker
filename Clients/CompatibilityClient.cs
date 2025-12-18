using System.Text.Json;
using PCCompatibilityChecker.Models;

namespace PCCompatibilityChecker.Clients;

public class CompatibilityClient : ICompatibilityClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;

    public CompatibilityClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<Component>> GetComponentsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("components");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Component>>(json, _options) ?? GetMockComponents();
        }
        catch
        {
            return GetMockComponents();
        }
    }

    public async Task<CompatibilityResult> CheckCpuMotherboardAsync(string cpuId, string motherboardId)
    {
        var components = await GetComponentsAsync();
        var cpu = components.FirstOrDefault(c => c.Id == cpuId);
        var motherboard = components.FirstOrDefault(c => c.Id == motherboardId);

        if (cpu == null || motherboard == null)
        {
            return new CompatibilityResult
            {
                IsCompatible = false,
                Message = "Nie znaleziono komponentów",
                Issues = { "Brak danych o komponentach" }
            };
        }

        var result = new CompatibilityResult
        {
            IsCompatible = cpu.Socket == motherboard.Socket,
            Message = cpu.Socket == motherboard.Socket
                ? $"✅ CPU {cpu.Name} pasuje do płyty {motherboard.Name} (Socket: {cpu.Socket})"
                : $"❌ CPU {cpu.Name} NIE pasuje do płyty {motherboard.Name} (CPU: {cpu.Socket}, Płyta: {motherboard.Socket})"
        };

        if (!result.IsCompatible)
        {
            result.Issues.Add($"Niezgodność socketów: CPU={cpu.Socket}, Płyta={motherboard.Socket}");
        }

        return result;
    }

    public async Task<CompatibilityResult> CheckRamMotherboardAsync(string ramId, string motherboardId)
    {
        var components = await GetComponentsAsync();
        var ram = components.FirstOrDefault(c => c.Id == ramId);
        var motherboard = components.FirstOrDefault(c => c.Id == motherboardId);

        if (ram == null || motherboard == null)
        {
            return new CompatibilityResult
            {
                IsCompatible = false,
                Message = "Nie znaleziono komponentów"
            };
        }

        var result = new CompatibilityResult
        {
            IsCompatible = ram.MemoryType == motherboard.MemoryType,
            Message = ram.MemoryType == motherboard.MemoryType
                ? $"✅ RAM {ram.Name} pasuje do płyty {motherboard.Name} (Typ: {ram.MemoryType})"
                : $"❌ RAM {ram.Name} NIE pasuje do płyty {motherboard.Name} (RAM: {ram.MemoryType}, Płyta: {motherboard.MemoryType})"
        };

        return result;
    }

    public async Task<CompatibilityResult> CheckFullBuildAsync(BuildRequest build)
    {
        var issues = new List<string>();
        var messages = new List<string>();

        if (!string.IsNullOrEmpty(build.CpuId) && !string.IsNullOrEmpty(build.MotherboardId))
        {
            var cpuCheck = await CheckCpuMotherboardAsync(build.CpuId, build.MotherboardId);
            messages.Add(cpuCheck.Message);
            if (!cpuCheck.IsCompatible) issues.AddRange(cpuCheck.Issues);
        }

        if (!string.IsNullOrEmpty(build.RamId) && !string.IsNullOrEmpty(build.MotherboardId))
        {
            var ramCheck = await CheckRamMotherboardAsync(build.RamId, build.MotherboardId);
            messages.Add(ramCheck.Message);
            if (!ramCheck.IsCompatible) issues.Add("Problem z RAM");
        }

        return new CompatibilityResult
        {
            IsCompatible = issues.Count == 0,
            Message = string.Join("\n", messages),
            Issues = issues
        };
    }

    private List<Component> GetMockComponents()
    {
        return new List<Component>
        {
            new() {
                Id = "1",
                Name = "Intel Core i5-12400",
                Type = "CPU",
                Manufacturer = "Intel",
                Socket = "LGA1700",
                MemoryType = "",
                Cores = 6,
                Threads = 12,
                Vram = ""
            },
            new() {
                Id = "2",
                Name = "AMD Ryzen 5 5600X",
                Type = "CPU",
                Manufacturer = "AMD",
                Socket = "AM4",
                MemoryType = "",
                Cores = 6,
                Threads = 12,
                Vram = ""
            },
            
            new() {
                Id = "3",
                Name = "MSI B660M-A",
                Type = "Motherboard",
                Manufacturer = "MSI",
                Socket = "LGA1700",
                MemoryType = "DDR4",
                Cores = 0,
                Threads = 0,
                Vram = ""
            },
            new() {
                Id = "4",
                Name = "ASUS TUF B550-PLUS",
                Type = "Motherboard",
                Manufacturer = "ASUS",
                Socket = "AM4",
                MemoryType = "DDR4",
                Cores = 0,
                Threads = 0,
                Vram = ""
            },
            
            new() {
                Id = "5",
                Name = "Corsair Vengeance LPX 16GB DDR4",
                Type = "RAM",
                Manufacturer = "Corsair",
                Socket = "",
                MemoryType = "DDR4",
                Cores = 0,
                Threads = 0,
                Vram = ""
            },
            new() {
                Id = "6",
                Name = "Kingston Fury Beast 32GB DDR5",
                Type = "RAM",
                Manufacturer = "Kingston",
                Socket = "",
                MemoryType = "DDR5",
                Cores = 0,
                Threads = 0,
                Vram = ""
            },
            
            new() {
                Id = "7",
                Name = "NVIDIA RTX 4060",
                Type = "GPU",
                Manufacturer = "NVIDIA",
                Socket = "",
                MemoryType = "",
                Cores = 0,
                Threads = 0,
                Vram = "8GB"
            }
        };
    }
}