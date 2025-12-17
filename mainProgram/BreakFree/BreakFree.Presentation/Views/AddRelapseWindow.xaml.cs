namespace BreakFree.Presentation.Views
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;
    using BreakFree.DAL.Entities;

    public partial class AddRelapseWindow : Window
    {
        private readonly int habitId;
        private readonly DailyStatusService dailyStatusService;
        private readonly ILoggerService logger;
        private readonly int userId;
        private int userIdForNavigation;

        public AddRelapseWindow(int habitId, int userId)
        {
            this.InitializeComponent();
            this.habitId = habitId;
            this.userIdForNavigation = userId;
            this.dailyStatusService = new DailyStatusService();

            this.logger = new FileLoggerService();

            this.DateTimePicker.DisplayDateEnd = DateTime.Today;
            this.DateTimePicker.SelectedDate = DateTime.Today;

            this.userId = userId;

            this.logger.LogInfo($"Вікно 'Додати зрив' було відкрито. HabitID: {habitId}, UserID: {userId}.");

            this.Closing += this.AddRelapseWindow_Closing;
        }

        private void AddRelapseWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.Show();
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime selectedDate = this.DateTimePicker.SelectedDate ?? DateTime.Today;

            if (selectedDate.Date > DateTime.Today)
            {
                this.logger.LogWarning($"Спроба додати зрив у майбутньому. UserID: {this.userId}.");
                this.StatusText.Text = "Неможливо додати зрив у майбутньому!";
                this.StatusText.Foreground = Brushes.Red;
                return;
            }

            this.SaveButton.IsEnabled = false;

            var status = new DailyStatus
            {
                HabitId = this.habitId,
                DateTime = selectedDate,
                Trigger = this.TriggerComboBox.Text,
                Note = this.NoteTextBox.Text,
                CravingLevel = (int)this.IntensitySlider.Value,
            };

            try
            {
                this.dailyStatusService.AddDailyStatus(status);

                this.logger.LogInfo($"Зрив було додано. ID запису: {status.StatusId}, Тригер/прчина: {status.Trigger}.");

                this.StatusText.Text = "Успішно! Переходимо в журнал...";
                this.StatusText.Foreground = Brushes.Green;

                await Task.Delay(1000);

                JournalWindow targetJournal;

                if (this.Owner is JournalWindow jw)
                {
                    targetJournal = jw;
                }
                else
                {
                    targetJournal = new JournalWindow(this.userId);
                    targetJournal.Owner = this.Owner;
                    this.Owner = null;
                }

                targetJournal.Show();

                targetJournal.HighlightNewEntry(status.StatusId);

                this.Close();
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Помилка при збережені зриву. HabitID {this.habitId}.", ex.StackTrace);

                this.StatusText.Text = "Помилка: " + ex.Message;
                this.StatusText.Foreground = Brushes.Red;
                this.SaveButton.IsEnabled = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Додавання зриву було скасовано.");
            this.Close();
        }
    }
}