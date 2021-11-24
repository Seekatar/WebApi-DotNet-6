using Seekatar.Interfaces;

class WeatherForcastService : IWeatherForcastService
{
    private readonly IObjectFactory<IWorker> _factory;
    private readonly ILogger<WeatherForcastService> _logger;

    public WeatherForcastService(IObjectFactory<IWorker> factory, ILogger<WeatherForcastService> logger)
    {
        _factory = factory;
        _logger = logger;
        _logger.LogInformation("Loaded workers:");
        foreach (var t in _factory.LoadedTypes)
        {
            _logger.LogInformation("    {name} => {type}", t.Key, t.Value.Name);
        }
    }

    public async Task<IEnumerable<WeatherForecast>> GetForecasts(bool future)
    {
        var worker = _factory.GetInstance((future ? typeof(FutureWorker) : typeof(Worker)).Name);
        if (worker != null)
        {
            return await worker.GetForcasts().ConfigureAwait(false);
        }

        return new List<WeatherForecast>();
    }
}