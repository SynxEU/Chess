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
    
    public static async Task InsertRawDataIntoMongoDB(string url)
    {
        BsonDocument fetchedPlayer = await GetData.GetPlayer(url);
        if (fetchedPlayer != null)
        {
            await _playersCollection.InsertOneAsync(fetchedPlayer);
            Console.WriteLine("Inserted raw player data");
        }

        BsonDocument stats = await GetData.GetStats(string.Concat(url + "/stats"));
        if (stats != null)
        {
            await _statsCollection.InsertOneAsync(stats); 
            Console.WriteLine("Inserted raw stats for player");
        }
    }

    
    public static async Task<List<BsonDocument>> GetRawStatsFromMongoDB()
    {
        var sortDefinition = Builders<BsonDocument>.Sort
            .Descending("CreatedAt");

        return await _statsCollection.Find(FilterDefinition<BsonDocument>.Empty)
            .Sort(sortDefinition)
            .Limit(1) // Change to more than 1 if you want more recent entries
            .ToListAsync();
    }

    public static async Task<List<BsonDocument>> GetRawPlayersFromMongoDB()
    {
        var sortDefinition = Builders<BsonDocument>.Sort
            .Descending("CreatedAt");

        return await _playersCollection.Find(FilterDefinition<BsonDocument>.Empty)
            .Sort(sortDefinition)
            .Limit(1)
            .ToListAsync();
    }

    
    
    public static async Task<List<Stats>> GetStatsFromMongo()
    {
        IgnoreExtraFields.RegisterClassMaps();
        
        List<BsonDocument> rawStatsFromMongo = await GetRawStatsFromMongoDB();

        List<Stats> statsFromMongo = rawStatsFromMongo
            .Select(doc => MongoDeserialize.DeserializeStats(doc)) // Deserialize BsonDocument to Stats
            .ToList();

        return statsFromMongo;
    }

    public static async Task<List<ChessPlayer>> GetPlayersFromMongo()
    {
        IgnoreExtraFields.RegisterClassMaps();
        
        List<BsonDocument> rawPlayersFromMongo = await GetRawPlayersFromMongoDB();

        List<ChessPlayer> playersFromMongo = rawPlayersFromMongo
            .Select(doc => MongoDeserialize.DeserializeChessPlayer(doc)) // Deserialize BsonDocument to ChessPlayer
            .ToList();

        return playersFromMongo;
    }

}