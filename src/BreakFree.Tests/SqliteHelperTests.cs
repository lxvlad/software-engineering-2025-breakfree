using Xunit;
using BreakFree.ConsoleSeed;
using System.IO;

namespace BreakFree.Tests
{
    public class SqliteHelperTests
    {
        [Fact]
        public void EnsureDatabase_CreatesDbFile()
        {
            string tempDbPath = Path.Combine(Path.GetTempPath(), "test_breakfree.db");
            if (File.Exists(tempDbPath)) File.Delete(tempDbPath);

            var sqlSchemaPath = Path.Combine(AppContext.BaseDirectory, "create_breakfree.sql");
            SqliteHelper.EnsureDatabase(sqlSchemaPath);

            Assert.True(File.Exists(SqliteHelper.DbPath));
        }
    }
}
