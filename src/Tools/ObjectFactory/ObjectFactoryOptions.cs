namespace Seekatar.Tools;

public class ObjectFactoryOptions
{
    /// <summary>
    /// Wild card mask of assemblies to load, that wouldn't automatically load
    /// </summary>
    /// <remarks>If empty, doesn't load anything extra</remarks>
    public string? AssemblyNameMask { get; set; }

    /// <summary>
    /// Regular expression of assembly names to scan.
    /// </summary>
    /// <remarks>If empty scans all assemblies</remarks>
    public string? AssemblyNameRegEx { get; set; }
}
