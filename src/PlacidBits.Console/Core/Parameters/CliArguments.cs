namespace PlacidBits.Console.Core.Parameters;

public class CliArguments(string[] args)
{
    public string[] Args { get; set; } = args;
}