using System.Reflection;

namespace Temp1ate.Worker;

class WorkerFactory : IWorkerFactory
{
    private readonly IServiceProvider _provider;
    private readonly Dictionary<string,Type> _workerTypes;

    public WorkerFactory(IServiceProvider provider)
    {
        _provider = provider;
        _workerTypes = LoadTypes<IWorker>();
    }

    private static Dictionary<string,Type> LoadTypes<T>() where T : class
    {
        return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => typeof(T).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .ToDictionary(x => x.Name, x => x);
    }

    public T? GetWorker<T>(string name) where T : class
    {
        if (!string.IsNullOrEmpty(name) && _workerTypes.ContainsKey(name))
        {
            var type = _workerTypes[name];
            return _provider.GetService(type) as T ?? ActivatorUtilities.GetServiceOrCreateInstance(_provider, type) as T;
        }
        return null;
    }
}