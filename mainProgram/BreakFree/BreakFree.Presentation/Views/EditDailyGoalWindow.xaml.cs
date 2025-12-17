namespace BreakFree.Presentation.Views
{
    using System.Windows;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;

    public partial class EditDailyGoalWindow : Window
    {
        private readonly ILoggerService logger;

        public EditDailyGoalWindow(int currentGoal)
        {
            this.InitializeComponent();
            this.logger = new FileLoggerService();

            this.GoalTextBox.Text = currentGoal.ToString();
            this.GoalTextBox.Focus();
            this.GoalTextBox.SelectAll();
        }

        public int NewGoal { get; private set; }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(this.GoalTextBox.Text, out int result) && result >= 0)
            {
                this.NewGoal = result;

                this.logger.LogInfo($"Ціль було змінено на {this.NewGoal}.");

                this.DialogResult = true;
                this.Close();
            }
            else
            {
                this.logger.LogWarning($"Некоректне введення нової цілі: '{this.GoalTextBox}'");
                MessageBox.Show("Будь ласка, введіть коректне ціле число.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Зміна цілі була скасована.");
            this.DialogResult = false;
            this.Close();
        }
    }
}
