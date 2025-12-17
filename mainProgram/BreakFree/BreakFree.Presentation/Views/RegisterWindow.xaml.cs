namespace BreakFree.Presentation.Views
{
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;

    public partial class RegisterWindow : Window
    {
        private readonly UserService userService = new ();
        private readonly ILoggerService logger;

        public RegisterWindow()
        {
            this.InitializeComponent();
            this.logger = new FileLoggerService();
            this.logger.LogInfo("Вікно 'Реєстрація' було відкрито.");
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.NameTextBox.Text))
            {
                this.SetError(this.NameErrorText, "Ім'я не може бути пустим");
            }
            else
            {
                this.SetSuccess(this.NameErrorText, "Чудово!");
            }
        }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(this.EmailTextBox.Text, emailPattern))
            {
                this.SetError(this.EmailErrorText, "Некоректний формат Email");
            }
            else
            {
                this.SetSuccess(this.EmailErrorText, "Email підходить");
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string password = this.PasswordBox.Password;
            string errorMsg = string.Empty;

            if (password.Length < 6)
            {
                errorMsg += "Мін. 6 символів. ";
            }

            if (!password.Any(char.IsUpper))
            {
                errorMsg += "Потрібна велика літера. ";
            }

            if (!password.Any(char.IsDigit))
            {
                errorMsg += "Потрібна цифра. ";
            }

            if (!string.IsNullOrEmpty(errorMsg))
            {
                this.SetError(this.PasswordErrorText, errorMsg);
            }
            else
            {
                this.SetSuccess(this.PasswordErrorText, "Надійний пароль");
            }

            if (this.ConfirmPasswordBox.Password.Length > 0)
            {
                this.ConfirmPasswordBox_PasswordChanged(sender, e);
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.PasswordBox.Password != this.ConfirmPasswordBox.Password)
            {
                this.SetError(this.ConfirmErrorText, "Паролі не збігаються");
            }
            else if (string.IsNullOrEmpty(this.ConfirmPasswordBox.Password))
            {
                this.ConfirmErrorText.Text = string.Empty;
            }
            else
            {
                this.SetSuccess(this.ConfirmErrorText, "Паролі збігаються");
            }
        }

        private void SetError(TextBlock block, string message)
        {
            block.Text = message;
            block.Foreground = Brushes.Red;
        }

        private void SetSuccess(TextBlock block, string message)
        {
            block.Text = message;
            block.Foreground = Brushes.Green;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.NameErrorText.Foreground == Brushes.Red ||
                this.EmailErrorText.Foreground == Brushes.Red ||
                this.PasswordErrorText.Foreground == Brushes.Red ||
                this.ConfirmErrorText.Foreground == Brushes.Red ||
                string.IsNullOrWhiteSpace(this.PasswordBox.Password))
            {
                this.logger.LogWarning("Спроба реєстрації з помилками валідації.");
                MessageBox.Show("Будь ласка, виправте помилки у формі.");
                return;
            }

            var name = this.NameTextBox.Text.Trim();
            var email = this.EmailTextBox.Text.Trim();
            var password = this.PasswordBox.Password;

            try
            {
                bool success = this.userService.Register(name, email, password);

                if (success)
                {
                    this.logger.LogInfo($"Новий користувач був зареєстрований: {email} (Name: {name}).");
                    MessageBox.Show("Успішно зареєстровано! Увійдіть у свій акаунт.");

                    var login = new LoginWindow();
                    login.Show();
                    this.Close();
                }
                else
                {
                    this.logger.LogWarning($"Невдала спроба реєстрації. Email зайнятий {email}.");
                    MessageBox.Show("Цей Email вже зареєстрований або виникла помилка.");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Критична помилак при реєстрації {email}.", ex.StackTrace);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Користувач скасував реєстрацію.");
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}