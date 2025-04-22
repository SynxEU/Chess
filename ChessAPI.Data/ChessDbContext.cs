using ChessAPI.Models;
using Microsoft.EntityFrameworkCore;

public class ChessDbContext : DbContext
{
    public ChessDbContext(DbContextOptions<ChessDbContext> options) : base(options) { }

    public DbSet<ChessPlayer> ChessPlayers { get; set; }
    public DbSet<ChessDaily> ChessDailies { get; set; }
    public DbSet<Stats> Stats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure ChessBlitz as an owned entity
        modelBuilder.Entity<ChessBlitz>()
            .OwnsOne(c => c.last);
        modelBuilder.Entity<ChessBlitz>()
            .OwnsOne(c => c.best);
        modelBuilder.Entity<ChessBlitz>()
            .OwnsOne(c => c.record);

        // Configure ChessBullet as an owned entity
        modelBuilder.Entity<ChessBullet>()
            .OwnsOne(c => c.last);
        modelBuilder.Entity<ChessBullet>()
            .OwnsOne(c => c.best);
        modelBuilder.Entity<ChessBullet>()
            .OwnsOne(c => c.record);

        // ChessDaily is now a regular entity and should not be owned
        modelBuilder.Entity<ChessDaily>()
            .HasKey(c => c.id); // Primary key for ChessDaily
        modelBuilder.Entity<ChessDaily>()
            .OwnsOne(c => c.last);
        modelBuilder.Entity<ChessDaily>()
            .OwnsOne(c => c.best);
        modelBuilder.Entity<ChessDaily>()
            .OwnsOne(c => c.record);

        // Configure ChessRapid as an owned entity
        modelBuilder.Entity<ChessRapid>()
            .OwnsOne(c => c.last);
        modelBuilder.Entity<ChessRapid>()
            .OwnsOne(c => c.best);
        modelBuilder.Entity<ChessRapid>()
            .OwnsOne(c => c.record);

        // Configure Tactics as an owned entity
        modelBuilder.Entity<Tactics>()
            .OwnsOne(t => t.highest);
        modelBuilder.Entity<Tactics>()
            .OwnsOne(t => t.lowest);

        // Configure ChessPlayer and its streaming platforms
        modelBuilder.Entity<ChessPlayer>()
            .OwnsMany(p => p.streaming_platforms);

        // Configure ChessPlayer's index
        modelBuilder.Entity<ChessPlayer>()
            .HasIndex(p => new { p.username, p.FetchedAt })
            .IsUnique(false);

        // Configure the Stats entity and its relationship with ChessPlayer
        modelBuilder.Entity<Stats>()
            .HasOne(s => s.ChessPlayer)
            .WithMany(p => p.Stats) // One ChessPlayer can have many Stats
            .HasForeignKey(s => s.ChessPlayerId);

        // Configure the CreatedAt timestamp for Stats
        modelBuilder.Entity<Stats>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()"); // Automatically set to the current date/time
    }

}
