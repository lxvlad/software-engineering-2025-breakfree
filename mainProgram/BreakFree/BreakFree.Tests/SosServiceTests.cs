namespace BreakFree.Tests
{
    using BreakFree.BLL.Services;
    using BreakFree.DAL;
    using BreakFree.DAL.Entities;
    using Microsoft.EntityFrameworkCore;

    public class SosServiceTests
    {
        [Fact]
        public void LogAttempt_ShouldAddLog_WhenActionExists()
        {
            var options = new DbContextOptionsBuilder<BreakFreeContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new BreakFreeContext(options);
            var action = new SOSAction { UserId = 1, Text = "A1" };
            context.SOSActions.Add(action);
            context.SaveChanges();

            var service = new SosService(context);
            service.LogAttempt(1, action.ActionId, true);

            var log = context.UserSOSLogs.FirstOrDefault();
            Assert.NotNull(log);
            Assert.True(log.Worked);
        }

        [Fact]
        public void GetSortedTips_ShouldSortByEfficiencyAndUsageCount()
        {
            var options = new DbContextOptionsBuilder<BreakFreeContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new BreakFreeContext(options);

            var action1 = new SOSAction { UserId = 1, Text = "A1" };
            var action2 = new SOSAction { UserId = 1, Text = "A2" };
            context.SOSActions.AddRange(action1, action2);
            context.SaveChanges();

            context.UserSOSLogs.Add(new UserSOSLog { UserId = 1, ActionId = action1.ActionId, Worked = true });
            context.UserSOSLogs.Add(new UserSOSLog { UserId = 1, ActionId = action2.ActionId, Worked = true });
            context.UserSOSLogs.Add(new UserSOSLog { UserId = 1, ActionId = action2.ActionId, Worked = true });
            context.SaveChanges();

            var service = new SosService(context);
            var result = service.GetSortedTips(1, skipDefaultActions: true);

            Assert.Equal(2, result.Count);
            Assert.Equal("A2", result[0].Text);
            Assert.Equal("A1", result[1].Text);
        }
    }
}
