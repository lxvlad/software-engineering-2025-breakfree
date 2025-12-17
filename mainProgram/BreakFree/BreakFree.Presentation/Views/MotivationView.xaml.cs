namespace BreakFree.Presentation.Views
{
    using System.Windows;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;

    public partial class MotivationView : Window
    {
        private readonly int userId;
        private readonly HabitService habitService;
        private readonly DailyStatusService statusService;
        private readonly ILoggerService logger;

        private readonly List<string> quotes = new List<string>
        {
            "Кожен день без звички — це перемога над собою вчорашнім. Ти вже сильніший, ніж думаєш.",
            "Успіх — це сума невеликих зусиль, що повторюються день у день.",
            "Не чекай, поки стане легше, простіше, краще. Не стане. Труднощі будуть завжди. Вчися бути щасливим прямо зараз.",
            "Сила не в тому, щоб ніколи не падати, а в тому, щоб підніматися кожного разу, коли падаєш.",
            "Твоє майбутнє створюється тим, що ти робиш сьогодні, а не тим, що робитимеш завтра.",
            "Дисципліна — це вибір між тим, чого ти хочеш зараз, і тим, чого ти хочеш найбільше.",
            "Найважчий крок — це крок за поріг звичної поведінки.",
            "Інвестуй у себе. Це приносить найкращі дивіденди.",
            "Свобода починається там, де закінчується залежність.",
            "Ти можеш більше, ніж тобі здається. Просто продовжуй йти.",
        };

        public MotivationView()
            : this(0)
        {
        }

        public MotivationView(int userId)
        {
            this.InitializeComponent();
            this.userId = userId;

            this.habitService = new HabitService();
            this.statusService = new DailyStatusService();

            this.logger = new FileLoggerService();
            this.logger.LogInfo($"Вікно 'Мотивація' було відкрито. UserID: {userId}.");

            this.LoadQuoteOfTheDay();
            this.CalculateSavings();

            this.Closing += this.MotivationView_Closing;
        }

        private void MotivationView_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            this.logger.LogInfo("Вікно 'Мотивація' було закрито.");

            if (this.Owner != null)
            {
                this.Owner.Show();
            }
        }

        private void LoadQuoteOfTheDay()
        {
            if (this.quotes.Count == 0)
            {
                return;
            }

            int dayOfYear = DateTime.Now.DayOfYear;
            int quoteIndex = dayOfYear % this.quotes.Count;

            this.QuoteTextBlock.Text = this.quotes[quoteIndex];
        }

        private void ChangeQuote_Click(object sender, RoutedEventArgs e)
        {
            if (this.quotes.Count == 0)
            {
                return;
            }

            this.logger.LogInfo("Цитата була змінена.");

            Random random = new Random();
            int index = random.Next(this.quotes.Count);

            this.QuoteTextBlock.Text = this.quotes[index];
        }

        private void CalculateSavings()
        {
            if (this.userId <= 0)
            {
                return;
            }

            try
            {
                var habits = this.habitService.GetUserHabits(this.userId);
                var statuses = this.statusService.GetStatusesByUser(this.userId);

                decimal totalSaved = 0;
                int totalCleanDays = 0;

                foreach (var habit in habits)
                {
                    var daysSinceStart = (DateTime.Now.Date - habit.StartDate.Date).Days + 1;

                    if (daysSinceStart <= 0)
                    {
                        continue;
                    }

                    int relapseDays = statuses
                        .Where(s => s.HabitId == habit.HabitId && (s.CravingLevel.HasValue && s.CravingLevel.Value > 0))
                        .Select(s => s.DateTime.Date)
                        .Distinct()
                        .Count();

                    int cleanDaysForHabit = daysSinceStart - relapseDays;
                    if (cleanDaysForHabit < 0)
                    {
                        cleanDaysForHabit = 0;
                    }

                    totalCleanDays += cleanDaysForHabit;
                    totalSaved += cleanDaysForHabit * habit.DailyGoal;
                }

                this.MoneySavedText.Text = $"{totalSaved} ₴";
                this.CleanDaysText.Text = $"за {totalCleanDays} днів без звички (сумарно)";
            }
            catch (Exception ex)
            {
                this.logger.LogError("Помилка при розрахунку заощаджень.", ex.StackTrace);
            }
        }

        private void SosButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.LogInfo("Навігація: Мотивація -> SOS поради.");
            var sosWindow = new SosTipsView(this.userId);

            sosWindow.Owner = this;
            this.Hide();
            sosWindow.Show();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}