namespace BreakFree.Presentation.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;

    public partial class SosTipsView : Window
    {
        private readonly int userId;
        private readonly SosService sosService;
        private readonly ILoggerService logger;
        private bool isAnyButtonAnimating = false;

        public SosTipsView(int userId)
        {
            this.InitializeComponent();
            this.userId = userId;
            this.sosService = new SosService();

            this.logger = new FileLoggerService();
            this.logger.LogInfo($"Вікно 'SOS порaди' було відкрито. UserID: {userId}.");

            this.Closing += this.SosTipsView_Closing;
            this.LoadTips();
        }

        private void LoadTips()
        {
            try
            {
                var tips = this.sosService.GetSortedTips(this.userId);

                if (tips.Count == 0)
                {
                    this.logger.LogWarning("Список порад відсутній.");
                    MessageBox.Show("Увага: Список порад пустий! Перевірте базу даних.");
                }

                this.TipsListControl.ItemsSource = tips;
            }
            catch (Exception ex)
            {
                this.logger.LogError("Помилка завантаження SOS порад", ex.StackTrace);
            }
        }

        private void SosTipsView_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            this.logger.LogInfo("Вікно 'SOS порaди' було закрито.");

            if (this.Owner != null)
            {
                this.Owner.Show();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void TipWorked_Click(object sender, RoutedEventArgs e)
        {
            if (this.isAnyButtonAnimating)
            {
                return;
            }

            if (sender is not Button btn)
            {
                return;
            }

            try
            {
                if (int.TryParse(btn.Tag.ToString(), out int actionId))
                {
                    this.logger.LogInfo($"Порада {actionId} спрацювала.");
                    this.sosService.LogAttempt(this.userId, actionId, true);
                }

                this.isAnyButtonAnimating = true;
                var original = btn.Background;
                btn.Background = Brushes.LightGreen;
                await Task.Delay(1000);
                btn.Background = original;
                this.isAnyButtonAnimating = false;

                this.LoadTips();
            }
            catch (Exception ex)
            {
                this.logger.LogError("Помилка при оцінці поради.", ex.StackTrace);
                this.isAnyButtonAnimating = false;
            }
        }

        private async void TipNotWorked_Click(object sender, RoutedEventArgs e)
        {
            if (this.isAnyButtonAnimating)
            {
                return;
            }

            if (sender is not Button btn)
            {
                return;
            }

            try
            {
                if (int.TryParse(btn.Tag.ToString(), out int actionId))
                {
                    this.logger.LogInfo($"Порада {actionId} НЕ спрацювала.");
                    this.sosService.LogAttempt(this.userId, actionId, false);
                }

                this.isAnyButtonAnimating = true;
                var original = btn.Background;
                btn.Background = Brushes.IndianRed;
                await Task.Delay(1000);
                btn.Background = original;
                this.isAnyButtonAnimating = false;

                this.LoadTips();
            }
            catch (Exception ex)
            {
                this.logger.LogError("Помилка при оцінці поради.", ex.StackTrace);
                this.isAnyButtonAnimating = false;
            }
        }
    }
}