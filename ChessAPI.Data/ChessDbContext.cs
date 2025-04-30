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

        modelBuilder.Entity<ChessBlitz>()
            .OwnsOne(c => c.last);
        modelBuilder.Entity<ChessBlitz>()
            .OwnsOne(c => c.best);
        modelBuilder.Entity<ChessBlitz>()
            .OwnsOne(c => c.record);

        modelBuilder.Entity<ChessBullet>()
            .OwnsOne(c => c.last);
        modelBuilder.Entity<ChessBullet>()
            .OwnsOne(c => c.best);
        modelBuilder.Entity<ChessBullet>()
            .OwnsOne(c => c.record);

        modelBuilder.Entity<ChessDaily>()
            .HasKey(c => c.id);
        modelBuilder.Entity<ChessDaily>()
            .OwnsOne(c => c.last);
        modelBuilder.Entity<ChessDaily>()
            .OwnsOne(c => c.best);
        modelBuilder.Entity<ChessDaily>()
            .OwnsOne(c => c.record);

        modelBuilder.Entity<ChessRapid>()
            .OwnsOne(c => c.last);
        modelBuilder.Entity<ChessRapid>()
            .OwnsOne(c => c.best);
        modelBuilder.Entity<ChessRapid>()
            .OwnsOne(c => c.record);

        modelBuilder.Entity<Tactics>()
            .OwnsOne(t => t.highest);
        modelBuilder.Entity<Tactics>()
            .OwnsOne(t => t.lowest);

        modelBuilder.Entity<ChessPlayer>()
            .OwnsMany(p => p.streaming_platforms);

        modelBuilder.Entity<ChessPlayer>()
            .HasIndex(p => new { p.username, p.FetchedAtDate, p.FetchedAtTime })
            .IsUnique(false);

        modelBuilder.Entity<Stats>()
            .HasOne(s => s.ChessPlayer)
            .WithMany(p => p.Stats)
            .HasForeignKey(s => s.ChessId);
        
        modelBuilder.Entity<ChessPlayer>()
            .HasIndex(p => new { p.ChessId, p.FetchedAtDate, p.FetchedAtTime })
            .IsUnique(false);
        
        modelBuilder.Entity<ChessPlayer>()
            .Property(e => e.FetchedAtTime)
            .HasColumnType("time(0)");
        
        modelBuilder.Entity<ChessPlayer>()
            .Property(e => e.UpdatedAtTime)
            .HasColumnType("time(0)");
        
        modelBuilder.Entity<Stats>()
            .Property(e => e.FetchedAtTime)
            .HasColumnType("time(0)");
    }

}
