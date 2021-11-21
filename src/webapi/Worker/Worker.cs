class Worker : IWorker
{
    private readonly ILogger<Worker> _logger;
    private static readonly string[] _summaries = { "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };


    public Worker(ILogger<Worker> logger)
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
               Random.Shared.Next(10, 55),
               _summaries[Random.Shared.Next(_summaries.Length)],
               future:false
           )));
    }
}