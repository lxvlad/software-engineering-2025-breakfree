using Microsoft.EntityFrameworkCore;
using System.IO;

namespace BreakFree.Core.Data
{
    public class BreakFreeContext : DbContext
    {
        public static readonly string ConnectionString = $"Data Source={DbPath}";
        private static string DbPath
        {
            get
            {
                var baseDir = Directory.GetCurrentDirectory();
                return Path.Combine(baseDir, "breakfree.db");
            }
        }

        public DbSet<BreakFree.Core.Models.Habit> Habits => Set<BreakFree.Core.Models.Habit>();
        public DbSet<BreakFree.Core.Models.Slip> Slips => Set<BreakFree.Core.Models.Slip>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BreakFree.Core.Models.Habit>()
                .Property(h => h.Name).HasMaxLength(200).IsRequired();

            modelBuilder.Entity<BreakFree.Core.Models.Slip>()
                .HasIndex(s => s.HabitId);
        }
    }
}
