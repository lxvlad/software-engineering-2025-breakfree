namespace BreakFree.Presentation.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;

    public partial class LoginWindow : Window
    {
        private readonly UserService userService = new ();
        private readonly ILoggerService logger;

        public LoginWindow()
        {
            this.InitializeComponent();

            this.logger = new FileLoggerService();
            this.logger.LogInfo("Вікно 'Вхід' було відкрито.");
        }

        private void TxtLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtUsername.Text))
            {
                this.LoginErrorText.Text = "Введіть логін";
                this.LoginErrorText.Foreground = Brushes.Red;
            }
            else
            {
                this.LoginErrorText.Text = string.Empty;
            }
        }

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtPassword.Password))
            {
                this.PasswordErrorText.Text = "Введіть пароль";
                this.PasswordErrorText.Foreground = Brushes.Red;
            }
            else
            {
                this.PasswordErrorText.Text = string.Empty;
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            bool hasError = false;

            if (string.IsNullOrWhiteSpace(this.txtUsername.Text))
            {
                this.LoginErrorText.Text = "Введіть логін";
                this.LoginErrorText.Foreground = Brushes.Red;
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(this.txtPassword.Password))
            {
                this.PasswordErrorText.Text = "Введіть пароль";
                this.PasswordErrorText.Foreground = Brushes.Red;
                hasError = true;
            }

            if (hasError)
            {
                this.logger.LogWarning("Спроба входу з пустими полями.");
                MessageBox.Show("Будь ласка, заповніть всі поля.");
                return;
            }

            var username = this.txtUsername.Text.Trim();
            var password = this.txtPassword.Password;

            try
            {
                var user = this.userService.Login(username, password);

                if (user != null)
                {
                    this.logger.LogInfo($"Було здийснено вхід користувача: {username}. UserID: {user.UserId}.");

                    int userId = user.UserId;
                    var main = new Dashboard(userId);
                    main.Show();
                    this.Close();
                }
                else
                {
                    this.logger.LogWarning($"Невдала спроба входу: '{username}'.");
                    MessageBox.Show("Невірний логін або пароль!");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("Критична помилка при спробі входу", ex.StackTrace);
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Навігація: Вхід -> Реєстрація.");
            var reg = new RegisterWindow();
            reg.Show();
            this.Close();
        }
    }
}