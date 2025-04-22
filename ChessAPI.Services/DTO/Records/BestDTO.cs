namespace ChessAPI.Services.DTO.Records;

public class BestDTO
{
    public int Rating { get; set; }
    public int Date { get; set; }
    public string Game { get; set; }
    public int TotalAttempts { get; set; }
    public int Score { get; set; } 
}