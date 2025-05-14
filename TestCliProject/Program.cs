using System;
using System.CommandLine;
using System.Threading.Tasks;

namespace TestCliProject
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("Sample .NET CLI tool");
            
            var nameOption = new Option<string>(
                "--name",
                "Name to greet");
            nameOption.AddAlias("-n");
            nameOption.IsRequired = true;
            
            rootCommand.AddOption(nameOption);
            
            rootCommand.SetHandler((name) => {
                Console.WriteLine($"Hello, {name}! This is a sample .NET CLI tool.");
            }, nameOption);
            
            return await rootCommand.InvokeAsync(args);
        }
    }
}
