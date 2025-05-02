using System.Data.Entity;
using ChessAPI.Models;
using ChessAPI.Services.DTO.PlayerStats;
using ChessAPI.Services.DTO.UserData;
using ChessAPI.Services.Extenstions;

namespace ChessAPI.Services.Services;

public static class GetHistoricalData
{
    public static async Task<List<PlayerStatsDTO>> GetHistoryStatsFromDB(ChessDbContext context, ChessPlayer player)
    {
        List<Stats> statsFromDb= context.Stats
            .Where(s => s.ChessId == player.ChessId)
            .OrderBy(s => s.FetchedAtDate)
            .ThenBy(s => s.FetchedAtTime)
            .ToList();

        List<PlayerStatsDTO> stats = new List<PlayerStatsDTO>();
        
        foreach (Stats stat in statsFromDb)
            stats.Add(stat.ToDTO());
        
        return stats;
    }

    public static async Task<List<ChessPlayerDTO>> GetLatestPlayersAsync(ChessDbContext context)
    {
        List<ChessPlayer> players = context.ChessPlayers
            .OrderByDescending(p => p.FetchedAtDate)
            .ThenByDescending(p => p.FetchedAtTime) 
            .ToList();

        List<ChessPlayer> latestPlayers = players
            .GroupBy(p => p.username)
            .Select(g => g.OrderByDescending(p => p.FetchedAtDate) 
                .ThenByDescending(p => p.FetchedAtTime) 
                .FirstOrDefault()) 
            .OrderBy(p => p.username) 
            .ToList();

        List<ChessPlayerDTO> playersDTO = new List<ChessPlayerDTO>();
    
        foreach (ChessPlayer player in latestPlayers)
            playersDTO.Add(player.ToDTO());

        return playersDTO;
    }




}