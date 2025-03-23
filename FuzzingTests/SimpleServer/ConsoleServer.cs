using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;

namespace FuzzingTests.SimpleServer;

public partial class ConsoleServer(string input): IServer
{
    public IFeatureCollection Features { get; } = new FeatureCollection();

    [GeneratedRegex(
        @"(\S*)\s*(\S*)\s*(.*)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    )]
    private static partial Regex RequestRegex();
    [GeneratedRegex(
        @""
    )]
    private static partial Regex PathBaseRegex();
    
    
    public async Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken) where TContext : notnull
    {
        IHttpResponseBodyFeature responseBodyFeature = new ConsoleResponseBodyFeature();
        HttpResponseFeature responseFuture = new HttpResponseFeature();
        
        Features.Set<IHttpResponseFeature>(responseFuture);
        Features.Set<IHttpResponseBodyFeature>(responseBodyFeature);
        
        // Task serverLoop = Task.Factory.StartNew(async () =>
        // {
        //     while (true)
        //     {
                IHttpRequestFeature requestFuture =ParseRequest(input);
                
                var localFutures = new FeatureCollection(Features);
                localFutures.Set<IHttpRequestFeature>(requestFuture);

                TContext context = application.CreateContext(localFutures);
                
                Console.WriteLine("ConsoleServer Before ProcessRequest Thread {0}", Thread.CurrentThread.ManagedThreadId);
                await application.ProcessRequestAsync(context);
                Console.WriteLine("ConsoleServer After ProcessRequest Thread {0}", Thread.CurrentThread.ManagedThreadId);   
        //     }
        // });

        // _ = serverLoop.ContinueWith((task) =>
        // {
        //     if (task.IsFaulted)
        //         Console.WriteLine(task.Exception);
        // });
        
        return;
    }

    private IHttpRequestFeature WaitRequest()
    {
        string request = Console.ReadLine() ?? String.Empty;
        return ParseRequest(request);
    }

    private HttpRequestFeature ParseRequest(string request)
    {
        Regex regex = RequestRegex();
        HttpRequestFeature requestFuture = new HttpRequestFeature();

        Match requestMatch = regex.Match(request ?? string.Empty);
        if (!requestMatch.Success)
            return requestFuture;

        string method = requestMatch.Groups[1].Value;
        string path = requestMatch.Groups[2].Value;
        string body = requestMatch.Groups[3].Value;
        
        requestFuture.Method = method;
        requestFuture.Path = path;
        requestFuture.PathBase = path;
        
        if (!string.IsNullOrEmpty(body))
        {
            requestFuture.Headers.ContentLength = body.Length;
            byte[] byteArray = Encoding.UTF8.GetBytes(body);
            requestFuture.Body = new MemoryStream(byteArray);
        }

        return requestFuture;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
    }
}