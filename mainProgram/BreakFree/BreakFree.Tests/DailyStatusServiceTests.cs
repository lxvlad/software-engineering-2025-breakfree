using BreakFree.BLL.Services;
using BreakFree.DAL;
using BreakFree.DAL.Entities;
using BreakFree.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

public class DailyStatusServiceTests
{
    [Fact]
    public void AddDailyStatusShouldAddStatus()
    {
        using var context = this.GetInMemoryContext();
        var repository = new DailyStatusRepository(context);
        var service = new DailyStatusService(repository);

        var status = new DailyStatus
        {
            HabitId = 1,
            DateTime = DateTime.Today,
            Trigger = "Test trigger",
            Note = "Test note",
            CravingLevel = 3,
        };

        service.AddDailyStatus(status);

        var savedStatus = context.DailyStatuses.FirstOrDefault();
        Assert.NotNull(savedStatus);
        Assert.Equal(1, savedStatus.HabitId);
        Assert.Equal("Test trigger", savedStatus.Trigger);
        Assert.Equal("Test note", savedStatus.Note);
        Assert.Equal(3, savedStatus.CravingLevel);
    }

    [Fact]
    public void GetStatusesByHabit_ShouldReturnOnlyHabitStatuses()
    {
        using var context = this.GetInMemoryContext();
        context.DailyStatuses.AddRange(
            new DailyStatus { HabitId = 1, DateTime = DateTime.Today },
            new DailyStatus { HabitId = 2, DateTime = DateTime.Today });
        context.SaveChanges();

        var repository = new DailyStatusRepository(context);
        var service = new DailyStatusService(repository);

        var habit1Statuses = service.GetStatusesByHabit(1);
        Assert.Single(habit1Statuses);
        Assert.Equal(1, habit1Statuses[0].HabitId);

        var habit2Statuses = service.GetStatusesByHabit(2);
        Assert.Single(habit2Statuses);
        Assert.Equal(2, habit2Statuses[0].HabitId);
    }

    [Fact]
    public void GetStatusesByUser_ShouldReturnStatusesForUser()
    {
        using var context = this.GetInMemoryContext();

        context.Habits.AddRange(
            new Habit { HabitId = 1, UserId = 1 },
            new Habit { HabitId = 2, UserId = 2 });

        context.DailyStatuses.AddRange(
            new DailyStatus { HabitId = 1, DateTime = DateTime.Today },
            new DailyStatus { HabitId = 2, DateTime = DateTime.Today });

        context.SaveChanges();

        var repository = new DailyStatusRepository(context);
        var service = new DailyStatusService(repository);

        var user1Statuses = service.GetStatusesByUser(1);
        Assert.Single(user1Statuses);
        Assert.Equal(1, user1Statuses[0].HabitId);

        var user2Statuses = service.GetStatusesByUser(2);
        Assert.Single(user2Statuses);
        Assert.Equal(2, user2Statuses[0].HabitId);
    }

    [Fact]
    public void UpdateDailyStatus_ShouldUpdateStatus()
    {
        using var context = this.GetInMemoryContext();
        var status = new DailyStatus
        {
            HabitId = 1,
            DateTime = DateTime.Today,
            Trigger = "Original",
            Note = "Original note",
            CravingLevel = 2,
        };
        context.DailyStatuses.Add(status);
        context.SaveChanges();

        var repository = new DailyStatusRepository(context);
        var service = new DailyStatusService(repository);

        status.Trigger = "Updated";
        status.CravingLevel = 5;
        service.UpdateDailyStatus(status);
        var updatedStatus = context.DailyStatuses.FirstOrDefault();
        Assert.NotNull(updatedStatus);
        Assert.Equal("Updated", updatedStatus.Trigger);
        Assert.Equal(5, updatedStatus.CravingLevel);
    }

    [Fact]
    public void DeleteDailyStatus_ShouldRemoveStatus()
    {
        using var context = this.GetInMemoryContext();
        var status = new DailyStatus { HabitId = 1, DateTime = DateTime.Today };
        context.DailyStatuses.Add(status);
        context.SaveChanges();

        var repository = new DailyStatusRepository(context);
        var service = new DailyStatusService(repository);

        service.DeleteDailyStatus(status.StatusId);

        Assert.Empty(context.DailyStatuses.ToList());
    }

    [Fact]
    public void DefaultConstructor_ShouldCreateRepository()
    {
        var service = new DailyStatusService();
        Assert.NotNull(service);
    }

    private BreakFreeContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<BreakFreeContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new BreakFreeContext(options);
    }
}
