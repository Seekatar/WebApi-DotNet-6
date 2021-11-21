namespace Temp1ate.Model;

public record WeatherForecast(DateTime Date, int TemperatureC, string? Summary, bool future)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}