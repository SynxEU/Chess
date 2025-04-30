using ChessAPI.Models;

namespace ChessAPI.DataCollect.API;

public static class LoadData
{
    public static async Task<ChessPlayer> GetPlayerFromDB(ChessDbContext context, string selectedUsername)
    {
        var player = await ProcessData.ProcessAndSaveFilteredPlayerData(context, selectedUsername);
        
        return player;
    }


    public static async Task<Stats> GetStatsFromDB(ChessDbContext context, ChessPlayer player)
    {
        var stats = await ProcessData.ProcessAndSaveFilteredStatsData(context, player.username);
        
        return stats;
    }
}