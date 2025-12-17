namespace BreakFree.Presentation.Views
{
    using System.Windows;
    using System.Windows.Media;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;
    using BreakFree.DAL.Entities;

    public partial class EditRelapseWindow : Window
    {
        private readonly DailyStatusService service;
        private readonly ILoggerService logger;

        private readonly int statusId;
        private readonly int habitId;

        public EditRelapseWindow(JournalItem item)
        {
            this.InitializeComponent();
            this.service = new DailyStatusService();
            this.logger = new FileLoggerService();

            this.statusId = item.StatusId;
            this.habitId = item.HabitId;

            this.logger.LogInfo($"Вікно 'Редагування запису' було відкрито. StatusID: {this.statusId}.");

            this.DateTimePicker.SelectedDate = item.FullDate;
            this.DateTimePicker.DisplayDateEnd = DateTime.Today;
            this.TriggerComboBox.Text = item.Reason;
            this.NoteTextBox.Text = item.Note;
            this.IntensitySlider.Value = item.CravingLevel;

            this.Closing += this.EditRelapseWindow_Closing;
        }

        private void EditRelapseWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.Show();

                if (this.Owner is JournalWindow journal)
                {
                    journal.RefreshJournal();
                }
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.SaveButton.IsEnabled = false;

            try
            {
                var status = new DailyStatus
                {
                    StatusId = this.statusId,
                    HabitId = this.habitId,
                    DateTime = this.DateTimePicker.SelectedDate ?? DateTime.Now,
                    Trigger = this.TriggerComboBox.Text,
                    Note = this.NoteTextBox.Text,
                    CravingLevel = (int)this.IntensitySlider.Value,
                };

                this.service.UpdateDailyStatus(status);

                this.logger.LogInfo($"Запис зриву було оновлено. StatusID: {this.statusId}.");

                this.StatusText.Text = "Зміни збережено!";
                this.StatusText.Foreground = Brushes.Green;

                await Task.Delay(1000);

                this.Close();
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Помилка при оновлені запису зриву. StatusID: {this.statusId}.", ex.StackTrace);

                this.StatusText.Text = "Помилка: " + ex.Message;
                this.StatusText.Foreground = Brushes.Red;
                this.SaveButton.IsEnabled = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Редагування запису про зрив було скасовано.");
            this.Close();
        }
    }
}