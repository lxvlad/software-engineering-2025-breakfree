using BreakFree.DAL.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BreakFree.DAL.Repositories
{
    public class HabitRepository
    {
        private readonly BreakFreeContext _context;

        public HabitRepository(BreakFreeContext context)
        {
            _context = context;
        }

        public void AddHabit(Habit habit)
        {
            _context.Habits.Add(habit);
            _context.SaveChanges();
        }

        public List<Habit> GetHabitsByUser(int userId)
        {
            return _context.Habits.Where(h => h.UserId == userId).ToList();
        }

        public void DeleteHabit(int habitId)
        {
            var habit = _context.Habits.FirstOrDefault(h => h.HabitId == habitId);
            if (habit != null)
            {
                _context.Habits.Remove(habit);
                _context.SaveChanges();
            }
        }

        public void UpdateHabit(Habit habit)
        {
            _context.Habits.Update(habit);
            _context.SaveChanges();
        }
    }
}
