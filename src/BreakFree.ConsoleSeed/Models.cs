namespace BreakFree.ConsoleSeed
{
    public record User(int Id, string Name, string? Email, string Password, string CreatedAt);
    public record Habit(int Id, int UserId, string Name, string StartDate, int GoalDays, string? Motivation, bool IsActive, int TotalDays, int CurrentStreak, decimal TotalSaving);
    public record DailyStatus(int Id, int HabitId, string Date, bool IsClean, string Trigger, string? Note, int CravingLevel);
    public record Achievement(int Id, int UserId, string Title, string AchievedAt);
    public record SOSAction(int Id, string Text, string Category);
    public record UserSOSLog(int Id, int UserId, int ActionId, string Date, bool Worked);
    public record Quote(int Id, string Text);
    public record Saving(int Id, int HabitId, decimal Amount);
}
