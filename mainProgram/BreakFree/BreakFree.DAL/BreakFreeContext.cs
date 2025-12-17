namespace BreakFree.DAL
{
    using BreakFree.DAL.Entities;
    using Microsoft.EntityFrameworkCore;

    public class BreakFreeContext : DbContext
    {
        public BreakFreeContext()
        {
        }

        public BreakFreeContext(DbContextOptions<BreakFreeContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Habit> Habits { get; set; }

        public DbSet<DailyStatus> DailyStatuses { get; set; }

        public DbSet<Quote> Quotes { get; set; }

        public DbSet<SOSAction> SOSActions { get; set; }

        public DbSet<UserSOSLog> UserSOSLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=breakfree.db");
            }
        }
    }
}
