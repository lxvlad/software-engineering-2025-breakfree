using System;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Data.Sqlite;
using BreakFree.Core.Data;
using BreakFree.Core.Models;

namespace BreakFree.Wpf
{
    public partial class MainWindow : Window
    {
        private readonly BreakFreeContext _db;

        public MainWindow()
        {
            InitializeComponent();
            _db = new BreakFreeContext();
            _db.Database.EnsureCreated();
            Reload();
        }

        private void Reload()
        {
            HabitList.ItemsSource = _db.Habits
                .OrderBy(h => h.Name)
                .Select(h => $"{h.Name} — зривів: {_db.Slips.Count(s => s.HabitId == h.Id)}")
                .ToList();
        }

        private void AddHabit_Click(object sender, RoutedEventArgs e)
        {
            var name = HabitNameBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(name)) return;
            _db.Habits.Add(new Habit { Name = name, CreatedAt = DateTime.UtcNow });
            _db.SaveChanges();
            HabitNameBox.Text = "";
            Reload();
        }

        private void AddSlip_Click(object sender, RoutedEventArgs e)
        {
            var habit = _db.Habits.FirstOrDefault();
            if (habit == null)
            {
                MessageBox.Show("Спочатку додайте хоча б одну звичку.");
                return;
            }
            _db.Slips.Add(new Slip { HabitId = habit.Id, HappenedAt = DateTime.UtcNow });
            _db.SaveChanges();
            Reload();
        }

        private void ShowStats_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            using var conn = new SqliteConnection(BreakFreeContext.ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT h.Name, COUNT(s.Id) AS SlipCount
                FROM Habits h
                LEFT JOIN Slips s ON s.HabitId = h.Id
                    AND s.HappenedAt >= datetime('now','-30 day')
                GROUP BY h.Id, h.Name
                ORDER BY SlipCount DESC, h.Name ASC;";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var name = reader.GetString(0);
                var count = reader.GetInt32(1);
                sb.AppendLine($"{name}: {count} зрив(ів) за 30 днів");
            }
            MessageBox.Show(sb.ToString().Trim().Length == 0 ? "Даних немає" : sb.ToString(), "Статистика");
        }
    }
}
