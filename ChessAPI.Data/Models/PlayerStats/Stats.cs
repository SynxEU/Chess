namespace ChessAPI.Models;

public class Stats
{
    public int id { get; set; }
        
    public int ChessId { get; set; }
    public ChessPlayer ChessPlayer { get; set; }

    public ChessDaily? chess_daily { get; set; }
    public ChessRapid? chess_rapid { get; set; }
    public ChessBullet? chess_bullet { get; set; }
    public ChessBlitz? chess_blitz { get; set; }
    public int fide { get; set; }
    public Tactics? tactics { get; set; }

    public DateOnly FetchedAtDate { get; set; }
    public TimeSpan FetchedAtTime { get; set; }
}