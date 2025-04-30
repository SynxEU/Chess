using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class DbContextFactory : IDesignTimeDbContextFactory<ChessDbContext>
{
    public ChessDbContext CreateDbContext(string[] args)
    {
        var envPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "config.env");
        var fullPath = Path.GetFullPath(envPath);
        Env.Load(fullPath);
        
        var optionsBuilder = new DbContextOptionsBuilder<ChessDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("SSMS");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("The environment variable 'SSMS' is not set.");
        }

        optionsBuilder.UseSqlServer(connectionString);
        
        return new ChessDbContext(optionsBuilder.Options);
    }
}



