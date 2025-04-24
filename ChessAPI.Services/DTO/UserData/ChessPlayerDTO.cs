using ChessAPI.Services.DTO.PlayerStats;

namespace ChessAPI.Services.DTO.UserData;

public class ChessPlayerDTO
{
    public int ChessId { get; set; }
    public string? Avatar { get; set; }
    public int playerId { get; set; }
    public string? Url { get; set; }
    public string? Name { get; set; }
    public string? Username { get; set; }
    public int Followers { get; set; }
    public string? Country { get; set; }
    public string? Location { get; set; }
    public int LastOnline { get; set; }
    public int Joined { get; set; }
    public string Status { get; set; }
    public bool IsStreamer { get; set; }
    public bool Verified { get; set; }
    public string? League { get; set; }
    public DateOnly? FetchedAtDate { get; set; }
    public TimeSpan? FetchedAtTime { get; set; }
    public List<StreamingPlatformDTO> StreamingPlatforms { get; set; }
    public List<PlayerStatsDTO> Stats { get; set; }
}
