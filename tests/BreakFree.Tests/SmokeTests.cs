using Xunit;
using BreakFree.Core.Data;

namespace BreakFree.Tests
{
    public class SmokeTests
    {
        [Fact]
        public void DbContext_UsesSqlite()
        {
            var ctx = new BreakFreeContext();
            Assert.Contains("Sqlite", ctx.Database.ProviderName!);
        }
    }
}
