using System;
using System.Data.SqlClient;
using System.IO;

class Program
{
    private static string folderPath = @"C:\Users\M.Sobikanth\Docs\Documents\DotNet\NewQuikTravelUI\QuikTravel\Data Scripts\Release 003";
    private static string connectionString = "Server=localhost\\SQLEXPRESS;Database=QTAirLog;Integrated Security=True;Encrypt=False;"; // Update with your actual database connection string

    static void Main(string[] args)
    {
        // Initialize FileSystemWatcher
        FileSystemWatcher watcher = new FileSystemWatcher();
        watcher.Path = folderPath;
        watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
        watcher.Filter = "*.sql";
        watcher.Changed += OnChanged;
        watcher.Created += OnChanged;

        // Start monitoring
        watcher.EnableRaisingEvents = true;

        Console.WriteLine($"Monitoring folder: {folderPath}");
        Console.WriteLine("Press 'q' to quit.");

        RunAllScripts();

        // Keep the program running
        while (Console.Read() != 'q') ;
    }

    private static void OnChanged(object source, FileSystemEventArgs e)
    {
        Console.WriteLine($"File {e.ChangeType}: {e.FullPath}");

        // Run all scripts in order
        RunAllScripts();
    }

    private static void RunAllScripts()
    {
        var files = Directory.GetFiles(folderPath, "*.sql");

        foreach (var file in files)
        {
            try
            {
                Console.WriteLine($"Executing script: {file}");

                string script = File.ReadAllText(file);
                string[] batches = script.Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    foreach (var batch in batches)
                    {
                        using (SqlCommand command = new SqlCommand(batch, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Script {file} executed successfully.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                // Change text color to red for errors
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error executing script {file}: {ex.Message}");
                // Reset to default color
                Console.ResetColor();
            }
        }
    }
}