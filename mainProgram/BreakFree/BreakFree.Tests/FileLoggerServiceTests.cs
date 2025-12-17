using BreakFree.BLL.Services;

public class FileLoggerServiceTests : IDisposable
{
    private readonly string tempDirectory;
    private readonly FileLoggerService logger;

    public FileLoggerServiceTests()
    {
        this.tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(this.tempDirectory);

        this.logger = new FileLoggerServiceForTest(this.tempDirectory);
    }

    [Fact]
    public void LogInfo_WritesInfoMessage()
    {
        this.logger.LogInfo("Test info");

        var logFile = Directory.GetFiles(this.tempDirectory).FirstOrDefault();
        Assert.NotNull(logFile);
        var content = File.ReadAllText(logFile!);
        Assert.Contains("[INFO] Test info", content);
    }

    [Fact]
    public void LogWarning_WritesWarningMessage()
    {
        this.logger.LogWarning("Test warning");

        var logFile = Directory.GetFiles(this.tempDirectory).FirstOrDefault();
        Assert.NotNull(logFile);
        var content = File.ReadAllText(logFile!);
        Assert.Contains("[WARNING] Test warning", content);
    }

    [Fact]
    public void LogError_WritesErrorMessageWithStackTrace()
    {
        this.logger.LogError("Test error", "Stack trace here");

        var logFile = Directory.GetFiles(this.tempDirectory).FirstOrDefault();
        Assert.NotNull(logFile);
        var content = File.ReadAllText(logFile!);
        Assert.Contains("[Error] Test error", content);
        Assert.Contains("Stack trace here", content);
    }

    public void Dispose()
    {
        if (Directory.Exists(this.tempDirectory))
        {
            Directory.Delete(this.tempDirectory, true);
        }
    }

    private class FileLoggerServiceForTest : FileLoggerService
    {
        public FileLoggerServiceForTest(string testDirectory)
        {
            var fileName = $"log_{DateTime.Now:yyyy-MM-dd}.txt";
            var filePath = Path.Combine(testDirectory, fileName);

            var field = typeof(FileLoggerService)
                .GetField("logFilePath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field!.SetValue(this, filePath);

            var dirField = typeof(FileLoggerService)
                .GetField("logDirectory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            dirField!.SetValue(this, testDirectory);
        }
    }
}
