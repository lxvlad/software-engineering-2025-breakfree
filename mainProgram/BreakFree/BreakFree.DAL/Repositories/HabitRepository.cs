namespace BreakFree.DAL.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using BreakFree.DAL.Entities;

    public class HabitRepository
    {
        private readonly BreakFreeContext context;

        public HabitRepository(BreakFreeContext? injectedContext = null)
        {
            this.context = injectedContext ?? new BreakFreeContext();
        }

        public void AddHabit(Habit habit)
        {
            this.context.Habits.Add(habit);
            this.context.SaveChanges();
        }

        public List<Habit> GetHabitsByUser(int userId)
        {
            return this.context.Habits.Where(h => h.UserId == userId).ToList();
        }

        public void DeleteHabit(int habitId)
        {
            var habit = this.context.Habits.FirstOrDefault(h => h.HabitId == habitId);
            if (habit != null)
            {
                this.context.Habits.Remove(habit);
                this.context.SaveChanges();
            }
        }

        public void UpdateHabit(Habit habit)
        {
            this.context.Habits.Update(habit);
            this.context.SaveChanges();
        }
    }
}
