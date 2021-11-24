using Microsoft.AspNetCore.Mvc;
using Seekatar.Interfaces;
using Seekatar.Tools;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IWeatherForcastService, WeatherForcastService>();
builder.Services.AddSingleton<IObjectFactory<IWorker>,ObjectFactory<IWorker>>();

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
