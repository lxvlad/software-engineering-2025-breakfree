namespace BreakFree.DAL.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using BreakFree.DAL.Entities;

    public class DailyStatusRepository
    {
        private readonly BreakFreeContext context;

        public DailyStatusRepository(BreakFreeContext context)
        {
            this.context = context;
        }

        public void AddDailyStatus(DailyStatus status)
        {
            this.context.DailyStatuses.Add(status);
            this.context.SaveChanges();
        }

        public List<DailyStatus> GetStatusesByHabit(int habitId)
        {
            return this.context.DailyStatuses.Where(s => s.HabitId == habitId).ToList();
        }

        public void UpdateDailyStatus(DailyStatus status)
        {
            this.context.DailyStatuses.Update(status);
            this.context.SaveChanges();
        }

        public void DeleteDailyStatus(int statusId)
        {
            var status = this.context.DailyStatuses.FirstOrDefault(s => s.StatusId == statusId);
            if (status != null)
            {
                this.context.DailyStatuses.Remove(status);
                this.context.SaveChanges();
            }
        }

        public List<DailyStatus> GetStatusesByUser(int userId)
        {
            return this.context.DailyStatuses
                .Where(s => this.context.Habits.Any(h => h.HabitId == s.HabitId && h.UserId == userId))
                .ToList();
        }
    }
}
