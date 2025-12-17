namespace BreakFree.Presentation.Views
{
    using System.Windows;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;
    using BreakFree.DAL.Entities;

    public partial class RenameHabitWindow : Window
    {
        private readonly ILoggerService logger;

        public RenameHabitWindow(Habit habit)
        {
            this.InitializeComponent();

            this.logger = new FileLoggerService();

            this.Owner = Application.Current.MainWindow;

            this.NameTextBox.Text = habit.HabitName;
            this.IsActiveCheckBox.IsChecked = habit.IsActive;
        }

        public string NewName { get; private set; } = string.Empty;

        public new bool IsActive { get; private set; }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.NameTextBox.Text))
            {
                this.logger.LogWarning("Спроба зберегти звичку з пустою назвою.");
                MessageBox.Show("Назва не може бути порожньою.");
                return;
            }

            this.NewName = this.NameTextBox.Text.Trim();
            this.IsActive = this.IsActiveCheckBox.IsChecked ?? true;

            this.logger.LogInfo($"Зміни було збережено: Назва - '{this.NewName}', Активна - {this.IsActive}.");

            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Перейменування звички скасовано.");
            this.DialogResult = false;
            this.Close();
        }
    }
}
