using System;
using System.IO;

namespace BreakFree.ConsoleSeed
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var baseDir = AppContext.BaseDirectory;

            // Try to locate schema relative to repo root
            string schemaPath;
            var repoDbPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "db", "create_breakfree.sql"));
            if (File.Exists(repoDbPath))
                schemaPath = repoDbPath;
            else
                schemaPath = Path.Combine(baseDir, "create_breakfree.sql"); // fallback

            Console.WriteLine("[BreakFree] Console ADO.NET utility");
            Console.WriteLine($"[INFO] DB file: {SqliteHelper.DbPath}");
            Console.WriteLine($"[INFO] Schema:  {schemaPath}");

            SqliteHelper.EnsureDatabase(schemaPath);
            Seed.Run(SqliteHelper.ConnectionString, baseDir);
            Seed.PrintSamples(SqliteHelper.ConnectionString);

            Console.WriteLine("\nDone.");
        }
    }
}
