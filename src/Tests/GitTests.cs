using System;
using System.CommandLine;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Diagnostics;

namespace StartFromRepo.Tests
{
  public class GitTests
  {
    [Fact]
    public void TestGitCommandExecution()
    {
      // This test verifies that git commands can be executed
      var result = ExecuteGitCommand("--version");

      // Verify git is available
      Assert.NotNull(result);
      Assert.Contains("git version", result);
    }

    private string ExecuteGitCommand(string arguments)
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
            UseShellExecute = false,
            CreateNoWindow = true,
          }
        };

        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return result;
      }
      catch (Exception ex)
      {
        return $"Error executing git command: {ex.Message}";
      }
    }
  }
}
