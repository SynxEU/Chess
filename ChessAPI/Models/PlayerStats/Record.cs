namespace ChessAPI.Models;

public class Record
{
    public int win { get; set; }
    public int loss { get; set; }
    public int draw { get; set; }
    public int time_per_move { get; set; }
    public int timeout_percent { get; set; }
}