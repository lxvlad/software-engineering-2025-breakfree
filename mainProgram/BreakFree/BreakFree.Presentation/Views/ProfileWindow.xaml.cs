namespace BreakFree.Presentation.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;

    public partial class ProfileWindow : Window
    {
        private readonly UserService userService = new UserService();
        private readonly ILoggerService logger;
        private int userId;

        private string originalUsername = string.Empty;
        private string originalEmail = string.Empty;

        private bool isDirty = false;

        public ProfileWindow(int userId)
        {
            this.userId = userId;
            this.InitializeComponent();

            this.logger = new FileLoggerService();
            this.logger.LogInfo($"Вікно 'Керування профілем' було відкрито. UserID: {userId}.");

            this.LoadUserData();

            this.txtUsername.TextChanged += this.OnFieldChanged;
            this.txtEmail.TextChanged += this.OnFieldChanged;

            this.Closing += this.ProfileWindow_Closing;
        }

        private void ProfileWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.Show();
            }
        }

        private void LoadUserData()
        {
            try
            {
                var user = this.userService.GetUserById(this.userId);
                if (user != null)
                {
                    this.txtUsername.Text = user.UserName;
                    this.txtEmail.Text = user.Email;

                    this.originalUsername = user.UserName;
                    this.originalEmail = user.Email;
                }

                HabitService habitService = new HabitService();
                var habits = habitService.GetUserHabits(this.userId);

                if (habits != null && habits.Count > 0)
                {
                    var firstDate = habits.Min(h => h.StartDate);
                    this.StartDateText.Text = firstDate.ToString("dd/MM/yyyy");
                }
                else
                {
                    this.StartDateText.Text = "Ще немає звичок";
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("Помилка при завантажені даних профілю.", ex.StackTrace);
                MessageBox.Show("Помилка завантаження даних.");
            }
        }

        private void OnFieldChanged(object sender, TextChangedEventArgs e)
        {
            if (this.txtUsername.Text != this.originalUsername || this.txtEmail.Text != this.originalEmail)
            {
                this.isDirty = true;
            }
            else
            {
                this.isDirty = false;
            }
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Навігація: Керування профілем -> Змінити пароль.");
            ChangePasswordWindow changePasswordWindow = new ChangePasswordWindow(this.userId);
            changePasswordWindow.Show();
            this.Close();
        }

        private void DeleteProfileButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo($"Навігація: Керування профілем -> Видалити профіль. UserID: {this.userId}.");
            DeleteProfileWindow deleteProfileWindow = new DeleteProfileWindow(this.userId);
            deleteProfileWindow.Show();
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            string newUsername = this.txtUsername.Text.Trim();
            string newEmail = this.txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(newUsername) || string.IsNullOrWhiteSpace(newEmail))
            {
                this.logger.LogWarning($"Спроба збрегти профіль з пустими полями. UserID: {this.userId}.");
                MessageBox.Show("Заповніть всі поля");
                return;
            }

            try
            {
                bool success = this.userService.UpdateUser(this.userId, newUsername, newEmail);

                if (success)
                {
                    this.logger.LogInfo($"Профіль оновлено. Новий логін: {newUsername}, Email: {newEmail}.");
                    var main = new Dashboard(this.userId);
                    main.Show();
                    this.Close();
                }
                else
                {
                    this.logger.LogWarning($"Невдале оновлення профілю. UserID: {this.userId}.");
                    MessageBox.Show("Email або Username вже використовується іншим користувачем");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Критична помилка при збереженні профілю. UserID: {this.userId}", ex.StackTrace);
                MessageBox.Show("Сталася помилка при збереженні.");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.isDirty)
            {
                this.Close();
                return;
            }

            ConfirmCancelWindow confirm = new ConfirmCancelWindow();

            if (confirm.ShowDialog() == true)
            {
                this.logger.LogInfo("Користувач скасував незбережені зміни в профілі.");
                this.Close();
            }
        }
    }
}
