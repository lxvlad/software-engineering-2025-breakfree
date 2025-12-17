namespace BreakFree.BLL.Services
{
    public class SosTipViewModel
    {
        public int ActionId { get; set; }

        public string? Text { get; set; }

        public int Efficiency { get; set; }

        public int UsageCount { get; set; }

        public string ToolTipText => this.UsageCount > 0 ? $"������������: {this.Efficiency}% ({this.UsageCount} �����)" : "�� �� �����������������";

        public string StatsText => this.UsageCount > 0 ? $"{this.Efficiency}%" : "New";

        public string StatsColor => this.Efficiency >= 50 ? "Green" : "Gray";
    }
}