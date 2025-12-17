namespace BreakFree.BLL.Services
{
    using System;
    using System.Collections.Generic;
    using BreakFree.BLL.Interfaces;
    using BreakFree.DAL.Entities;
    using BreakFree.DAL.Repositories;

    public class HabitService : IHabitService
    {
        private readonly HabitRepository habitRepository;

        public HabitService(HabitRepository repository)
        {
            this.habitRepository = repository;
        }

        public HabitService()
            : this(new HabitRepository())
        {
        }

        public void AddHabit(int userId, string name, DateTime startDate, int goal, string motivation)
        {
            var habit = new Habit
            {
                UserId = userId,
                HabitName = name,
                StartDate = startDate,
                DailyGoal = goal,
                Motivation = motivation,
                IsActive = true,
            };

            this.habitRepository.AddHabit(habit);
        }

        public List<Habit> GetUserHabits(int userId)
        {
            return this.habitRepository.GetHabitsByUser(userId);
        }

        public void UpdateHabit(Habit habit)
        {
            this.habitRepository.UpdateHabit(habit);
        }

        public void DeleteHabit(int habitId)
        {
            this.habitRepository.DeleteHabit(habitId);
        }
    }
}
