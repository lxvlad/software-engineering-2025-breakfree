namespace BreakFree.Presentation.Views
{
    using System.Windows;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;

    public partial class Support : Window
    {
        private readonly int userId;
        private readonly ILoggerService logger;

        public Support()
            : this(0)
        {
        }

        public Support(int userId)
        {
            this.InitializeComponent();
            this.userId = userId;

            this.logger = new FileLoggerService();
            this.logger.LogInfo($"Вікно 'Звернутися у підтримку' було відкрито. UserID: {userId}.");

            this.Closing += this.Support_Closing;
        }

        private void Support_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            this.logger.LogInfo($"Вікно 'Звернутися у підтримку' було закрито.");

            if (this.Owner != null)
            {
                this.Owner.Show();
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo($"Повідомлення було надіслане у підтримку. UserID: {this.userId}.");
            MessageBox.Show("Повідомлення надіслано! Дякуємо.");
            this.GoBack();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.GoBack();
        }

        private void GoBack()
        {
            this.Close();
        }
    }
}