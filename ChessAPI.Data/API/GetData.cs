using MongoDB.Bson;

namespace ChessAPI.DataCollect.API;

public class GetData
{
    public static async Task<BsonDocument> GetPlayer(string url)
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
        BsonDocument player = BsonDocument.Parse(profileJson);
    
        player.Add("createdAt", BsonValue.Create(DateTime.UtcNow));

        return player;
    }


    public static async Task<BsonDocument> GetStats(string url, string username)
    {
        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("CSharpApp/1.0");

        HttpResponseMessage statsResponse = await client.GetAsync(url);

        if (!statsResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to fetch data. Status code: Stats = {statsResponse.StatusCode}");
            return null;
        }

        string statsJson = await statsResponse.Content.ReadAsStringAsync();
        BsonDocument stats = BsonDocument.Parse(statsJson);
    
        stats.Add("createdAt", BsonValue.Create(DateTime.UtcNow));
        stats.Add("username", username);

        return stats;
    }

}
