namespace BreakFree.Presentation.Views
{
    using System.Windows;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;

    public partial class AboutView : Window
    {
        private readonly ILoggerService logger;

        public AboutView()
        {
            this.InitializeComponent();

            this.logger = new FileLoggerService();
            this.logger.LogInfo("Вікно 'Про застосунок' було відкрито.");

            this.Closing += this.AboutWindow_Closing;
        }

        private void AboutWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            this.logger.LogInfo("Вікно 'Про застосунок' було закрито.");

            if (this.Owner != null)
            {
                this.Owner.Show();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
