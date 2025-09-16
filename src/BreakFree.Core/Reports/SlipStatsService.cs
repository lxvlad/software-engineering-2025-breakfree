using Microsoft.Data.Sqlite;

namespace BreakFree.Core.Reports
{
    public class SlipStatsService
    {
        private readonly string _connectionString;
        public SlipStatsService(string connectionString) => _connectionString = connectionString;

        public IEnumerable<(string Habit, int Count)> TopSlipsLastDays(int days = 30)
        {
            var list = new List<(string, int)>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT h.Name, COUNT(s.Id) AS SlipCount
                FROM Habits h
                LEFT JOIN Slips s ON s.HabitId = h.Id
                    AND s.HappenedAt >= datetime('now', $window)
                GROUP BY h.Id, h.Name
                ORDER BY SlipCount DESC, h.Name ASC;";
            cmd.Parameters.AddWithValue("$window", $"-{days} day");
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var name = reader.GetString(0);
                var count = reader.GetInt32(1);
                list.Add((name, count));
            }
            return list;
        }
    }
}
