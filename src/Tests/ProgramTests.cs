using System;
using System.CommandLine;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Diagnostics;

namespace StartFromRepo.Tests
{
    public class ProgramTests
    {
        [Fact]
        public async Task TestCommandLineParameters()
        {
            // Arrange
            var output = new StringWriter();
            TextWriter originalOut = Console.Out;
            Console.SetOut(output);

            try
            {
                // Act
                await Program.Main(new[] {
                    "--username", "testuser",
                    "--source", "source-repo",
                    "--destination", "dest-repo"
                });

                // Assert
                var outputString = output.ToString().Trim();
                Assert.Contains("Username: testuser", outputString);
                Assert.Contains("Source repository: source-repo", outputString);
                Assert.Contains("Destination repository: dest-repo", outputString);
            }
            finally
            {
                // Reset console output
                Console.SetOut(originalOut);
            }
        }

        [Fact]
        public async Task TestCommandLineParametersWithShortNames()
        {
            // Arrange
            var output = new StringWriter();
            TextWriter originalOut = Console.Out;
            Console.SetOut(output);

            try
            {
                // Act
                await Program.Main(new[] {
                    "-u", "testuser",
                    "-s", "source-repo",
                    "-d", "dest-repo"
                });

                // Assert
                var outputString = output.ToString().Trim();
                Assert.Contains("Username: testuser", outputString);
                Assert.Contains("Source repository: source-repo", outputString);
                Assert.Contains("Destination repository: dest-repo", outputString);
            }
            finally
            {
                // Reset console output
                Console.SetOut(originalOut);
            }
        }
    }
}
