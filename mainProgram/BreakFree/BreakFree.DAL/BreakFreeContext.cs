using Microsoft.EntityFrameworkCore;
using BreakFree.DAL.Entities;

namespace BreakFree.DAL
{
    public class BreakFreeContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Habit> Habits { get; set; }
        public DbSet<DailyStatus> DailyStatuses { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<SOSAction> SOSActions { get; set; }
        public DbSet<UserSOSLog> UserSOSLogs { get; set; }

        public BreakFreeContext(DbContextOptions<BreakFreeContext> options) : base(options) { }

        public BreakFreeContext() { } 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=breakfree.db");
            }
        }
    }
}
