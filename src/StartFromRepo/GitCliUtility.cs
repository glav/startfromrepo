using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

                // Print debug output
                Console.WriteLine($"Debug - Clone output: {output}");

                // Check if destination directory exists to confirm clone success
                if (!Directory.Exists(destination))
                {
                    Console.WriteLine($"Debug - Destination directory {destination} does not exist");
                    cloneSuccess = false;
                }

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

        public static async Task<bool> CheckRepositoryExistsAsync(string username, string repositoryName)
        {
            try
            {
                // Use git ls-remote to check if the repository exists and is accessible
                string output = await ExecuteGitCommandAsync($"ls-remote https://github.com/{username}/{repositoryName}.git");

                // If the repository doesn't exist, the command will return an error message
                if (output.Contains("repository not found") ||
                    output.Contains("fatal:") ||
                    output.Contains("error:"))
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LoggingUtility.LogError($"Error checking repository existence: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> PushCodeToRepositoryAsync(string destinationDir)
        {
            try
            {
                // Make sure we're in the right directory
                if (!Directory.Exists(destinationDir))
                {
                    LoggingUtility.LogError($"Destination directory {destinationDir} does not exist");
                    return false;
                }

                // Execute git push command with -u flag to track the branch
                string output = await ExecuteGitCommandAsync($"-C {destinationDir} push -u origin main");

                // Check if the push was successful
                if (output.Contains("fatal:") || output.Contains("error:"))
                {
                    LoggingUtility.LogError($"Push failed: {output}");
                    return false;
                }

                LoggingUtility.LogInfo("Successfully pushed code to GitHub repository");
                return true;
            }
            catch (Exception ex)
            {
                LoggingUtility.LogError($"Error pushing code to repository: {ex.Message}");
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
