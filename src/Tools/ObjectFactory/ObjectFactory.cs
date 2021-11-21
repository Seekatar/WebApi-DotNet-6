using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Seekatar.Interfaces;

namespace Seekatar.Tools;

/// <summary>
/// Factory for loading types from assemblies then creating them on demand
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObjectFactory<T> : IObjectFactory<T> where T : class
{
    private readonly IServiceProvider _provider;
    private readonly Dictionary<string, Type> _objectTypes;
    private readonly Regex? _assemblyNameRegex;

    public ObjectFactory(IServiceProvider provider, IOptions<ObjectFactoryOptions> options)
    {
        _provider = provider;
        if (!string.IsNullOrWhiteSpace(options?.Value?.AssemblyNameMask))
        {
            var assemblies = LoadAssemblies(options.Value.AssemblyNameMask);
            foreach (var assembly in assemblies)
            {
                System.Diagnostics.Debug.WriteLine(assembly.FullName);
            }
        }
        _objectTypes = LoadTypes();
    }

    private static IList<Assembly> LoadAssemblies(string searchPattern, string folder = "")
    {
        if (string.IsNullOrWhiteSpace(folder))
        {
            folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException();
        }
        if (!searchPattern.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
        {
            searchPattern += ".dll";
        }
        return Directory.EnumerateFiles(folder, searchPattern, SearchOption.TopDirectoryOnly)
                        .Select(Assembly.LoadFrom)
                        .ToList();
    }

    /// <summary>
    /// Get a list of the loaded types.
    /// </summary>
    public IReadOnlyDictionary<string, Type> ObjectTypes => _objectTypes;

    /// <summary>
    /// Criteria for the classes to load. Defaults to classes that implement T
    /// </summary>
    /// <param name="type">A Type to check</param>
    /// <returns>true if this Type should be included</returns>
    protected virtual bool Predicate(Type type) => typeof(T).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract;

    /// <summary>
    /// The name of the class used to populate the dictionary then used by GetInstance. Defaults to the class Name
    /// </summary>
    /// <param name="type">The Type to get the name from </param>
    /// <returns>The name you want to identify the type</returns>
    protected virtual string ObjectName(Type type) => type.Name;

    private Dictionary<string, Type> LoadTypes()
    {
        IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();
        if (_assemblyNameRegex != null)
        {
            assemblies = assemblies.Where(o => _assemblyNameRegex.IsMatch(o.GetName().Name ?? ""));
        }
        return assemblies.SelectMany(s => s.GetTypes())
                         .Where(Predicate)
                         .ToDictionary(ObjectName, type => type);
    }

    /// <summary>
    /// Get the instance of an object type that this factory loaded
    /// </summary>
    /// <param name="name">name that matches with ObjectName</param>
    /// <returns>The instance or null</returns>
    public T? GetInstance(string name)
    {
        if (!string.IsNullOrEmpty(name) && _objectTypes.ContainsKey(name))
        {
            var type = _objectTypes[name];
            return _provider.GetService(type) as T ?? ActivatorUtilities.GetServiceOrCreateInstance(_provider, type) as T;
        }
        return null;
    }
}