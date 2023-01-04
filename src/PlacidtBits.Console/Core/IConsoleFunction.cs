namespace PlacidBits.Console.Core;

public interface IConsoleFunction
{
    Task RunAsync(CancellationToken cancellationToken);
}