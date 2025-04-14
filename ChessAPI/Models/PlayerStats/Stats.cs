namespace ChessAPI.Models;

public class Stats
{
    public ChessDaily chess_daily { get; set; }
    public Chess960Daily chess960_daily { get; set; }
    public ChessRapid chess_rapid { get; set; }
    public ChessBullet chess_bullet { get; set; }
    public ChessBlitz chess_blitz { get; set; }
    public int fide { get; set; }
    public Tactics tactics { get; set; }
    public PuzzleRush puzzle_rush { get; set; }
}