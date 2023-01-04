namespace PlacidBits.Console.Core.Parameters;

public class CliArguments
{
    public string[] Args { get; set; }
    
    public CliArguments(string[] args)
    {
        Args = args;
    }
}