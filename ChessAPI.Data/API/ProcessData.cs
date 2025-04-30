using ChessAPI.DataCollect.Helpers;
using ChessAPI.Models;
using MongoDB.Bson;

namespace ChessAPI.DataCollect.API;

public class ProcessData
{
    public static async Task<ChessPlayer> ProcessAndSaveFilteredPlayerData(ChessDbContext context)
    {
        List<ChessPlayer> playersFromMongo = await DataMongo.GetPlayersFromMongo();

        // Process and filter player data first
        foreach (ChessPlayer player in playersFromMongo)
        {
            var existingPlayer = context.ChessPlayers
                .FirstOrDefault(p =>
                    p.username == player.username &&
                    p.is_streamer == player.is_streamer &&
                    p.verified == player.verified &&
                    p.country == player.country &&
                    p.player_id == player.player_id &&
                    p.apiURL == player.apiURL &&
                    p.followers == player.followers &&
                    p.joined == player.joined &&
                    p.last_online == player.last_online &&
                    p.league == player.league &&
                    p.status == player.status &&
                    p.url == player.url &&
                    p.avatar == player.avatar);

            if (existingPlayer != null)
            {
                bool hasSameStreamingPlatform = player.streaming_platforms != null &&
                                                existingPlayer.streaming_platforms != null &&
                                                existingPlayer.streaming_platforms.Any(existingPlatform =>
                                                    player.streaming_platforms.Any(newPlatform =>
                                                        newPlatform.channel_url == existingPlatform.channel_url));

                if (hasSameStreamingPlatform)
                {
                    foreach (StreamingPlatform newPlatform in player.streaming_platforms)
                    {
                        StreamingPlatform existingPlatform = existingPlayer.streaming_platforms
                            .FirstOrDefault(p => p.channel_url == newPlatform.channel_url);

                        if (existingPlatform != null)
                        {
                            existingPlatform.Weight += 1;
                        }
                    }

                    existingPlayer.Weight += 1;
                    existingPlayer.UpdatedAtDate = DateOnly.FromDateTime(DateTime.UtcNow);
                    existingPlayer.UpdatedAtTime = TimeOnly.FromDateTime(DateTime.UtcNow).ToTimeSpan();

                    await context.SaveChangesAsync();
                    Console.WriteLine($"Updated weight for {player.username} with streaming platform.");
                }
                else
                {
                    existingPlayer.Weight += 1;
                    existingPlayer.UpdatedAtDate = DateOnly.FromDateTime(DateTime.UtcNow);
                    existingPlayer.UpdatedAtTime = TimeOnly.FromDateTime(DateTime.UtcNow).ToTimeSpan();

                    await context.SaveChangesAsync();
                    Console.WriteLine($"Updated weight for {player.username} without streaming platform.");
                }
            }
            else
            {
                context.ChessPlayers.Add(player);
                await context.SaveChangesAsync();
                Console.WriteLine($"Added new player: {player.username}");
            }
        }
        return playersFromMongo.FirstOrDefault();
    }

    public static async Task<Stats> ProcessAndSaveFilteredStatsData(ChessDbContext context)
    {
        List<Stats> statsFromMongo = await DataMongo.GetStatsFromMongo();

        foreach (var stat in statsFromMongo)
        {
            var existingPlayer = context.ChessPlayers.FirstOrDefault(p => p.ChessId == stat.ChessId);
            if (existingPlayer == null)
            {
                Console.WriteLine($"Player with ChessId {stat.ChessId} does not exist. Skipping stat insert.");
                continue;
            }

            var statList = context.Stats
                .Where(s => s.ChessId == stat.ChessId)
                .ToList();

            Stats? alreadyExists =
                statList.FirstOrDefault(ae => ae.fide == stat.fide && Compare.CompareGameModes(ae, stat));

            if (alreadyExists == null)
            {
                context.Stats.Add(stat);
                await context.SaveChangesAsync();
                Console.WriteLine($"Inserted new stats for ChessId: {stat.ChessId}");
            }
            else
            {
                alreadyExists.Weight += 1;
                await context.SaveChangesAsync();
                Console.WriteLine($"Updated weight for ChessId: {stat.ChessId}");
            }
        }

        return statsFromMongo.FirstOrDefault();
    }
}
