using System.IO.Pipelines;
using Microsoft.AspNetCore.Http.Features;

namespace FuzzingTests.SimpleServer;

public class ConsoleResponseBodyFeature: IHttpResponseBodyFeature
{
    public ConsoleResponseBodyFeature()
    {
        Stream = Console.OpenStandardOutput();
        Writer = PipeWriter.Create(Stream);
    }
    
    public Stream Stream { get; }
    public PipeWriter Writer { get; }

    public void DisableBuffering()
    {
        throw new NotImplementedException();
    }

    public Task StartAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }

    public Task SendFileAsync(string path, long offset, long? count,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }

    public Task CompleteAsync()
    {
        return Task.CompletedTask;
    }

}