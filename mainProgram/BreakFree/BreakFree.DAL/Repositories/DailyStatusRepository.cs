using BreakFree.DAL.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BreakFree.DAL.Repositories
{
    public class DailyStatusRepository
    {
        private readonly BreakFreeContext _context;

        public DailyStatusRepository(BreakFreeContext context)
        {
            _context = context;
        }

        public void AddDailyStatus(DailyStatus status)
        {
            _context.DailyStatuses.Add(status);
            _context.SaveChanges();
        }

        public List<DailyStatus> GetStatusesByHabit(int habitId)
        {
            return _context.DailyStatuses.Where(s => s.HabitId == habitId).ToList();
        }

        public void UpdateDailyStatus(DailyStatus status)
        {
            _context.DailyStatuses.Update(status);
            _context.SaveChanges();
        }

        public void DeleteDailyStatus(int statusId)
        {
            var status = _context.DailyStatuses.FirstOrDefault(s => s.StatusId == statusId);
            if (status != null)
            {
                _context.DailyStatuses.Remove(status);
                _context.SaveChanges();
            }
        }

        public List<DailyStatus> GetStatusesByUser(int userId)
        {
            return _context.DailyStatuses
                .Where(s => _context.Habits.Any(h => h.HabitId == s.HabitId && h.UserId == userId))
                .ToList();
        }
    }
}
