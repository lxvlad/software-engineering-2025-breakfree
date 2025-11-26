using Xunit;
using BreakFree.BLL.Services;
using BreakFree.DAL.Repositories;
using BreakFree.DAL.Entities;
using BreakFree.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

public class HabitServiceTests
{
    // Метод для створення ізольованої InMemory бази
    private BreakFreeContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<BreakFreeContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // унікальна база на кожен тест
            .Options;

        return new BreakFreeContext(options);
    }

    [Fact]
    public void AddHabit_ShouldAddHabit()
    {
        using var context = GetInMemoryContext();
        var repository = new HabitRepository(context);
        var service = new HabitService(repository);

        service.AddHabit(1, "Run", DateTime.Today, 5, "Stay healthy");

        var habit = context.Habits.FirstOrDefault();
        Assert.NotNull(habit);
        Assert.Equal("Run", habit.HabitName);
        Assert.Equal(1, habit.UserId);
        Assert.Equal(5, habit.DailyGoal);
        Assert.Equal("Stay healthy", habit.Motivation);
        Assert.True(habit.IsActive);
    }

    [Fact]
    public void GetUserHabits_ShouldReturnOnlyUserHabits()
    {
        using var context = GetInMemoryContext();
        context.Habits.AddRange(
            new Habit { UserId = 1, HabitName = "Run", StartDate = DateTime.Today, DailyGoal = 5, Motivation = "Health", IsActive = true },
            new Habit { UserId = 2, HabitName = "Read", StartDate = DateTime.Today, DailyGoal = 1, Motivation = "Knowledge", IsActive = true }
        );
        context.SaveChanges();

        var repository = new HabitRepository(context);
        var service = new HabitService(repository);

        var user1Habits = service.GetUserHabits(1);
        Assert.Single(user1Habits);
        Assert.Equal("Run", user1Habits[0].HabitName);

        var user2Habits = service.GetUserHabits(2);
        Assert.Single(user2Habits);
        Assert.Equal("Read", user2Habits[0].HabitName);
    }

    [Fact]
    public void AddMultipleHabits_ForSameUser_ShouldAddAll()
    {
        using var context = GetInMemoryContext();
        var repository = new HabitRepository(context);
        var service = new HabitService(repository);

        service.AddHabit(1, "Run", DateTime.Today, 5, "Health");
        service.AddHabit(1, "Read", DateTime.Today, 1, "Knowledge");

        var habits = service.GetUserHabits(1);
        Assert.Equal(2, habits.Count);
        Assert.Contains(habits, h => h.HabitName == "Run");
        Assert.Contains(habits, h => h.HabitName == "Read");
    }
}
