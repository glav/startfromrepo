using System;
using System.CommandLine;
using System.Threading.Tasks;
using Octokit;

namespace StartFromRepo
{
  public class Program
  {
    public static async Task<int> Main(string[] args)
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

      rootCommand.SetHandler(async (username, source, destination) =>
      {
        Console.WriteLine($"Username: {username}");
        Console.WriteLine($"Source repository: {source}");
        Console.WriteLine($"Destination repository: {destination}");

        // GitHub authentication
        var client = await AuthenticateGitHubAsync(username);
        if (client != null)
        {
          Console.WriteLine("Successfully authenticated with GitHub!");
          // Here you would add repository access functionality
        }

      }, usernameOption, sourceOption, destinationOption);

      return await rootCommand.InvokeAsync(args);
    }

    private static async Task<GitHubClient> AuthenticateGitHubAsync(string username)
    {
      try
      {
        var client = new GitHubClient(new ProductHeaderValue("StartFromRepo"));

        Console.WriteLine($"Starting GitHub authentication for user: {username}");
        Console.WriteLine("Please enter your GitHub Personal Access Token:");

        // Hide the token input
        string token = "";
        ConsoleKeyInfo key;
        do
        {
          key = Console.ReadKey(true);

          if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
          {
            token += key.KeyChar;
            Console.Write("*");
          }
          else if (key.Key == ConsoleKey.Backspace && token.Length > 0)
          {
            token = token.Substring(0, token.Length - 1);
            Console.Write("\b \b");
          }
        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();

        if (string.IsNullOrEmpty(token))
        {
          Console.WriteLine("Authentication failed: No token provided.");
          return null;
        }

        client.Credentials = new Credentials(token);

        try
        {
          // Verify that the authentication worked
          var user = await client.User.Current();
          Console.WriteLine($"Successfully authenticated as {user.Login}");
          return client;
        }
        catch (AuthorizationException)
        {
          Console.WriteLine("Authentication failed: Invalid token.");
          return null;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Authentication error: {ex.Message}");
        return null;
      }
    }
  }
}
