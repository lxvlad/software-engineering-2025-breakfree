namespace BreakFree.Presentation.Views
{
    using System;

    public class JournalItem
    {
        public int StatusId { get; set; }

        public int HabitId { get; set; }

        public int CravingLevel { get; set; }

        public DateTime FullDate { get; set; }

        public string Date { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;

        public string Note { get; set; } = string.Empty;
    }
}