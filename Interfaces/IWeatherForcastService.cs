namespace Temp1ate.Interfaces;
internal interface IWeatherForcastService
{
    Task<IEnumerable<WeatherForecast>> GetForecasts(bool future);
}