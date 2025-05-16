using Microsoft.VisualStudio.TestTools.UnitTesting;
using StartFromRepo;
using System;
using System.IO;

namespace StartFromRepo.Tests
{
  [TestClass]
  public class LoggingTests
  {
    private StringWriter _consoleOutput;
    private TextWriter _originalConsoleOutput;

    [TestInitialize]
    public void TestInitialize()
    {
      _originalConsoleOutput = Console.Out;
      _consoleOutput = new StringWriter();
      Console.SetOut(_consoleOutput);
    }

    [TestCleanup]
    public void TestCleanup()
    {
      Console.SetOut(_originalConsoleOutput);
      _consoleOutput.Dispose();
    }

    [TestMethod]
    public void LogInfo_ShouldContainInfoPrefix()
    {
      // Arrange & Act
      LoggingUtility.LogInfo("Test info message");

      // Assert
      string output = _consoleOutput.ToString();
      Assert.IsTrue(output.Contains("[INFO] Test info message"));
    }

    [TestMethod]
    public void LogError_ShouldContainErrorPrefix()
    {
      // Arrange & Act
      LoggingUtility.LogError("Test error message");

      // Assert
      string output = _consoleOutput.ToString();
      Assert.IsTrue(output.Contains("[ERROR] Test error message"));
    }

    [TestMethod]
    public void LogDebug_ShouldContainDebugPrefix()
    {
      // Arrange & Act
      LoggingUtility.LogDebug("Test debug message");

      // Assert
      string output = _consoleOutput.ToString();
      Assert.IsTrue(output.Contains("[DEBUG] Test debug message"));
    }
  }
}
