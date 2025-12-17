namespace BreakFree.Presentation.Views
{
    using System;
    using System.Windows;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;

    public partial class ConfirmCancelWindow : Window
    {
        private readonly int userId;
        private readonly ILoggerService logger;

        public ConfirmCancelWindow()
        {
            this.InitializeComponent();

            this.logger = new FileLoggerService();

            this.Closing += this.ConfirmCancelWindow_Closing;
        }

        private void ConfirmCancelWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.Show();
            }
        }

        private void DiscardButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Підтвердження скасування змін.");
            this.DialogResult = true;
            this.Close();
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Підтвердження для продовження редагування.");
            this.DialogResult = false;
            this.Close();
        }
    }
}