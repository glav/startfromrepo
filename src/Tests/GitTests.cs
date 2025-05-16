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
    // Helper method that calls the async method but returns synchronously
    private string ExecuteGitCommand(string arguments)
    {
      return GitCliUtility.ExecuteGitCommandAsync(arguments).GetAwaiter().GetResult();
    }

    [Fact]
    public void TestGitCommandExecution()
    {
      // This test verifies that git commands can be executed
      var result = ExecuteGitCommand("--version");

      // Verify git is available
      Assert.NotNull(result);
      Assert.Contains("git version", result);
    }

    [Fact]
    public async Task TestChangeGitOrigin()
    {
      // Setup - create a temporary directory
      string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
      Directory.CreateDirectory(tempDir);

      try
      {
        // Initialize a temporary git repo to test with
        ExecuteGitCommand($"-C {tempDir} init");

        // Test our method to change the origin
        bool result = await GitCliUtility.ChangeGitOriginAsync("testuser", "newrepo", tempDir);

        // Verify the origin was changed
        string originUrl = ExecuteGitCommand($"-C {tempDir} remote get-url origin").Trim();

        Assert.True(result);
        Assert.Equal("https://github.com/testuser/newrepo.git", originUrl);
      }
      finally
      {
        // Clean up the test directory
        if (Directory.Exists(tempDir))
        {
          Directory.Delete(tempDir, true);
        }
      }
    }

    [Fact]
    public async Task TestBranchRenaming()
    {
      // Setup - create a temporary directory
      string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
      Directory.CreateDirectory(tempDir);

      try
      {
        // Initialize a temporary git repo to test with
        ExecuteGitCommand($"-C {tempDir} init");

        // Create a test file and commit it to establish the initial branch
        File.WriteAllText(Path.Combine(tempDir, "test.txt"), "Test content");
        ExecuteGitCommand($"-C {tempDir} add .");
        ExecuteGitCommand($"-C {tempDir} -c user.name=Test -c user.email=test@example.com commit -m \"Initial commit\"");

        // Save the initial branch name (usually master)
        string initialBranch = ExecuteGitCommand($"-C {tempDir} rev-parse --abbrev-ref HEAD").Trim();

        // Test our method to change the origin and rename branch
        bool result = await GitCliUtility.ChangeGitOriginAsync("testuser", "newrepo", tempDir);

        // Verify the branch was renamed to main
        string currentBranch = ExecuteGitCommand($"-C {tempDir} rev-parse --abbrev-ref HEAD").Trim();

        Assert.True(result);
        Assert.Equal("main", currentBranch);
      }
      finally
      {
        // Clean up the test directory
        if (Directory.Exists(tempDir))
        {
          Directory.Delete(tempDir, true);
        }
      }
    }
  }
}
