using BreakFree.BLL.Services;
using BreakFree.DAL;
using BreakFree.DAL.Entities;
using BreakFree.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

public class HabitServiceTests
{
    [Fact]
    public void AddHabitShouldAddHabit()
    {
        using var context = this.GetInMemoryContext();
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
        using var context = this.GetInMemoryContext();
        context.Habits.AddRange(
            new Habit { UserId = 1, HabitName = "Run", StartDate = DateTime.Today, DailyGoal = 5, Motivation = "Health", IsActive = true },
            new Habit { UserId = 2, HabitName = "Read", StartDate = DateTime.Today, DailyGoal = 1, Motivation = "Knowledge", IsActive = true });
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
        using var context = this.GetInMemoryContext();
        var repository = new HabitRepository(context);
        var service = new HabitService(repository);

        service.AddHabit(1, "Run", DateTime.Today, 5, "Health");
        service.AddHabit(1, "Read", DateTime.Today, 1, "Knowledge");

        var habits = service.GetUserHabits(1);
        Assert.Equal(2, habits.Count);
        Assert.Contains(habits, h => h.HabitName == "Run");
        Assert.Contains(habits, h => h.HabitName == "Read");
    }

    [Fact]
    public void UpdateHabit_ShouldUpdateHabit()
    {
        using var context = this.GetInMemoryContext();
        var repository = new HabitRepository(context);
        var service = new HabitService(repository);

        var habit = new Habit { HabitId = 1, UserId = 1, HabitName = "Test", DailyGoal = 5 };
        repository.AddHabit(habit);

        habit.HabitName = "Updated Name";
        service.UpdateHabit(habit);

        var updatedHabit = repository.GetHabitsByUser(1).First();
        Assert.Equal("Updated Name", updatedHabit.HabitName);
    }

    [Fact]
    public void DeleteHabit_ShouldRemoveHabit()
    {
        using var context = this.GetInMemoryContext();
        var repository = new HabitRepository(context);
        var service = new HabitService(repository);

        var habit = new Habit { HabitId = 1, UserId = 1, HabitName = "Test" };
        repository.AddHabit(habit);

        service.DeleteHabit(habit.HabitId);

        var habits = repository.GetHabitsByUser(1);
        Assert.Empty(habits);
    }

    private BreakFreeContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<BreakFreeContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new BreakFreeContext(options);
    }
}
