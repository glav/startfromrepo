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
    const string PUSHOPTIONTEXT = "push";

    public static async Task<int> Main(string[] args)
    {
      var rootCommand = CreateCommandLineOptions();

      rootCommand.SetHandler(async (context) =>
      {
        var username = GetOption(rootCommand, context, USEROPTIONTEXT);
        var source = GetOption(rootCommand, context, SOURCEOPTIONTEXT);
        var destination = GetOption(rootCommand, context, DESTINATIONOPTIONTEXT);
        var pushOption = GetTypedOption<bool>(rootCommand, context, PUSHOPTIONTEXT);

        LoggingUtility.LogInfo($"Username: {username}");
        LoggingUtility.LogInfo($"Source repository: {source}");
        LoggingUtility.LogInfo($"Destination repository: {destination}");
        if (pushOption)
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
            if (pushOption)
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

      var pushOption = new Option<bool>(
          "--push",
          "Push code to GitHub repository after cloning");
      pushOption.AddAlias("-p");
      pushOption.SetDefaultValue(false);

      rootCommand.AddOption(usernameOption);
      rootCommand.AddOption(sourceOption);
      rootCommand.AddOption(destinationOption);
      rootCommand.AddOption(pushOption);

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

    static T GetTypedOption<T>(RootCommand rootCommand, InvocationContext context, string name)
    {
      var option = rootCommand.Options.FirstOrDefault(o => o.Name == name);
      if (option == null)
      {
        throw new ArgumentException($"Option '{name}' not found in the command.");
      }
      return context.ParseResult.GetValueForOption<T>((Option<T>)option);
    }
  }
}
