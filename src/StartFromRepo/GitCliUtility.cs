using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartFromRepo
{
    public class GitCliUtility
    {
        public static async Task<bool> VerifyGitCredentialsAsync()
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

        public static async Task<bool> CloneRepositoryAsync(string username, string source, string destination)
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
                bool cloneSuccess = !output.Contains("fatal:") && !output.Contains("error:");

                // If clone was successful, change the origin to the new repository
                if (cloneSuccess)
                {
                    bool originChangeSuccess = await ChangeGitOriginAsync(username, destination, destination);
                    if (!originChangeSuccess)
                    {
                        Console.WriteLine("Warning: Repository was cloned but failed to change the origin to the new repository.");
                    }
                }

                return cloneSuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cloning repository: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> ChangeGitOriginAsync(string username, string repositoryName, string workingDirectory)
        {
            try
            {
                // Format the new GitHub URL
                string newOriginUrl = $"https://github.com/{username}/{repositoryName}.git";

                // Execute git remote set-url command in the working directory
                string command = $"-C {workingDirectory} remote set-url origin {newOriginUrl}";

                // For a new repository that doesn't have a remote yet, first add it
                string checkRemoteOutput = await ExecuteGitCommandAsync($"-C {workingDirectory} remote");
                if (string.IsNullOrEmpty(checkRemoteOutput))
                {
                    await ExecuteGitCommandAsync($"-C {workingDirectory} remote add origin {newOriginUrl}");
                }
                else
                {
                    // Change the existing remote URL
                    string output = await ExecuteGitCommandAsync(command);

                    // Check if the command failed
                    if (output.Contains("fatal:") || output.Contains("error:"))
                    {
                        Console.WriteLine($"Error changing git origin: {output}");
                        return false;
                    }
                }

                // Ensure the main branch is named 'main'
                string currentBranchOutput = await ExecuteGitCommandAsync($"-C {workingDirectory} rev-parse --abbrev-ref HEAD");
                string currentBranch = currentBranchOutput.Trim();

                if (!string.IsNullOrEmpty(currentBranch) && currentBranch != "main")
                {
                    // Rename the branch to 'main'
                    string renameBranchOutput = await ExecuteGitCommandAsync($"-C {workingDirectory} branch -m {currentBranch} main");

                    if (renameBranchOutput.Contains("fatal:") || renameBranchOutput.Contains("error:"))
                    {
                        Console.WriteLine($"Warning: Could not rename branch to 'main': {renameBranchOutput}");
                    }
                    else
                    {
                        Console.WriteLine($"Default branch renamed from '{currentBranch}' to 'main'");
                    }
                }

                Console.WriteLine($"Git origin updated to: {newOriginUrl}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing git origin: {ex.Message}");
                return false;
            }
        }

        public static async Task<string> ExecuteGitCommandAsync(string arguments)
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

                var output = new StringBuilder();
                var error = new StringBuilder();

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
