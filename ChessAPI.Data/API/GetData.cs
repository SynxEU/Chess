using System;
using System.Data.Entity;
using System.Net.Http;
using System.Threading.Tasks;
using ChessAPI.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace ChessAPI.DataCollect.API;

public class GetData
{

    public static async Task<Stats> GetStats(string url, ChessDbContext context, ChessPlayer player)
    {
        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("CSharpApp/1.0");

        HttpResponseMessage statsResponse = await client.GetAsync($"{url}");

        if (!statsResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to fetch data. Status code: Stats = {statsResponse.StatusCode}");
            return null;
        }

        string statsJson = await statsResponse.Content.ReadAsStringAsync();
        Stats? stats = JsonConvert.DeserializeObject<Stats>(statsJson);

        if (stats != null)
        {
            stats.FetchedAtDate = DateOnly.FromDateTime(DateTime.UtcNow);
            stats.FetchedAtTime = TimeOnly.FromDateTime(DateTime.UtcNow).ToTimeSpan();
            stats.ChessId = player.ChessId;
            
            // Needs to be reworked, not checking how i wanted
            bool alreadyExists = context.Stats
                .Any(s => s.ChessId == player.ChessId
                          && s.FetchedAtTime == stats.FetchedAtTime
                          && s.FetchedAtDate == stats.FetchedAtDate);

            if (!alreadyExists)
            {
                context.Stats.Add(stats);
                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine($"Stats already exist for {player.username} on {stats.FetchedAtDate} at {stats.FetchedAtTime} — skipping save.");
            }
        }

        return stats;
    }

    public static async Task<ChessPlayer> GetPlayer(string url, ChessDbContext context)
    {
        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("CSharpApp/1.0");

        HttpResponseMessage profileResponse = await client.GetAsync(url);

        if (!profileResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to fetch data. Status code: Profile = {profileResponse.StatusCode}");
            return null;
        }

        string profileJson = await profileResponse.Content.ReadAsStringAsync();
        ChessPlayer? player = JsonConvert.DeserializeObject<ChessPlayer>(profileJson);

        if (player != null)
        {
            player.FetchedAtDate = DateOnly.FromDateTime(DateTime.UtcNow);
            player.FetchedAtTime = TimeOnly.FromDateTime(DateTime.UtcNow).ToTimeSpan();

            // Needs to be reworked, not checking how i wanted
            bool alreadyExists = context.ChessPlayers
                .Any(p => p.username == player.username 
                          && p.FetchedAtDate == player.FetchedAtDate
                          && p.FetchedAtTime == player.FetchedAtTime);

            if (!alreadyExists)
                
            {
                context.ChessPlayers.Add(player);
                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine($"Data already exists for {player.username} at {player.FetchedAtDate} at {player.FetchedAtTime} — skipping save.");
            }
        }

        return player;
    }

}