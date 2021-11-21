namespace Temp1ate.Interfaces;

public interface IWorker
{
    Task DoItAsync();
    Task<IEnumerable<WeatherForecast>> GetForcasts();
}