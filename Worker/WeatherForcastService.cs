class WeatherForcastService : IWeatherForcastService
{
    private readonly IWorkerFactory _factory;

    public WeatherForcastService(IWorkerFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<WeatherForecast>> GetForecasts(bool future)
    {
        var worker = _factory.GetWorker<IWorker>((future ? typeof(FutureWorker) : typeof(Worker)).Name);
        if (worker != null)
        {
            return await worker.GetForcasts().ConfigureAwait(false);
        }

        return new List<WeatherForecast>();
    }
}