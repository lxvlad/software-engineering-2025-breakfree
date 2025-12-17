namespace BreakFree.Presentation.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;

    public partial class AddHabitWindow : Window
    {
        private readonly HabitService habitService;
        private readonly ILoggerService logger;

        private int userId;

        public AddHabitWindow(int userId)
        {
            this.InitializeComponent();
            this.userId = userId;
            this.habitService = new HabitService();

            this.logger = new FileLoggerService();
            this.logger.LogInfo($"Вікно 'Налаштування звички' було відкрито. UserID: {userId}.");

            this.txtStartDate.SelectedDate = DateTime.Today;

            this.Closing += this.AddHabitWindow_Closing;
        }

        private void AddHabitWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.Show();
            }
        }

        private void TxtGoal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtGoal.Text))
            {
                this.GoalErrorText.Text = string.Empty;
                return;
            }

            if (int.TryParse(this.txtGoal.Text, out int result) && result > 0)
            {
                this.GoalErrorText.Text = "Коректне число";
                this.GoalErrorText.Foreground = Brushes.Green;
            }
            else
            {
                this.GoalErrorText.Text = "Має бути цілим числом > 0";
                this.GoalErrorText.Foreground = Brushes.Red;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var habitName = this.txtHabitName.Text.Trim();
            var startDateString = this.txtStartDate.Text.Trim();
            var goalString = this.txtGoal.Text.Trim();
            var motivation = this.txtMotivation.Text.Trim();

            if (string.IsNullOrWhiteSpace(habitName) ||
                string.IsNullOrWhiteSpace(startDateString) ||
                string.IsNullOrWhiteSpace(goalString))
            {
                this.logger.LogWarning($"Спроба збереження звички з пустими полями. UserID: {this.userId}.");
                MessageBox.Show("Будь ласка, заповніть усі обов'язкові поля.");
                return;
            }

            if (!DateTime.TryParse(startDateString, out var startDate))
            {
                this.logger.LogWarning($"Невірний формат дати: {startDateString}. UserID: {this.userId}.");
                MessageBox.Show("Неправильний формат дати.");
                return;
            }

            if (!int.TryParse(goalString, out var goal) || goal <= 0)
            {
                this.logger.LogWarning($"Невірний формат цілі: {goalString}. UserID: {this.userId}.");
                MessageBox.Show("Ціль повинна бути додатнім цілим числом.");
                return;
            }

            if (this.GoalErrorText.Foreground == Brushes.Red)
            {
                MessageBox.Show("Виправте помилки перед збереженням.");
                return;
            }

            try
            {
                this.habitService.AddHabit(this.userId, habitName, startDate, goal, motivation);

                this.logger.LogInfo($"Нову звичку: '{habitName}' було створено. UserID: {this.userId}.");

                if (this.Owner is Dashboard dashboard)
                {
                    dashboard.RefreshData();
                }

                this.Close();
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Помилка при збереженні звички '{habitName}'. UserID: {this.userId}.", ex.StackTrace);

                MessageBox.Show("Помилка при збереженні: " + ex.Message);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Додавання звички було скасовано.");
            this.Close();
        }
    }
}