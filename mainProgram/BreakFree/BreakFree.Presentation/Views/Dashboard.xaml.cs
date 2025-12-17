namespace BreakFree.Presentation.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Threading;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;
    using BreakFree.DAL.Entities;

    public partial class Dashboard : Window
    {
        private static bool hasPlayedEntryAnimation = false;

        private readonly int userId;
        private readonly HabitService habitService;
        private readonly DailyStatusService statusService;
        private readonly ILoggerService logger;
        private List<Habit> habits = new List<Habit>();
        private int currentHabitIndex = 0;
        private bool isNavigation = false;

        public Dashboard(int userId)
        {
            this.InitializeComponent();
            this.userId = userId;

            this.habitService = new HabitService();
            this.statusService = new DailyStatusService();

            this.logger = new FileLoggerService();
            this.logger.LogInfo($"Було здійснено вхід на вікно 'Прогрес'. UserID: {userId}");

            this.LoadHabits();
            this.CalculateStats();

            this.Closing += this.Dashboard_Closing;
        }

        public void RefreshData()
        {
            this.logger.LogInfo("Було здійснено оновлення даних у вікні 'Прогрес'.");
            this.LoadHabits();
            this.CalculateStats();
        }

        private void CalculateStats()
        {
            try
            {
                var userHabits = this.habitService.GetUserHabits(this.userId);

                if (userHabits == null || userHabits.Count == 0)
                {
                    this.TotalDaysText.Text = "0";
                    this.StreakDaysText.Text = "0";
                    return;
                }

                var firstStartDate = userHabits.Min(h => h.StartDate);
                int totalDays = (DateTime.Now.Date - firstStartDate.Date).Days;

                if (totalDays < 0)
                {
                    totalDays = 0;
                }

                var allStatuses = this.statusService.GetStatusesByUser(this.userId);

                var lastRelapse = allStatuses
                    .Where(s => (s.CravingLevel ?? 0) > 0 && s.DateTime <= DateTime.Now)
                    .OrderByDescending(s => s.DateTime)
                    .FirstOrDefault();

                int streakDays;
                if (lastRelapse == null)
                {
                    streakDays = totalDays;
                }
                else
                {
                    streakDays = (DateTime.Now.Date - lastRelapse.DateTime.Date).Days;
                }

                if (streakDays < 0)
                {
                    streakDays = 0;
                }

                if (!hasPlayedEntryAnimation)
                {
                    this.AnimateTextCounter(this.TotalDaysText, totalDays);
                    this.AnimateTextCounter(this.StreakDaysText, streakDays);

                    hasPlayedEntryAnimation = true;
                }
                else
                {
                    this.TotalDaysText.Text = totalDays.ToString();
                    this.StreakDaysText.Text = streakDays.ToString();
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Помилка при розрахунку статистики. UserID: {this.userId}.", ex.StackTrace);
            }
        }

        private void AnimateTextCounter(TextBlock targetBlock, int toValue)
        {
            if (toValue == 0)
            {
                targetBlock.Text = "0";
                return;
            }

            int current = 0;
            int step = toValue / 20;
            if (step < 1)
            {
                step = 1;
            }

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(30);

            timer.Tick += (s, e) =>
            {
                current += step;

                if (current >= toValue)
                {
                    current = toValue;
                    timer.Stop();
                }

                targetBlock.Text = current.ToString();
            };

            timer.Start();
        }

        private void Dashboard_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.isNavigation)
            {
                return;
            }

            ConfirmExitView confirmExit = new ConfirmExitView();
            bool? result = confirmExit.ShowDialog();

            if (result == true)
            {
                this.logger.LogInfo("Завершення роботи програми.");
                Application.Current.Shutdown();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void LoadHabits()
        {
            try
            {
                this.habits = this.habitService.GetUserHabits(this.userId);

                if (this.habits.Count == 0)
                {
                    this.HabitNameText.Text = "Немає звичок";
                    this.StartDateText.Text = "-";
                    this.StatusText.Text = "-";
                    this.MotivationText.Text = "—";
                    this.MotivationText.Foreground = Brushes.Gray;
                    this.MoneySavedText.Text = "-";

                    this.HabitNavigationPanel.Visibility = Visibility.Visible;

                    return;
                }

                if (this.currentHabitIndex >= this.habits.Count)
                {
                    this.currentHabitIndex = 0;
                }

                this.DisplayHabit(this.currentHabitIndex);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Помилка при завантажені звичок. UserID: {this.userId}.", ex.StackTrace);
                MessageBox.Show("Не вдалося завантажити звички.");
            }
        }

        private void DisplayHabit(int index)
        {
            if (this.habits == null || this.habits.Count == 0)
            {
                return;
            }

            var h = this.habits[index];

            this.HabitNameText.Text = h.HabitName;
            this.StartDateText.Text = h.StartDate.ToString("dd.MM.yyyy");

            if (string.IsNullOrWhiteSpace(h.Motivation))
            {
                this.MotivationText.Text = "Додайте мотивацію";
                this.MotivationText.FontStyle = FontStyles.Italic;
                this.MotivationText.Foreground = Brushes.Gray;
            }
            else
            {
                this.MotivationText.Text = h.Motivation;
                this.MotivationText.FontStyle = FontStyles.Normal;
                this.MotivationText.Foreground = Brushes.Black;
            }

            this.StatusText.Text = h.IsActive ? "✔ Активна" : "✖ Не активна";

            int daysSinceStart = (DateTime.Now.Date - h.StartDate.Date).Days;
            if (daysSinceStart < 0)
            {
                daysSinceStart = 0;
            }

            var habitStatuses = this.statusService.GetStatusesByHabit(h.HabitId);

            int relapseDaysCount = 0;
            if (habitStatuses != null)
            {
                relapseDaysCount = habitStatuses
                    .Where(s => (s.CravingLevel ?? 0) > 0 &&
                                s.DateTime.Date >= h.StartDate.Date &&
                                s.DateTime.Date <= DateTime.Now.Date)
                    .Select(s => s.DateTime.Date)
                    .Distinct()
                    .Count();
            }

            int cleanDays = daysSinceStart - relapseDaysCount;
            if (cleanDays < 0)
            {
                cleanDays = 0;
            }

            decimal moneySaved = cleanDays * h.DailyGoal;

            this.MoneySavedText.Text = $"{moneySaved} ₴ (ціль: {h.DailyGoal} ₴/день)";

            if (h.IsActive)
            {
                this.ToggleHabitStatusMenuItem.Header = "✔ Позначити як подолану";
            }
            else
            {
                this.ToggleHabitStatusMenuItem.Header = "↩️ Повернути звичку";
            }
        }

        private void EditMotivation_Click(object sender, RoutedEventArgs e)
        {
            if (this.habits == null || this.habits.Count == 0)
            {
                this.logger.LogWarning($"Спроба редагувати мотивацію без створених звичок. UserID: {this.userId}.");
                MessageBox.Show("Спочатку створіть звичку!");
                return;
            }

            var currentHabit = this.habits[this.currentHabitIndex];
            var editWindow = new EditMotivationWindow(currentHabit.Motivation);

            if (editWindow.ShowDialog() == true)
            {
                this.logger.LogInfo($"Було змінено мотивацію. HabitID: {currentHabit.HabitId}.");
                currentHabit.Motivation = editWindow.Motivation;
                this.habitService.UpdateHabit(currentHabit);
                this.DisplayHabit(this.currentHabitIndex);
            }
        }

        private void EditGoal_Click(object sender, RoutedEventArgs e)
        {
            if (this.habits == null || this.habits.Count == 0)
            {
                this.logger.LogWarning($"Спроба редагувати ціль без створених звичок. UserID: {this.userId}.");
                MessageBox.Show("Спочатку створіть звичку!");
                return;
            }

            var currentHabit = this.habits[this.currentHabitIndex];
            var editWindow = new EditDailyGoalWindow(currentHabit.DailyGoal);

            if (editWindow.ShowDialog() == true)
            {
                this.logger.LogInfo($"Було змінено ціль. HabitID: {currentHabit.HabitId}.");
                currentHabit.DailyGoal = editWindow.NewGoal;
                this.habitService.UpdateHabit(currentHabit);
                this.DisplayHabit(this.currentHabitIndex);
            }
        }

        private void DeleteHabit_Click(object sender, RoutedEventArgs e)
        {
            if (this.habits == null || this.habits.Count == 0)
            {
                this.logger.LogWarning($"Спроба видалення при порожньому списку звичок. UserID: {this.userId}.");
                MessageBox.Show("Немає звичок для видалення!");
                return;
            }

            var habit = this.habits[this.currentHabitIndex];

            var result = MessageBox.Show(
                $"Видалити звичку \"{habit.HabitName}\"?",
                "Підтвердження",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    this.habitService.DeleteHabit(habit.HabitId);

                    this.logger.LogWarning($"Звичку {habit.HabitName} було видалено. HabitID: {habit.HabitId}.");

                    this.LoadHabits();
                    this.CalculateStats();

                    MessageBox.Show("Звичку видалено.");
                }
                catch (Exception ex)
                {
                    this.logger.LogError($"Помилка при видаленні звички. HabitID: {habit.HabitId}", ex.StackTrace);
                }
            }
        }

        private void RenameHabit_Click(object sender, RoutedEventArgs e)
        {
            if (this.habits == null || this.habits.Count == 0)
            {
                this.logger.LogWarning($"Спроба перейменування без створених звичок. UserID: {this.userId}.");
                MessageBox.Show("Спочатку створіть звичку!");
                return;
            }

            var habit = this.habits[this.currentHabitIndex];

            var renameWindow = new RenameHabitWindow(habit);

            if (renameWindow.ShowDialog() == true)
            {
                habit.HabitName = renameWindow.NewName;
                habit.IsActive = renameWindow.IsActive;

                this.habitService.UpdateHabit(habit);
                this.logger.LogInfo($"Було перейменовано звичку на {habit.HabitName}. HabitID: {habit.HabitId}.");

                this.DisplayHabit(this.currentHabitIndex);

                MessageBox.Show("Зміни збережено!");
            }
        }

        private void ToggleHabitStatus_Click(object sender, RoutedEventArgs e)
        {
            var habit = this.habits[this.currentHabitIndex];

            if (habit.IsActive)
            {
                habit.IsActive = false;
                this.habitService.UpdateHabit(habit);

                this.logger.LogInfo($"Звичку {habit.HabitName} було позначено як 'Виконано/Подолано'. HabitID: {habit.HabitId}.");

                this.ToggleHabitStatusMenuItem.Header = "↩️ Повернути звичку";
                this.StatusText.Text = "✖ Не активна";

                MessageBox.Show("Звичку позначено як подолану 🎉");
            }
            else
            {
                habit.IsActive = true;
                this.habitService.UpdateHabit(habit);

                this.logger.LogInfo($"Звичку {habit.HabitName} було відновлено в активні. HabitID: {habit.HabitId}.");

                this.ToggleHabitStatusMenuItem.Header = "✔ Позначити як подолану";
                this.StatusText.Text = "✔ Активна";

                MessageBox.Show("Звичку повернуто у активні 🟢");
            }

            this.DisplayHabit(this.currentHabitIndex);
        }

        private void NextHabit_Click(object sender, RoutedEventArgs e)
        {
            if (this.habits == null || this.habits.Count == 0)
            {
                return;
            }

            this.currentHabitIndex = (this.currentHabitIndex + 1) % this.habits.Count;
            this.DisplayHabit(this.currentHabitIndex);
        }

        private void PreviousHabit_Click(object sender, RoutedEventArgs e)
        {
            if (this.habits == null || this.habits.Count == 0)
            {
                return;
            }

            this.currentHabitIndex = (this.currentHabitIndex - 1 + this.habits.Count) % this.habits.Count;
            this.DisplayHabit(this.currentHabitIndex);
        }

        private void PreviousHabitLabel_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.PreviousHabit_Click(sender, e);
        }

        private void NextHabitLabel_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.NextHabit_Click(sender, e);
        }

        private void CalendarButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Навігація: Прогрес -> Прогрес за місяць.");
            var w = new CalendarWindow(this.userId);
            w.Owner = this;
            this.Hide();
            w.Show();
        }

        private void AddHabitButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Навігація: Прогрес -> Налаштування звички.");
            var w = new AddHabitWindow(this.userId);
            w.Owner = this;
            this.Hide();
            w.Show();
        }

        private void OpenProfileButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Навігація: Прогрес -> Керування профілем.");
            var w = new ProfileWindow(this.userId);
            w.Owner = this;
            this.Hide();
            w.Show();
        }

        private void AddRelapseButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.habits == null || this.habits.Count == 0)
            {
                this.logger.LogWarning($"Спроба додати зрив без створених звичок. UserID: {this.userId}.");
                MessageBox.Show("Спочатку створіть звичку!");
                return;
            }

            int habitId = this.habits[this.currentHabitIndex].HabitId;

            this.logger.LogInfo("Навігація: Прогрес -> Додати зрив.");
            var w = new AddRelapseWindow(habitId, this.userId);
            w.Owner = this;

            this.Hide();
            w.Show();
        }

        private void SosButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Навігація: Прогрес -> SOS поради.");
            var w = new SosTipsView(this.userId);

            w.Owner = this;
            this.Hide();
            w.Show();
        }

        private void StatisticButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Навігація: Прогрес -> Статистика.");
            var w = new StatisticsView(this.userId);
            w.Owner = this;
            this.Hide();
            w.Show();
        }

        private void MotivationButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Навігація: Прогрес -> Мотивація.");
            var w = new MotivationView(this.userId);
            w.Owner = this;
            this.Hide();
            w.Show();
        }

        private void SupportButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Навігація: Прогрес -> Звернутися у підтримку.");
            var w = new Support(this.userId);
            w.Owner = this;
            this.Hide();
            w.Show();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Навігація: Прогрес -> Про застосунок.");
            var w = new AboutView();
            w.Owner = this;
            this.Hide();
            w.Show();
        }
    }
}