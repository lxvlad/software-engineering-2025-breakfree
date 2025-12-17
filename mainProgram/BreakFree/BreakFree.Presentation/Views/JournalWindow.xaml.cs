namespace BreakFree.Presentation.Views
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using BreakFree.BLL.Interfaces;
    using BreakFree.BLL.Services;

    public partial class JournalWindow : Window
    {
        private readonly int userId;
        private readonly DailyStatusService statusService;
        private readonly HabitService habitService;
        private readonly ILoggerService logger;

        public JournalWindow(int userId)
        {
            this.InitializeComponent();
            this.userId = userId;
            this.statusService = new DailyStatusService();
            this.habitService = new HabitService();
            this.logger = new FileLoggerService();

            this.logger.LogInfo($"Вікно 'Журнал зривів' було відкрито. UserID: {userId}.");

            this.IsVisibleChanged += this.JournalWindow_IsVisibleChanged;
            this.Closing += this.JournalWindow_Closing;

            this.LoadJournal();
        }

        public void RefreshJournal()
        {
            this.logger.LogInfo("Дані 'Журналу зривів' було оновлено.");
            this.LoadJournal();
        }

        public async void HighlightNewEntry(int statusId)
        {
            try
            {
                this.LoadJournal();

                var item = this.JournalDataGrid.Items.OfType<JournalItem>()
                                               .FirstOrDefault(i => i.StatusId == statusId);

                if (item != null)
                {
                    this.JournalDataGrid.UpdateLayout();
                    this.JournalDataGrid.ScrollIntoView(item);
                    this.JournalDataGrid.SelectedItem = item;

                    await Task.Delay(200);

                    var row = (DataGridRow)this.JournalDataGrid.ItemContainerGenerator.ContainerFromItem(item);
                    if (row != null)
                    {
                        this.JournalDataGrid.SelectedItem = null;
                        var oldBrush = row.Background;
                        row.Background = new SolidColorBrush(Color.FromRgb(255, 255, 180));
                        await Task.Delay(1000);
                        row.Background = oldBrush;
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogWarning($"Помилка анімації: {ex.Message}");
            }
        }

        private void JournalWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                this.LoadJournal();
            }
        }

        private void JournalWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            this.logger.LogInfo("Вікно 'Журнал зривів' було закрито.");

            if (this.Owner != null)
            {
                this.Owner.Show();
            }
        }

        private void LoadJournal()
        {
            try
            {
                var statuses = this.statusService.GetStatusesByUser(this.userId);

                var journalItems = statuses
                    .OrderByDescending(s => s.DateTime)
                    .Select(s => new JournalItem
                    {
                        StatusId = s.StatusId,
                        HabitId = s.HabitId,
                        CravingLevel = s.CravingLevel ?? 0,
                        FullDate = s.DateTime,

                        Date = s.DateTime.ToString("dd.MM"),
                        Reason = s.Trigger ?? "-",
                        Note = s.Note ?? "-",
                    })
                    .ToList();

                this.JournalDataGrid.ItemsSource = journalItems;
            }
            catch (Exception ex)
            {
                this.logger.LogError("Помилка при завантажені записів журналу.", ex.StackTrace);
                MessageBox.Show("Не вдалося завантажити журнал.");
            }
        }

        private void AddRelapse_Click(object sender, RoutedEventArgs e)
        {
            var habits = this.habitService.GetUserHabits(this.userId);

            if (habits != null && habits.Count > 0)
            {
                int firstHabitId = habits.First().HabitId;

                this.logger.LogInfo("Навігація: Журнал зривів -> Додати запис про зрив.");

                AddRelapseWindow addWindow = new AddRelapseWindow(firstHabitId, this.userId);
                addWindow.Owner = this;
                this.Hide();
                addWindow.Show();
            }
            else
            {
                this.logger.LogWarning($"Спроба додати запис в журнал без активних звичок. UserID: {this.userId}.");
                MessageBox.Show("Спочатку створіть хоча б одну звичку на головній сторінці!");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EditEntry_Click(object sender, RoutedEventArgs e)
        {
            if (this.JournalDataGrid.SelectedItem is JournalItem selectedItem)
            {
                this.logger.LogInfo($"Навігація: Журнал зривів -> Редагувати запис(StatusID: {selectedItem.StatusId}).");

                var editWindow = new EditRelapseWindow(selectedItem);
                editWindow.Owner = this;
                this.Hide();
                editWindow.Show();
            }
        }

        private void DeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            if (this.JournalDataGrid.SelectedItem is JournalItem selectedItem)
            {
                var confirm = new ConfirmDeleteWindow();
                if (confirm.ShowDialog() == true)
                {
                    try
                    {
                        this.statusService.DeleteDailyStatus(selectedItem.StatusId);
                        this.logger.LogWarning($"Запис журналу було видалено. StatusID: {selectedItem.StatusId}.");
                        this.LoadJournal();
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogError($"Помилка при видаленні запису. StatusID: {selectedItem.StatusId}", ex.StackTrace);
                        MessageBox.Show("Помилка видалення: " + ex.Message);
                    }
                }
                else
                {
                    this.logger.LogInfo("Видалення запису було скасовано.");
                }
            }
        }
    }
}