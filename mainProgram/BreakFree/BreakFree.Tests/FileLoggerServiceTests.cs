namespace BreakFree.BLL.Services
{
    using BreakFree.BLL.Interfaces;

    public class FileLoggerService : ILoggerService
    {
        private readonly string logDirectory;
        private readonly string logFilePath;

        public FileLoggerService()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            this.logDirectory = Path.Combine(baseDirectory, "Logs");

            if (!Directory.Exists(this.logDirectory))
            {
                Directory.CreateDirectory(this.logDirectory);
            }

            string fileName = $"log_{DateTime.Now:yyyy-MM-dd}.txt";
            this.logFilePath = Path.Combine(this.logDirectory, fileName);
        }

        public void LogInfo(string message)
        {
            this.WriteToFile("INFO", message);
        }

        public void LogError(string message, string? stackTrace = "")
        {
            string fullMessage = message;
            if (!string.IsNullOrEmpty(stackTrace))
            {
                fullMessage += $"\nStack Trace: {stackTrace}";
            }

            this.WriteToFile("Error", fullMessage);
        }

        public void LogWarning(string message)
        {
            this.WriteToFile("WARNING", message);
        }

        private void WriteToFile(string level, string message)
        {
            try
            {
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";

                using (StreamWriter writer = new StreamWriter(this.logFilePath, true))
                {
                    writer.WriteLine(logEntry);
                }
            }
            catch
            {
            }
        }
    }
}
