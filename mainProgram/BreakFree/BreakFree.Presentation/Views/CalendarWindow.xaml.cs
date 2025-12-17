namespace BreakFree.Presentation.Views
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;

    public partial class CalendarWindow : Window
    {
        private readonly int userId;
        private readonly DailyStatusService statusService;
        private readonly HabitService habitService;
        private readonly ILoggerService logger;

        private DateTime currentMonth;

        public CalendarWindow(int userId)
        {
            this.InitializeComponent();
            this.userId = userId;
            this.statusService = new DailyStatusService();
            this.habitService = new HabitService();

            this.logger = new FileLoggerService();
            this.logger.LogInfo($"Вікно 'Прогрес за місяць' було відкрито. UserID: {userId}.");

            this.currentMonth = DateTime.Today;
            this.LoadCalendar();

            this.Closing += this.CalendarWindow_Closing;
        }

        private void CalendarWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            this.logger.LogInfo($"Вікно 'Прогрес за місяць' було закрито.");

            if (this.Owner != null)
            {
                this.Owner.Show();
            }
        }

        private void PrevMonthLabel_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.PrevMonth_Click(sender, e);
        }

        private void NextMonthLabel_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.NextMonth_Click(sender, e);
        }

        private void PrevMonth_Click(object sender, RoutedEventArgs e)
        {
            this.currentMonth = this.currentMonth.AddMonths(-1);
            this.logger.LogInfo($"Було переглянуто попередній місяць: {this.currentMonth:MMMM yyyy}.");
            this.LoadCalendar();
        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            this.currentMonth = this.currentMonth.AddMonths(1);
            this.logger.LogInfo($"Було переглянуто наступний місяць: {this.currentMonth:MMMM yyyy}.");
            this.LoadCalendar();
        }

        private void LoadCalendar()
        {
            try
            {
                this.CalendarGrid.Children.Clear();

                this.MonthTitle.Text = this.currentMonth.ToString("MMMM yyyy");

                var statuses = this.statusService.GetStatusesByUser(this.userId);
                var habits = this.habitService.GetUserHabits(this.userId);

                DateTime? startDate = habits?.Min(h => h.StartDate).Date;

                int daysInMonth = DateTime.DaysInMonth(this.currentMonth.Year, this.currentMonth.Month);

                DateTime firstDay = new DateTime(this.currentMonth.Year, this.currentMonth.Month, 1);
                int startColumn = (int)firstDay.DayOfWeek;

                if (startColumn == 0)
                {
                    startColumn = 6;
                }
                else
                {
                    startColumn--;
                }

                int day = 1;
                int row = 0;
                int col = startColumn;

                while (day <= daysInMonth)
                {
                    DateTime cellDate = new DateTime(this.currentMonth.Year, this.currentMonth.Month, day);

                    var border = new Border
                    {
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(1),
                        Margin = new Thickness(1),
                        Background = Brushes.White,
                    };

                    if (startDate.HasValue && cellDate < startDate.Value)
                    {
                        border.Background = Brushes.White;
                    }
                    else if (cellDate > DateTime.Today)
                    {
                        border.Background = Brushes.White;
                    }
                    else
                    {
                        border.Background = Brushes.LightGreen;

                        bool relapse = statuses.Any(s => s.DateTime.Date == cellDate.Date);
                        if (relapse)
                        {
                            border.Background = Brushes.LightCoral;
                        }
                    }

                    var text = new TextBlock
                    {
                        Text = day.ToString(),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    };

                    border.Child = text;

                    this.CalendarGrid.Children.Add(border);
                    Grid.SetRow(border, row);
                    Grid.SetColumn(border, col);

                    day++;
                    col++;

                    if (col > 6)
                    {
                        col = 0;
                        row++;
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Критична помилка при завантажені календаря (LoadCalendar). UserID: {this.userId}.", ex.StackTrace);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Навігація: Прогрес за місяць -> Журнал зривів.");

            JournalWindow journalWindow = new JournalWindow(this.userId);

            journalWindow.Owner = this;

            this.Hide();

            journalWindow.Show();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}