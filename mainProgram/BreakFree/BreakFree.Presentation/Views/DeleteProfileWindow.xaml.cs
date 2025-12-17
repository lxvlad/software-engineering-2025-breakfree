namespace BreakFree.Presentation.Views
{
    using System;
    using System.Windows;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;

    public partial class DeleteProfileWindow : Window
    {
        private readonly ILoggerService logger;

        private int userId;

        private UserService userService;

        public DeleteProfileWindow(int userId)
        {
            this.InitializeComponent();
            this.userId = userId;
            this.userService = new UserService();

            this.logger = new FileLoggerService();
            this.logger.LogInfo($"Вікно 'Видалити профіль' було відкрито. UserID: {userId}.");

            this.Closing += this.DeleteProfileWindow_Closing;
        }

        private void DeleteProfileWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.Show();
            }
        }

        private void DeleteProfileButton_Click(object sender, RoutedEventArgs e)
        {
            string password = this.txtPassword.Password.Trim();

            if (string.IsNullOrWhiteSpace(password))
            {
                this.logger.LogWarning($"Спроба видалення профілю без пароля. UserId:{this.userId}.");
                MessageBox.Show("Введіть пароль");
                return;
            }

            try
            {
                bool success = this.userService.DeleteUser(this.userId, password);

                if (success)
                {
                    this.logger.LogWarning($"Профіль видалено назавжди. UserId:{this.userId}.");
                    Application.Current.Shutdown();
                }
                else
                {
                    this.logger.LogWarning($"Невдала спроба видалення профілю. UserId:{this.userId}.");
                    MessageBox.Show("Невірний пароль або сталася помилка");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Критична помилка при видаленні профілю. UserID: {this.userId}.", ex.StackTrace);
                MessageBox.Show("Сталися помилка при спробі видалення");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Видалення профілю скасовано.");
            this.Close();
        }
    }
}
