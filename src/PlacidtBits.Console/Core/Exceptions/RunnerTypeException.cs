namespace PlacidBits.Console.Core.Exceptions;

public class RunnerTypeException : Exception
{
    public RunnerTypeException()
    {
    }

    public RunnerTypeException(string? message) : base(message)
    {
    }

    public RunnerTypeException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}