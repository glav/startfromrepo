using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;

namespace StartFromRepo
{
  public class Program
  {
    // The options available on the command line
    public static async Task<int> Main(string[] args)
    {
      var usernameOption = new Option<string>("--username", "-u")
      {
        Description = "GitHub username",
        Required = true
      };

      var sourceOption = new Option<string>("--source", "-s")
      {
        Description = "Source repository name",
        Required = true
      };

      var destinationOption = new Option<string>("--destination", "-d")
      {
        Description = "Destination repository name",
        Required = true
      };

      var pushOption = new Option<bool>("--push", "-p")
      {
        Description = "Push code to GitHub repository after cloning",
        DefaultValueFactory = _ => false
      };

      var rootCommand = new RootCommand("GitHub repository tool")
      {
        usernameOption,
        sourceOption,
        destinationOption,
        pushOption
      };

      rootCommand.SetAction(async (parseResult) =>
      {
        var username = parseResult.GetValue(usernameOption)!;
        var source = parseResult.GetValue(sourceOption)!;
        var destination = parseResult.GetValue(destinationOption)!;
        var push = parseResult.GetValue(pushOption);

        LoggingUtility.LogInfo($"Username: {username}");
        LoggingUtility.LogInfo($"Source repository: {source}");
        LoggingUtility.LogInfo($"Destination repository: {destination}");
        if (push)
        {
          LoggingUtility.LogInfo("Push flag is enabled: Will attempt to push to GitHub after cloning");
        }

        // Demonstration of the LoggingUtility for testing
        LoggingUtility.LogDebug("Starting repository operations...");

        // Check git credentials
        if (await GitCliUtility.VerifyGitCredentialsAsync())
        {
          LoggingUtility.LogInfo("Successfully authenticated with GitHub using git credentials!");

          // Clone repository
          bool cloneSuccess = await GitCliUtility.CloneRepositoryAsync(username, source, destination);

          if (cloneSuccess)
          {
            LoggingUtility.LogInfo($"Successfully accessed the repository: {source}");
            LoggingUtility.LogInfo($"Cloned to: {destination}");
            LoggingUtility.LogInfo($"Remote origin set to: https://github.com/{username}/{destination}.git");
            LoggingUtility.LogInfo($"Default branch renamed to 'main'");

            // Handle push flag if enabled
            if (push)
            {
              LoggingUtility.LogInfo("Checking if destination repository exists on GitHub...");
              bool repoExists = await GitCliUtility.CheckRepositoryExistsAsync(username, destination);

              if (repoExists)
              {
                LoggingUtility.LogInfo("Repository exists, attempting to push code...");
                bool pushSuccess = await GitCliUtility.PushCodeToRepositoryAsync(destination);

                if (pushSuccess)
                {
                  LoggingUtility.LogInfo($"Successfully pushed code to https://github.com/{username}/{destination}");
                }
                else
                {
                  LoggingUtility.LogError("Failed to push code. Check console output for details.");
                }
              }
              else
              {
                LoggingUtility.LogError($"Repository {username}/{destination} does not exist on GitHub.");
                LoggingUtility.LogInfo($"You must first create an empty repository at https://github.com/{username}/{destination}");
                LoggingUtility.LogInfo("Then you can push manually with:");
                LoggingUtility.LogInfo($"  cd {destination}");
                LoggingUtility.LogInfo($"  git push -u origin main");
              }
            }
            else
            {
              LoggingUtility.LogInfo($"You can now push changes to your new repository with:");
              LoggingUtility.LogInfo($"  cd {destination}");
              LoggingUtility.LogInfo($"  git push -u origin main");
            }
          }
          else
          {
            LoggingUtility.LogError("Repository access failed. Please check your git credentials and repository permissions.");
          }
        }
        else
        {
          LoggingUtility.LogError("Git authentication failed. Please set up git credentials with 'git config' or SSH keys.");
        }

      });

      return await rootCommand.Parse(args).InvokeAsync();
    }
  }
}
