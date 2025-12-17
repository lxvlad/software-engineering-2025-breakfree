namespace BreakFree.BLL.Interfaces
{
    using System;
    using System.Collections.Generic;
    using BreakFree.DAL.Entities;

    public interface IHabitService
    {
        void AddHabit(int userId, string name, DateTime startDate, int goal, string motivation);

        List<Habit> GetUserHabits(int userId);

        void UpdateHabit(Habit habit);

        void DeleteHabit(int habitId);
    }
}
