using Xunit;
using BreakFree.ConsoleSeed;
using Microsoft.Data.Sqlite;

namespace BreakFree.Tests
{
    public class SeedTests
    {
        [Fact]
        public void SeedWithBogus_InsertsUsers()
        {
            var connectionString = SqliteHelper.ConnectionString;
            Seed.SeedWithBogus(connectionString);

            using var conn = new SqliteConnection(connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Users";
            var count = (long)cmd.ExecuteScalar();

            Assert.True(count > 0);
        }
    }
}
