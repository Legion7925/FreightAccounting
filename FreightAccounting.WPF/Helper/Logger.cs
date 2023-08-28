using System.IO;
using System;

namespace FreightAccounting.WPF.Helper;

public static class Logger
{
    public static void LogException(Exception exception)
    {
        // Get the current directory
        string currentDirectory = Directory.GetCurrentDirectory();

        // Create a unique file name using the current date and time
        string fileName = $"ErrorLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

        // Combine the current directory and file name
        string filePath = Path.Combine(currentDirectory, fileName);

        try
        {
            // Create or append to the log file
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                // Write the exception details to the file
                writer.WriteLine($"Date/Time: {DateTime.Now}");
                writer.WriteLine($"Exception Type: {exception.GetType().FullName}");
                writer.WriteLine($"Message: {exception.Message}");
                writer.WriteLine($"Stack Trace: {exception.StackTrace}");
                writer.WriteLine(new string('-', 50));
                writer.WriteLine();
            }
        }
        catch
        {
            // Failed to log the exception (handle the exception here if necessary)
        }
    }
}
