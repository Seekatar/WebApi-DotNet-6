class FutureWorker : IWorker
{
    private readonly ILogger<FutureWorker> _logger;
    private static readonly string[] _summaries = { "Freezing", "Bracing", "Chilly", "Cool" };


    public FutureWorker(ILogger<FutureWorker> logger)
    {
        _logger = logger;
    }

    public Task DoItAsync()
    {
        _logger.LogInformation("In DoIt!");
        return Task.CompletedTask;
    }

    public Task<IEnumerable<WeatherForecast>> GetForcasts()
    {
        _logger.LogInformation("In {type}", this.GetType().Name);
        return Task.FromResult(Enumerable.Range(1, 5).Select(index =>
           new WeatherForecast
           (
               DateTime.Now.AddDays(index),
               Random.Shared.Next(-20, 0),
               _summaries[Random.Shared.Next(_summaries.Length)],
               future:true
           )));
    }
}