using System;
using System.CommandLine;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;
using System.Reflection.Metadata.Ecma335;
using System.CommandLine.Invocation;

namespace StartFromRepo
{
  public class Program
  {
    // The options available on the command line
    const string USEROPTIONTEXT = "username";
    const string SOURCEOPTIONTEXT = "source";
    const string DESTINATIONOPTIONTEXT = "destination";

    public static async Task<int> Main(string[] args)
    {
      var rootCommand = CreateCommandLineOptions();

      rootCommand.SetHandler(async (context) =>
      {
        var username = GetOption(rootCommand, context, USEROPTIONTEXT);
        var source = GetOption(rootCommand, context, SOURCEOPTIONTEXT);
        var destination = GetOption(rootCommand, context, DESTINATIONOPTIONTEXT);

        Console.WriteLine($"Username: {username}");
        Console.WriteLine($"Source repository: {source}");
        Console.WriteLine($"Destination repository: {destination}");

        // Check git credentials
        if (await GitCliUtility.VerifyGitCredentialsAsync())
        {
          Console.WriteLine("Successfully authenticated with GitHub using git credentials!");

          // Clone repository
          bool cloneSuccess = await GitCliUtility.CloneRepositoryAsync(username, source, destination);

          if (cloneSuccess)
          {
            Console.WriteLine($"Successfully accessed the repository: {source}");
            Console.WriteLine($"Cloned to: {destination}");
          }
          else
          {
            Console.WriteLine("Repository access failed. Please check your git credentials and repository permissions.");
          }
        }
        else
        {
          Console.WriteLine("Git authentication failed. Please set up git credentials with 'git config' or SSH keys.");
        }

      });

      return await rootCommand.InvokeAsync(args);
    }

    private static RootCommand CreateCommandLineOptions()
    {
      var rootCommand = new RootCommand("GitHub repository tool");
      var usernameOption = new Option<string>(
          "--username",
          "GitHub username");
      usernameOption.AddAlias("-u");
      usernameOption.IsRequired = true;

      var sourceOption = new Option<string>(
          "--source",
          "Source repository name");
      sourceOption.AddAlias("-s");
      sourceOption.IsRequired = true;

      var destinationOption = new Option<string>(
          "--destination",
          "Destination repository name");
      destinationOption.AddAlias("-d");
      destinationOption.IsRequired = true;

      rootCommand.AddOption(usernameOption);
      rootCommand.AddOption(sourceOption);
      rootCommand.AddOption(destinationOption);

      return rootCommand;
    }


    static string GetOption(RootCommand rootCommand, InvocationContext context, string name)
    {
      var option = rootCommand.Options.FirstOrDefault(o => o.Name == name);
      if (option == null)
      {
        throw new ArgumentException($"Option '{name}' not found in the command.");
      }
      var optionValue = context.ParseResult.GetValueForOption<string>((Option<string>)option);
      return optionValue ?? string.Empty;

    }
  }
}
