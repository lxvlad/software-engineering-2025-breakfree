namespace BreakFree.Presentation.Views
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;
    using BreakFree.DAL.Entities;

    public partial class StatisticsView : Window
    {
        private const int MaxCravingLevel = 10;
        private const int MaxDaysOnChart = 15;

        private readonly DailyStatusService dailyStatusService;
        private readonly HabitService habitService;
        private readonly ILoggerService logger;
        private readonly int userId;

        public StatisticsView()
            : this(0)
        {
        }

        public StatisticsView(int userId)
        {
            this.InitializeComponent();

            this.userId = userId;
            this.dailyStatusService = new DailyStatusService();
            this.habitService = new HabitService();

            this.logger = new FileLoggerService();
            this.logger.LogInfo($"Вікно 'Статистика' було відкрито. UserID: {userId}.");

            this.Loaded += this.StatisticsView_Loaded;
            this.Closing += this.StatisticsView_Closing;
        }

        public int TotalCleanDays { get; private set; }

        public int LongestStreakDays { get; private set; }

        public int TotalRelapses { get; private set; }

        public List<DailySummaryPoint> ChartData { get; private set; } = new List<DailySummaryPoint>();

        public List<TriggerStat> TopTriggers { get; private set; } = new List<TriggerStat>();

        public List<AchievementBadge> Achievements { get; private set; } = new List<AchievementBadge>();

        private void StatisticsView_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoadStatistics();
            this.DataContext = this;
            this.DrawChart();
        }

        private void StatisticsView_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            this.logger.LogInfo("Вікно 'Статистика' було закрито.");

            if (this.Owner != null)
            {
                this.Owner.Show();
            }
        }

        private void LoadStatistics()
        {
            try
            {
                var habits = this.habitService.GetUserHabits(this.userId);

                if (habits == null || habits.Count == 0)
                {
                    this.ResetStats();
                    return;
                }

                var statuses = this.dailyStatusService.GetStatusesByUser(this.userId) ?? new List<DailyStatus>();

                var groupedByDay = statuses
                    .GroupBy(s => s.DateTime.Date)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var firstDate = habits.Min(h => h.StartDate).Date;
                var lastDate = DateTime.Now.Date;

                if (firstDate > lastDate)
                {
                    firstDate = lastDate;
                }

                var daily = new List<DailyDayInfo>();

                for (var date = firstDate; date <= lastDate; date = date.AddDays(1))
                {
                    groupedByDay.TryGetValue(date, out var dayStatuses);

                    bool hasRelapse = false;
                    int maxCraving = 0;

                    if (dayStatuses != null && dayStatuses.Count > 0)
                    {
                        maxCraving = dayStatuses.Max(s => s.CravingLevel ?? 0);
                        hasRelapse = dayStatuses.Any(s => (s.CravingLevel ?? 0) > 0);
                    }

                    daily.Add(new DailyDayInfo
                    {
                        Date = date,
                        HasRelapse = hasRelapse,
                        MaxCraving = maxCraving,
                    });
                }

                this.TotalRelapses = daily.Count(d => d.HasRelapse);
                this.TotalCleanDays = daily.Count(d => !d.HasRelapse);

                int currentStreak = 0;
                int longest = 0;

                foreach (var day in daily)
                {
                    if (day.HasRelapse)
                    {
                        currentStreak = 0;
                    }
                    else
                    {
                        currentStreak++;
                        if (currentStreak > longest)
                        {
                            longest = currentStreak;
                        }
                    }
                }

                this.LongestStreakDays = longest;

                var dailyForChart = daily;
                if (dailyForChart.Count > MaxDaysOnChart)
                {
                    dailyForChart = dailyForChart.Skip(dailyForChart.Count - MaxDaysOnChart).ToList();
                }

                this.ChartData = dailyForChart
                    .Select(d => new DailySummaryPoint
                    {
                        Date = d.Date,
                        HasRelapse = d.HasRelapse,
                        MaxCraving = d.MaxCraving,
                    })
                    .ToList();

                var relapseStatuses = statuses
                    .Where(s => (s.CravingLevel ?? 0) > 0)
                    .Where(s => !string.IsNullOrWhiteSpace(s.Trigger));

                this.TopTriggers = relapseStatuses
                    .GroupBy(s => s.Trigger!.Trim())
                    .Select(g => new TriggerStat { Trigger = g.Key, Count = g.Count() })
                    .OrderByDescending(t => t.Count)
                    .Take(3)
                    .ToList();

                this.Achievements = this.CreateAchievements(currentStreak);
            }
            catch (Exception ex)
            {
                this.logger.LogError("Помилка при розрахунку статистики.", ex.StackTrace);
                this.ResetStats();
            }
        }

        private void ResetStats()
        {
            this.TotalCleanDays = 0;
            this.LongestStreakDays = 0;
            this.TotalRelapses = 0;
            this.ChartData = new List<DailySummaryPoint>();
            this.TopTriggers = new List<TriggerStat>();
            this.Achievements = this.CreateAchievements(0);
        }

        private List<AchievementBadge> CreateAchievements(int streak)
        {
            return new List<AchievementBadge>
            {
                new AchievementBadge { Title = "7 днів",   IsUnlocked = streak >= 7 },
                new AchievementBadge { Title = "1 місяць", IsUnlocked = streak >= 30 },
                new AchievementBadge { Title = "3 місяці", IsUnlocked = streak >= 90 },
            };
        }

        private void DrawChart()
        {
            if (this.ChartCanvas == null || this.ChartData == null || this.ChartData.Count == 0)
            {
                return;
            }

            this.ChartCanvas.Children.Clear();

            double width = this.ChartCanvas.ActualWidth;
            double height = this.ChartCanvas.ActualHeight;

            if (width <= 0)
            {
                width = 300;
            }

            if (height <= 0)
            {
                height = 200;
            }

            double leftMargin = 10;
            double rightMargin = 10;
            double topMargin = 10;
            double bottomMargin = 10;

            double plotWidth = width - leftMargin - rightMargin;
            double plotHeight = height - topMargin - bottomMargin;

            if (plotWidth <= 0 || plotHeight <= 0)
            {
                return;
            }

            double axisY = topMargin + plotHeight;
            var axis = new Line
            {
                X1 = leftMargin,
                Y1 = axisY,
                X2 = leftMargin + plotWidth,
                Y2 = axisY,
                Stroke = Brushes.Gray,
                StrokeThickness = 1,
            };
            this.ChartCanvas.Children.Add(axis);

            int n = this.ChartData.Count;
            double stepX = plotWidth / n;

            double barWidth = stepX * 0.6;
            if (barWidth < 4)
            {
                barWidth = 4;
            }

            if (barWidth > 30)
            {
                barWidth = 30;
            }

            for (int i = 0; i < n; i++)
            {
                var d = this.ChartData[i];
                double centerX = leftMargin + (stepX * i) + (stepX / 2.0);

                double heightFactor;
                if (d.HasRelapse)
                {
                    heightFactor = Math.Max(0.1, (double)d.MaxCraving / MaxCravingLevel);
                }
                else
                {
                    heightFactor = 1.0;
                }

                double barHeight = plotHeight * heightFactor * 0.95;
                double top = axisY - barHeight;

                var rect = new Rectangle
                {
                    Width = barWidth,
                    Fill = d.HasRelapse ? Brushes.Red : Brushes.Green,
                    RadiusX = 3,
                    RadiusY = 3,
                    ToolTip = $"{d.Date:dd.MM}: {(d.HasRelapse ? $"Зрив (тяга {d.MaxCraving})" : "Чисто")}",
                };

                Canvas.SetLeft(rect, centerX - (barWidth / 2.0));

                rect.Height = 0;
                Canvas.SetTop(rect, axisY);

                DoubleAnimation heightAnim = new DoubleAnimation
                {
                    From = 0,
                    To = barHeight,
                    Duration = TimeSpan.FromMilliseconds(800),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                };

                DoubleAnimation topAnim = new DoubleAnimation
                {
                    From = axisY,
                    To = top,
                    Duration = TimeSpan.FromMilliseconds(800),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                };

                rect.BeginAnimation(Rectangle.HeightProperty, heightAnim);
                rect.BeginAnimation(Canvas.TopProperty, topAnim);

                this.ChartCanvas.Children.Add(rect);
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.logger.LogInfo("Навігація: Статистика -> Експорт.");

                string filePath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "breakfree_stats.txt");

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Статистика користувача #{this.userId}");
                sb.AppendLine($"Дата формування: {DateTime.Now}");
                sb.AppendLine("--------------------------------");
                sb.AppendLine($"Чистих днів: {this.TotalCleanDays}");
                sb.AppendLine($"Найдовша серія: {this.LongestStreakDays}");
                sb.AppendLine($"Зривів: {this.TotalRelapses}");
                sb.AppendLine("--------------------------------");
                sb.AppendLine("Топ тригери:");
                foreach (var t in this.TopTriggers)
                {
                    sb.AppendLine($"- {t.Trigger}: {t.Count} разів");
                }

                System.IO.File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);

                this.logger.LogInfo($"Статистику було експортовано у файл: {filePath}.");

                MessageBox.Show($"Файл збережено:\n{filePath}", "Експорт успішний", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                this.logger.LogError("Помилка при експорті у файл.", ex.StackTrace);

                MessageBox.Show($"Помилка експорту: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.Show();
            }

            this.Close();
        }

        public class DailySummaryPoint
        {
            public DateTime Date { get; set; }

            public bool HasRelapse { get; set; }

            public int MaxCraving { get; set; }
        }

        public class TriggerStat
        {
            public string Trigger { get; set; } = string.Empty;

            public int Count { get; set; }
        }

        public class AchievementBadge
        {
            public string Title { get; set; } = string.Empty;

            public bool IsUnlocked { get; set; }
        }

        internal class DailyDayInfo
        {
            public DateTime Date { get; set; }

            public bool HasRelapse { get; set; }

            public int MaxCraving { get; set; }
        }
    }
}