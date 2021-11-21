namespace Seekatar.Interfaces;

public interface IObjectFactory<T> where T : class
{
    T? GetInstance(string name);
}