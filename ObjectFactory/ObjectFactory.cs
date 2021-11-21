using System.Reflection;
using Seekatar.Interfaces;

namespace Seekatar.Tools;

class ObjectFactory<T> : IObjectFactory<T> where T : class
{
    private readonly IServiceProvider _provider;
    private readonly Dictionary<string,Type> _workerTypes;

    public ObjectFactory(IServiceProvider provider)
    {
        _provider = provider;
        _workerTypes = LoadTypes();
    }

    protected virtual bool Predicate(Type type) => typeof(T).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract;

    protected virtual string ObjectName(Type type) => type.Name;

    private Dictionary<string,Type> LoadTypes()
    {
        return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(Predicate)
                .ToDictionary(ObjectName, type => type);
    }

    public T? GetInstance(string name)
    {
        if (!string.IsNullOrEmpty(name) && _workerTypes.ContainsKey(name))
        {
            var type = _workerTypes[name];
            return _provider.GetService(type) as T ?? ActivatorUtilities.GetServiceOrCreateInstance(_provider, type) as T;
        }
        return null;
    }
}