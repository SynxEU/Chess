using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class DbContextFactory : IDesignTimeDbContextFactory<ChessDbContext>
{
    public ChessDbContext CreateDbContext(string[] args = null)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ChessDbContext>();
        var connectionString = @"Server=(LocalDB)\MSSQLLocalDB;Database=ChessDB;Trusted_Connection=True;";
        optionsBuilder.UseSqlServer(connectionString);

        return new ChessDbContext(optionsBuilder.Options);
    }
}



