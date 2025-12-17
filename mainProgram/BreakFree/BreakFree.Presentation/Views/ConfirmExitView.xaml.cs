namespace BreakFree.Presentation.Views
{
    using System.Windows;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;

    public partial class ConfirmExitView : Window
    {
        private readonly ILoggerService logger;

        public ConfirmExitView()
        {
            this.InitializeComponent();
            this.logger = new FileLoggerService();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Користувач підтвердив вихід з програми.");
            this.DialogResult = true;
            this.Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Користувач залишився в програмі.");
            this.DialogResult = false;
            this.Close();
        }
    }
}