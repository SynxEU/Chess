using System;
using System.Net.Http;
using System.Threading.Tasks;
using ChessAPI.Models;
using Newtonsoft.Json;

namespace ChessAPI.DataCollect.API;

public class GetData
{
    public static async Task<Stats> GetStats(string url)
    {
        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders
            .UserAgent
            .ParseAdd("CSharpApp/1.0");
        
        HttpResponseMessage statsResponse = await client.GetAsync(string.Concat(url, "/stats"));

        if (!statsResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to fetch data. " +
                              $"Status code:\n" +
                              $" Stats = {statsResponse.StatusCode}\n");
        }

        string statsJson = await statsResponse.Content.ReadAsStringAsync();

        Stats? stats = JsonConvert.DeserializeObject<Stats>(statsJson);

        return stats;
    }

    public static async Task<ChessPlayer> GetPlayer(string url)
    {
        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders
            .UserAgent
            .ParseAdd("CSharpApp/1.0");

        HttpResponseMessage profileResponse = await client.GetAsync(url);

        if (!profileResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to fetch data. " +
                              $"Status code:\n" +
                              $" Profile = {profileResponse.StatusCode}");
        }

        string profileJson = await profileResponse.Content.ReadAsStringAsync();

        ChessPlayer? player = JsonConvert.DeserializeObject<ChessPlayer>(profileJson);
        
        return player;
    }
}