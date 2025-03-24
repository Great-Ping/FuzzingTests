using System.Buffers;
using System.Text;
using FuzzingTests;
using FuzzingTests.SimpleServer;
using Microsoft.AspNetCore.Mvc;
using SharpFuzz;
namespace FuzzingTests;

public class Program
{
    public static void Main(string[] args)
    {
        // string str = Console.ReadLine();
        Fuzzer.OutOfProcess.Run((string str) =>
        {
            string method = "POST /";
            if (Random.Shared.Next(2) == 0)
            {
                method = "GET /";
            }

            StartServer(args, method + str);
        });
    }

    public static void StartServer(string[] args, string str)
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


        app.Use(async (context, next) =>
        {
            // await Task.Yield();
            Console.WriteLine(context.Request.Method == "GET" ? "GET" : "Invalid Method");
            Console.WriteLine((context.Request.Path.Value?.Length ?? 0) % 2 == 0 ? "Test1" : "Test2");
            Console.WriteLine((context.Request.Method.Length) % 2 == 0 ? "Test3" : "Test4");

            if (Random.Shared.Next(10) == 2)
            {
                await Task.Yield();
                Console.WriteLine("Yield");
            }

            try
            {
                await next();
            }
            finally
            {
                await app.StopAsync();
            }
        });
        
        app.MapGet("/weatherforecast", () =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    (
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    ))
                .ToArray();
            return Task.FromResult(forecast);
        })
        .WithName("GetWeatherForecast");
        
        app.MapPost("/{temp}",  (HttpContext context, string temp) =>
        {
            StreamReader reader = new StreamReader(context.Request.Body);
            string text = reader.ReadToEnd();
            return Task.FromResult(text);
        })
        .WithName("ping");
        
        // Console.WriteLine("Main Before RunApp Thread {0}", Thread.CurrentThread.ManagedThreadId);
        try
        {
            app.Run();
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }

        // Console.WriteLine("Main After RunApp Thread {0}", Thread.CurrentThread.ManagedThreadId);
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}