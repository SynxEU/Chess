namespace ChessAPI.Models;

public class ChessDaily
{
    public int id { get; set; }
    public Last last { get; set; }
    public Best best { get; set; }
    public Record record { get; set; }
}