using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StartFromRepo
{
    public class LoggingUtility
    {
        /// <summary>
        /// Log levels supported by the LoggingUtility
        /// </summary>
        public enum LogLevel
        {
            Info,
            Error,
            Debug
        }

        /// <summary>
        /// Logs an informational message to the console in white color
        /// </summary>
        /// <param name="message">The message to log</param>
        public static void LogInfo(string message)
        {
            Log(message, LogLevel.Info);
        }

        /// <summary>
        /// Logs an error message to the console in red color
        /// </summary>
        /// <param name="message">The error message to log</param>
        public static void LogError(string message)
        {
            Log(message, LogLevel.Error);
        }

        /// <summary>
        /// Logs a debug message to the console in yellow color
        /// </summary>
        /// <param name="message">The debug message to log</param>
        public static void LogDebug(string message)
        {
            Log(message, LogLevel.Debug);
        }

        /// <summary>
        /// Logs a message to the console with color based on the log level
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">The log level (Info, Error, or Debug)</param>
        public static void Log(string message, LogLevel level)
        {
            ConsoleColor originalColor = Console.ForegroundColor;

            switch (level)
            {
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"[INFO] {message}");
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[ERROR] {message}");
                    break;
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[DEBUG] {message}");
                    break;
            }

            Console.ForegroundColor = originalColor;
        }
    }
}
