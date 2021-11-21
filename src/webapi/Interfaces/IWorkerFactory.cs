namespace Temp1ate.Interfaces;
public interface IWorkerFactory
{
    T? GetWorker<T>(string name) where T : class;
}