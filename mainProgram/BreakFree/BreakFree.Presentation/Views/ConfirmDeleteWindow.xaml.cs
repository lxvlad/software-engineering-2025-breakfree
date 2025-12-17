namespace BreakFree.Presentation.Views
{
    using System.Windows;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;

    public partial class ConfirmDeleteWindow : Window
    {
        private readonly ILoggerService logger;

        public ConfirmDeleteWindow()
        {
            this.InitializeComponent();
            this.logger = new FileLoggerService();
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Користувач підтвердив видалення.");
            this.DialogResult = true;
            this.Close();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Користувач скасував видалення.");
            this.DialogResult = false;
            this.Close();
        }
    }
}