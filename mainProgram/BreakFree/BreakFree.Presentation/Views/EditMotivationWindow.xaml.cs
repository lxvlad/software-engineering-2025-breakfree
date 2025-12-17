namespace BreakFree.Presentation.Views
{
    using System.Windows;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;

    public partial class EditMotivationWindow : Window
    {
        private readonly ILoggerService logger;

        public EditMotivationWindow(string currentMotivation)
        {
            this.InitializeComponent();
            this.logger = new FileLoggerService();

            this.MotivationTextBox.Text = currentMotivation;

            this.MotivationTextBox.Focus();
            this.MotivationTextBox.CaretIndex = this.MotivationTextBox.Text.Length;
        }

        public string Motivation { get; private set; } = string.Empty;

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            this.Motivation = this.MotivationTextBox.Text.Trim();
            this.logger.LogInfo("Мотивацію було оновлено.");

            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Зміна мотивації була скасована.");
            this.DialogResult = false;
            this.Close();
        }
    }
}
