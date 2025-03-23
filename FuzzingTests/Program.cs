using FuzzingTests;
using FuzzingTests.SimpleServer;
using SharpFuzz;

Fuzzer.OutOfProcess.Run((string str) =>
{
    var builder = WebApplication.CreateBuilder(args);

    builder.WebHost.UseServer(new ConsoleServer(str));

    // Add services to the container.
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();

    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    app.MapGet("/weatherforecast", async () =>
        {
            // Console.WriteLine("Weatherforecast Before Yield Thread {0}", Thread.CurrentThread.ManagedThreadId);
            // await Task.Yield(); 
            // Console.WriteLine("Weatherforecast After Yield Thread {0}", Thread.CurrentThread.ManagedThreadId);
            var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    (
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    ))
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast");
    
    // Console.WriteLine("Main Before RunApp Thread {0}", Thread.CurrentThread.ManagedThreadId);
    //Костыльно, ну что уж
    _ = app.RunAsync();
    // Console.WriteLine("Main After RunApp Thread {0}", Thread.CurrentThread.ManagedThreadId);

});

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}