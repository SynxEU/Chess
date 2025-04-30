using ChessAPI.Services.DTO.Modes;

namespace ChessAPI.Services.DTO.PlayerStats;

public class PlayerStatsDTO
{
    public int ChessId { get; set; }
    public ChessDailyDTO ChessDaily { get; set; }
    public ChessRapidDTO ChessRapid { get; set; }
    public ChessBulletDTO ChessBullet { get; set; }
    public ChessBlitzDTO ChessBlitz { get; set; }
    public int Fide { get; set; }
    public TacticsDTO Tactics { get; set; }
    public DateOnly FetchedAtDate { get; set; }
    public TimeSpan FetchedAtTime { get; set; }
}