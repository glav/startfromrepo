using System;
using System.CommandLine;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestCliProject.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void TestHelloCommand()
        {
            // Arrange
            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            Program.Main(new[] { "--name", "Tester" }).Wait();

            // Assert
            var outputString = output.ToString().Trim();
            Assert.Contains("Hello, Tester!", outputString);
            Assert.Contains("This is a sample .NET CLI tool", outputString);
        }
    }
}
