using System.Collections.Generic;

namespace PCCompatibilityChecker.Models;

public class Component
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Socket { get; set; } = string.Empty;
    public string MemoryType { get; set; } = string.Empty;
    public int Cores { get; set; }
    public int Threads { get; set; }
    public string Vram { get; set; } = string.Empty;
}

public class CompatibilityResult
{
    public bool IsCompatible { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Issues { get; set; } = new();
}

public class BuildRequest
{
    public string? CpuId { get; set; }
    public string? MotherboardId { get; set; }
    public string? RamId { get; set; }
    public string? GpuId { get; set; }
}