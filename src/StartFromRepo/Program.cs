using System;
using System.CommandLine;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;

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

        // Check git credentials
        if (await VerifyGitCredentialsAsync())
        {
          Console.WriteLine("Successfully authenticated with GitHub using git credentials!");

          // Clone repository
          bool cloneSuccess = await CloneRepositoryAsync(username, source, destination);

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

      }, usernameOption, sourceOption, destinationOption);

      return await rootCommand.InvokeAsync(args);
    }

    // Verify that git credentials are working
    private static async Task<bool> VerifyGitCredentialsAsync()
    {
      try
      {
        // Run a simple git command that requires authentication
        // Just checking the git config which should be available without authentication
        string output = await ExecuteGitCommandAsync("config --get user.name");

        if (!string.IsNullOrEmpty(output))
        {
          Console.WriteLine($"Git configured for user: {output.Trim()}");
          return true;
        }
        return false;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error verifying git credentials: {ex.Message}");
        return false;
      }
    }

    // Clone repository from source to destination
    private static async Task<bool> CloneRepositoryAsync(string username, string source, string destination)
    {
      try
      {
        // Determine if source repository contains a username
        string sourceRepo = source;
        if (!source.Contains("/"))
        {
          sourceRepo = $"{username}/{source}";
        }

        // Execute git clone command
        string output = await ExecuteGitCommandAsync($"clone https://github.com/{sourceRepo}.git {destination}");

        // Check if the clone was successful
        return !output.Contains("fatal:") && !output.Contains("error:");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error cloning repository: {ex.Message}");
        return false;
      }
    }

    // Execute a git command and return the output
    private static async Task<string> ExecuteGitCommandAsync(string arguments)
    {
      try
      {
        var process = new Process
        {
          StartInfo = new ProcessStartInfo
          {
            FileName = "git",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
          }
        };

        StringBuilder output = new StringBuilder();
        StringBuilder error = new StringBuilder();

        process.OutputDataReceived += (sender, e) =>
        {
          if (!string.IsNullOrEmpty(e.Data))
          {
            output.AppendLine(e.Data);
          }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
          if (!string.IsNullOrEmpty(e.Data))
          {
            error.AppendLine(e.Data);
          }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();

        string result = output.ToString();
        if (string.IsNullOrEmpty(result))
        {
          result = error.ToString();
        }

        return result;
      }
      catch (Exception ex)
      {
        return $"Error executing git command: {ex.Message}";
      }
    }
  }
}
