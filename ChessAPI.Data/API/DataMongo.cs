using ChessAPI.DataCollect.Helpers;
using ChessAPI.Models;
using ChessAPI.DataCollect.API;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace ChessAPI.DataCollect.API;

public class DataMongo
{
    private static readonly IMongoClient _mongoClient = new MongoClient(Environment.GetEnvironmentVariable("MONGODB"));
    private static readonly IMongoDatabase _mongoDatabase = _mongoClient.GetDatabase("Chess");
    private static readonly IMongoCollection<BsonDocument> _statsCollection = _mongoDatabase.GetCollection<BsonDocument>("Stats");
    private static readonly IMongoCollection<BsonDocument> _playersCollection = _mongoDatabase.GetCollection<BsonDocument>("Players");
    
    public static async Task InsertRawDataIntoMongoDB(string url, string username)
    {
        BsonDocument fetchedPlayer = await GetData.GetPlayer(url);
        if (fetchedPlayer != null)
        {
            await _playersCollection.InsertOneAsync(fetchedPlayer);
            Console.WriteLine("Inserted raw player data");
        }

        BsonDocument stats = await GetData.GetStats(string.Concat(url + "/stats"), username);
        if (stats != null)
        {
            await _statsCollection.InsertOneAsync(stats); 
            Console.WriteLine("Inserted raw stats for player");
        }
    }

    
    public static async Task<List<BsonDocument>> GetRawStatsFromMongoDB(string selectedUsername)
    {
        FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("username", selectedUsername);
        SortDefinition<BsonDocument> sort = Builders<BsonDocument>.Sort.Descending("createdAt");

        return await _statsCollection.Find(filter)
            .Sort(sort)
            .Limit(1)
            .ToListAsync();
    }

    public static async Task<List<BsonDocument>> GetRawPlayersFromMongoDB(string selectedUsername)
    {
        FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("username", selectedUsername);
        SortDefinition<BsonDocument> sort = Builders<BsonDocument>.Sort.Descending("createdAt");

        return await _playersCollection.Find(filter)
            .Sort(sort)
            .Limit(1)
            .ToListAsync();
    }


    
    
    public static async Task<List<Stats>> GetStatsFromMongo(string selectedUsername)
    {
        IgnoreExtraFields.RegisterClassMaps();
        
        List<BsonDocument> rawStatsFromMongo = await GetRawStatsFromMongoDB(selectedUsername);

        List<Stats> statsFromMongo = rawStatsFromMongo
            .Select(doc => MongoDeserialize.DeserializeStats(doc)) 
            .ToList();

        return statsFromMongo;
    }

    public static async Task<List<ChessPlayer>> GetPlayersFromMongo(string selectedUsername)
    {
        IgnoreExtraFields.RegisterClassMaps();
        
        List<BsonDocument> rawPlayersFromMongo = await GetRawPlayersFromMongoDB(selectedUsername);

        List<ChessPlayer> playersFromMongo = rawPlayersFromMongo
            .Select(doc => MongoDeserialize.DeserializeChessPlayer(doc)) 
            .ToList();

        return playersFromMongo;
    }

}