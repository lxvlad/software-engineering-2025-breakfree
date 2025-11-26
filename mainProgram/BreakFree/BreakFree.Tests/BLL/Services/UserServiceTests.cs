using Xunit;
using BreakFree.BLL.Services;
using BreakFree.DAL;
using BreakFree.DAL.Entities;
using Microsoft.EntityFrameworkCore;

public class UserServiceTests
{
    // Метод для створення ізольованої InMemory бази
    private BreakFreeContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<BreakFreeContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // унікальна база на кожен тест
            .Options;

        return new BreakFreeContext(options);
    }

    [Fact]
    public void Register_Success_ReturnsTrue()
    {
        using var context = GetInMemoryContext();
        var service = new UserService(context);

        var result = service.Register("Alice", "alice@test.com", "123");

        Assert.True(result);
        Assert.Single(context.Users); // переконуємося, що в базі один користувач
    }

    [Fact]
    public void Register_ExistingEmail_ReturnsFalse()
    {
        using var context = GetInMemoryContext();
        context.Users.Add(new User { UserName = "Alice", Email = "alice@test.com", Password = "123" });
        context.SaveChanges();

        var service = new UserService(context);
        var result = service.Register("Bob", "alice@test.com", "456");

        Assert.False(result);
        Assert.Single(context.Users); // база не повинна додати нового користувача
    }

    [Fact]
    public void Login_ValidUser_ReturnsUser()
    {
        using var context = GetInMemoryContext();
        context.Users.Add(new User { UserName = "Alice", Email = "alice@test.com", Password = "123" });
        context.SaveChanges();

        var service = new UserService(context);
        var user = service.Login("alice@test.com", "123");

        Assert.NotNull(user);
        Assert.Equal("Alice", user!.UserName);
    }

    [Fact]
    public void Login_InvalidUser_ReturnsNull()
    {
        using var context = GetInMemoryContext();
        var service = new UserService(context);

        var user = service.Login("nonexist@test.com", "123");

        Assert.Null(user);
    }

    [Fact]
    public void ChangePassword_CorrectOldPassword_ReturnsTrue()
    {
        using var context = GetInMemoryContext();
        var user = new User { UserName = "Alice", Email = "alice@test.com", Password = "old" };
        context.Users.Add(user);
        context.SaveChanges();

        var service = new UserService(context);
        var result = service.ChangePassword(user.UserId, "old", "new");

        Assert.True(result);
        Assert.Equal("new", context.Users.First().Password);
    }

    [Fact]
    public void ChangePassword_WrongOldPassword_ReturnsFalse()
    {
        using var context = GetInMemoryContext();
        var user = new User { UserName = "Alice", Email = "alice@test.com", Password = "old" };
        context.Users.Add(user);
        context.SaveChanges();

        var service = new UserService(context);
        var result = service.ChangePassword(user.UserId, "wrong", "new");

        Assert.False(result);
        Assert.Equal("old", context.Users.First().Password);
    }

    [Fact]
    public void DeleteUser_RightPassword_DeletesUser()
    {
        using var context = GetInMemoryContext();
        var user = new User { UserName = "Alice", Email = "alice@test.com", Password = "123" };
        context.Users.Add(user);
        context.SaveChanges();

        var service = new UserService(context);
        var result = service.DeleteUser(user.UserId, "123");

        Assert.True(result);
        Assert.Empty(context.Users);
    }

    [Fact]
    public void DeleteUser_WrongPassword_ReturnsFalse()
    {
        using var context = GetInMemoryContext();
        var user = new User { UserName = "Alice", Email = "alice@test.com", Password = "123" };
        context.Users.Add(user);
        context.SaveChanges();

        var service = new UserService(context);
        var result = service.DeleteUser(user.UserId, "wrong");

        Assert.False(result);
        Assert.Single(context.Users);
    }

    [Fact]
    public void UpdateUser_Success_ReturnsTrue()
    {
        using var context = GetInMemoryContext();
        var user = new User { UserName = "OldName", Email = "alice@test.com", Password = "123" };
        context.Users.Add(user);
        context.SaveChanges();

        var service = new UserService(context);
        
        var result = service.UpdateUser(user.UserId, "NewName", "alice@test.com"); // передаємо userId, newUsername, newEmail

        Assert.True(result);
        Assert.Equal("NewName", context.Users.First().UserName);
    }
}
