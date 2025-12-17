namespace BreakFree.Presentation.Views
{
    using System;
    using System.Windows;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;

    public partial class ChangePasswordWindow : Window
    {
        private readonly ILoggerService logger;
        private int userId;
        private UserService userService;

        public ChangePasswordWindow(int userId)
        {
            this.InitializeComponent();
            this.userId = userId;
            this.userService = new UserService();

            this.logger = new FileLoggerService();
            this.logger.LogInfo($"Вікно 'Змінити пароль' було відкрито. UserID: {userId}.");

            this.Closing += this.ChangePasswordWindow_Closing;
        }

        private void ChangePasswordWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.Show();
            }
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string currentPassword = this.txtCurrentPassword.Password.Trim();
            string newPassword = this.txtNewPassword.Password.Trim();

            if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Заповніть усі поля.");
                return;
            }

            try
            {
                bool success = this.userService.ChangePassword(this.userId, currentPassword, newPassword);

                if (success)
                {
                    this.logger.LogInfo($"Пароль було змінено. UserID: {this.userId}.");

                    ProfileWindow profileWindow = new ProfileWindow(this.userId);
                    profileWindow.Show();
                    this.Close();
                }
                else
                {
                    this.logger.LogWarning($"Невдала спроба змінити пароль. UserID: {this.userId}.");
                    MessageBox.Show("Невірний поточний пароль");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Помилка при зміні пароля. UserID: {this.userId}.", ex.StackTrace);
                MessageBox.Show("Сталася помилка при зміні пароля.");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Зміну пароля було скасовано.");
            this.Close();
        }
    }
}
