using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace BreakFree.ConsoleSeed
{
    public static class Seed
    {
        private const int SEED_COUNT = 40;
        private static readonly string[] HabitNames = { "Smoking", "Alcohol", "SocialMedia", "Sugar", "Gaming" };
        private static readonly string[] Triggers = { "stress", "boredom", "company", "party", "fatigue" };
        private static readonly string[] Categories = { "physical", "breathing", "social", "mindfulness", "distraction" };
        private static readonly string[] Quotes = {
            "Small steps every day.", "You are stronger than your cravings.", "Progress, not perfection.", "One day at a time."
        };

        private static Random Rng = new Random();

        public static void Run(string connString, string baseDir)
        {
            using var conn = new SqliteConnection(connString);
            conn.Open();

            if (IsTableEmpty(conn, "Users"))
            {
                Console.WriteLine("[SEED] Users");
                for (int i = 0; i < SEED_COUNT; i++)
                {
                    var name = $"User{i+1}";
                    var email = $"user{i+1}@example.com";
                    var createdAt = DateTime.Today.AddDays(-Rng.Next(1, 120)).ToString("yyyy-MM-dd");
                    Exec(conn, "INSERT INTO Users(user_name,email,password,created_at) VALUES ($n,$e,$p,$c)",
                        ("$n", name), ("$e", email), ("$p", "P@ssw0rd!"), ("$c", createdAt));
                }
            }

            if (IsTableEmpty(conn, "SOSActions"))
            {
                Console.WriteLine("[SEED] SOSActions");
                for (int i = 0; i < SEED_COUNT; i++)
                {
                    var text = $"Action {i+1}: do something helpful";
                    var cat = Categories[Rng.Next(Categories.Length)];
                    Exec(conn, "INSERT INTO SOSActions(text,category) VALUES ($t,$c)",
                        ("$t", text), ("$c", cat));
                }
            }

            if (IsTableEmpty(conn, "Quotes"))
            {
                Console.WriteLine("[SEED] Quotes");
                for (int i = 0; i < SEED_COUNT; i++)
                {
                    var q = Quotes[Rng.Next(Quotes.Length)];
                    Exec(conn, "INSERT INTO Quotes(text) VALUES ($t)", ("$t", q));
                }
            }

            if (IsTableEmpty(conn, "Habits"))
            {
                Console.WriteLine("[SEED] Habits");
                var userCount = Count(conn, "Users");
                for (int i = 0; i < SEED_COUNT; i++)
                {
                    var userId = Rng.Next(1, userCount + 1);
                    var name = HabitNames[Rng.Next(HabitNames.Length)];
                    var startDate = DateTime.Today.AddDays(-Rng.Next(1, 100)).ToString("yyyy-MM-dd");
                    var goalDays = Rng.Next(7, 61);
                    var motivation = Rng.Next(0, 2) == 0 ? (string?)null : "Be better.";
                    var isActive = Rng.Next(0, 2) == 0 ? 0 : 1;
                    var totalDays = Rng.Next(0, 100);
                    var streak = Rng.Next(0, 30);
                    var saving = Math.Round((decimal)Rng.NextDouble() * 500, 2);
                    Exec(conn, @"INSERT INTO Habits(user_id,habit_name,start_date,goal_days,motivation,is_active,total_days,current_streak,total_saving)
                                 VALUES ($u,$n,$sd,$g,$m,$a,$td,$cs,$ts)",
                        ("$u", userId), ("$n", name), ("$sd", startDate), ("$g", goalDays),
                        ("$m", (object?)motivation ?? DBNull.Value), ("$a", isActive),
                        ("$td", totalDays), ("$cs", streak), ("$ts", saving));
                }
            }

            if (IsTableEmpty(conn, "DailyStatuses"))
            {
                Console.WriteLine("[SEED] DailyStatuses");
                var habitCount = Count(conn, "Habits");
                for (int i = 0; i < SEED_COUNT; i++)
                {
                    var habitId = Rng.Next(1, habitCount + 1);
                    var date = DateTime.Today.AddDays(-Rng.Next(0, 60)).ToString("yyyy-MM-dd");
                    var isClean = Rng.Next(0, 2);
                    var trigger = Triggers[Rng.Next(Triggers.Length)];
                    var note = Rng.Next(0, 2) == 0 ? (string?)null : "Note...";
                    var craving = Rng.Next(0, 11);
                    Exec(conn, @"INSERT OR IGNORE INTO DailyStatuses(habit_id,date,is_clean,trigger,note,craving_level)
                                 VALUES ($h,$d,$c,$t,$n,$cl)",
                        ("$h", habitId), ("$d", date), ("$c", isClean), ("$t", trigger),
                        ("$n", (object?)note ?? DBNull.Value), ("$cl", craving));
                }
            }

            if (IsTableEmpty(conn, "Savings"))
            {
                Console.WriteLine("[SEED] Savings");
                var habitCount = Count(conn, "Habits");
                for (int i = 0; i < SEED_COUNT; i++)
                {
                    var habitId = Rng.Next(1, habitCount + 1);
                    var amount = Math.Round((decimal)Rng.NextDouble() * 100, 2);
                    Exec(conn, "INSERT INTO Savings(habit_id,amount) VALUES ($h,$a)",
                        ("$h", habitId), ("$a", amount));
                }
            }

            if (IsTableEmpty(conn, "Achievements"))
            {
                Console.WriteLine("[SEED] Achievements");
                var userCount = Count(conn, "Users");
                for (int i = 0; i < SEED_COUNT; i++)
                {
                    var userId = Rng.Next(1, userCount + 1);
                    var title = $"Streak {Rng.Next(3, 31)} days";
                    var achievedAt = DateTime.Today.AddDays(-Rng.Next(1, 90)).ToString("yyyy-MM-dd");
                    Exec(conn, "INSERT INTO Achievements(user_id,title,achieved_at) VALUES ($u,$t,$d)",
                        ("$u", userId), ("$t", title), ("$d", achievedAt));
                }
            }

            if (IsTableEmpty(conn, "UserSOSLogs"))
            {
                Console.WriteLine("[SEED] UserSOSLogs");
                var userCount = Count(conn, "Users");
                var actionCount = Count(conn, "SOSActions");
                for (int i = 0; i < SEED_COUNT; i++)
                {
                    var userId = Rng.Next(1, userCount + 1);
                    var actionId = Rng.Next(1, actionCount + 1);
                    var date = DateTime.Today.AddDays(-Rng.Next(0, 60)).ToString("yyyy-MM-dd");
                    var worked = Rng.Next(0, 2);
                    Exec(conn, "INSERT INTO UserSOSLogs(user_id,action_id,date,worked) VALUES ($u,$a,$d,$w)",
                        ("$u", userId), ("$a", actionId), ("$d", date), ("$w", worked));
                }
            }

            Console.WriteLine("[SEED] Done.");
        }

        public static void PrintSamples(string connString)
        {
            using var conn = new SqliteConnection(connString);
            conn.Open();
            PrintTop5(conn, "Users", "user_id, user_name, email, created_at");
            PrintTop5(conn, "Habits", "habit_id, user_id, habit_name, start_date, goal_days");
            PrintTop5(conn, "DailyStatuses", "status_id, habit_id, date, is_clean, trigger");
            PrintTop5(conn, "Achievements", "achievement_id, user_id, title, achieved_at");
            PrintTop5(conn, "SOSActions", "action_id, text, category");
            PrintTop5(conn, "UserSOSLogs", "log_id, user_id, action_id, date, worked");
            PrintTop5(conn, "Savings", "saving_id, habit_id, amount");
            PrintTop5(conn, "Quotes", "quote_id, text");
        }

        private static bool IsTableEmpty(SqliteConnection conn, string table)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT CASE WHEN EXISTS(SELECT 1 FROM {table} LIMIT 1) THEN 0 ELSE 1 END;";
            return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
        }

        private static int Count(SqliteConnection conn, string table)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT COUNT(*) FROM {table};";
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private static void Exec(SqliteConnection conn, string sql, params (string, object)[] p)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            foreach (var (k, v) in p)
            {
                cmd.Parameters.AddWithValue(k, v);
            }
            cmd.ExecuteNonQuery();
        }

        private static void PrintTop5(SqliteConnection conn, string table, string cols)
        {
            Console.WriteLine($"\n=== {table} (top 5) ===");
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT {cols} FROM {table} LIMIT 5;";
            using var r = cmd.ExecuteReader();
            var schema = r.GetColumnSchema();
            while (r.Read())
            {
                var parts = new System.Collections.Generic.List<string>();
                for (int i = 0; i < r.FieldCount; i++)
                {
                    parts.Add($"{schema[i].ColumnName}={r.GetValue(i)}");
                }
                Console.WriteLine(string.Join(" | ", parts));
            }
        }
    }
}
