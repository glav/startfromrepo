using System;
using System.CommandLine;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StartFromRepo.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void TestCommandLineParameters()
        {
            // Arrange
            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            Program.Main(new[] {
                "--username", "testuser",
                "--source", "source-repo",
                "--destination", "dest-repo"
            }).Wait();

            // Assert
            var outputString = output.ToString().Trim();
            Assert.Contains("Username: testuser", outputString);
            Assert.Contains("Source repository: source-repo", outputString);
            Assert.Contains("Destination repository: dest-repo", outputString);
        }

        [Fact]
        public void TestCommandLineParametersWithShortNames()
        {
            // Arrange
            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            Program.Main(new[] {
                "-u", "testuser",
                "-s", "source-repo",
                "-d", "dest-repo"
            }).Wait();

            // Assert
            var outputString = output.ToString().Trim();
            Assert.Contains("Username: testuser", outputString);
            Assert.Contains("Source repository: source-repo", outputString);
            Assert.Contains("Destination repository: dest-repo", outputString);
        }
    }
}
