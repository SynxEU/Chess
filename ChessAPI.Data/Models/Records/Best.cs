namespace ChessAPI.Models;

public class Best
{
    public int id { get; set; }
    public int rating { get; set; }
    public int date { get; set; }
    public string? game { get; set; }
    public int total_attempts { get; set; }
    public int score { get; set; }
}