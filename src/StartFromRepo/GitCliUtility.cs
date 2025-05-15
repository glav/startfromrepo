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
                return !output.Contains("fatal:") && !output.Contains("error:");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cloning repository: {ex.Message}");
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
