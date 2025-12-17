namespace BreakFree.BLL.Interfaces
{
    public interface ILoggerService
    {
        void LogInfo(string message);

        void LogError(string message, string? stackTrace = "");

        void LogWarning(string message);
    }
}
