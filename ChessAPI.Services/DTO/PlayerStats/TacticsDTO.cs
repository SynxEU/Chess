using ChessAPI.Services.DTO.Scale;

namespace ChessAPI.Services.DTO.PlayerStats;

public class TacticsDTO
{
    public HighestDTO Highest { get; set; }
    public LowestDTO Lowest { get; set; }
}