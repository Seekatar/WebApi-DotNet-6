using Microsoft.AspNetCore.Mvc;
using Temp1ate.Worker;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IWeatherForcastService, WeatherForcastService>();
builder.Services.AddTransient<IWorkerFactory, WorkerFactory>();
if (args.Any(o => o == "-worker"))
{
    builder.Services.AddTransient<Worker>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", async ([FromQuery] bool? future, IWeatherForcastService service) =>
{
    return await service.GetForecasts(future ?? false);
})
.WithName("GetWeatherForecast");

app.Run();
