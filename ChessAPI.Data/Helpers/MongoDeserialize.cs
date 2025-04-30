using ChessAPI.Models;

namespace ChessAPI.DataCollect.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

public class MongoDeserialize
{
    public static Stats DeserializeStats(BsonDocument doc)
    {
        // Assuming MongoDB "id" is mapped as the logical identifier.
        var stats = new Stats
        {
            id = doc.Contains("id") ? doc["id"].AsInt32 : 0, // Manually assigning 'id' if it exists
            ChessId = doc.Contains("ChessId")
                ? doc["ChessId"].AsInt32
                : 0, // Manually assign ChessId (you may have another source to assign this)
            fide = doc["fide"].AsInt32,
            Weight = doc.Contains("weight") ? doc["Weight"].AsInt32 : 0,
            FetchedAtDate = doc.Contains("fetchedAtDate") 
                ? DateOnly.FromDateTime(doc["FetchedAtDate"].ToUniversalTime().Date) 
                : DateOnly.FromDateTime(DateTime.UtcNow),
            FetchedAtTime = doc.Contains("FetchedAtTime")
                ? doc["FetchedAtTime"].ToUniversalTime().TimeOfDay
                : TimeSpan.Zero
        };

        // Deserialize ChessPlayer (if embedded in Stats document)
        if (doc.Contains("ChessPlayer") && doc["ChessPlayer"].BsonType == BsonType.Document)
        {
            var chessPlayerDoc = doc["ChessPlayer"].AsBsonDocument;
            var chessPlayer = DeserializeChessPlayer(chessPlayerDoc);

            // Make sure the ChessId is linked from Stats to ChessPlayer
            chessPlayer.ChessId = stats.ChessId;

            stats.ChessPlayer = chessPlayer; // Attach ChessPlayer to Stats
        }

        // Deserialize additional fields if necessary
        stats.chess_daily = doc.Contains("chess_daily")
            ? BsonSerializer.Deserialize<ChessDaily>(doc["chess_daily"].AsBsonDocument)
            : null;
        stats.chess_rapid = doc.Contains("chess_rapid")
            ? BsonSerializer.Deserialize<ChessRapid>(doc["chess_rapid"].AsBsonDocument)
            : null;
        stats.chess_bullet = doc.Contains("chess_bullet")
            ? BsonSerializer.Deserialize<ChessBullet>(doc["chess_bullet"].AsBsonDocument)
            : null;
        stats.chess_blitz = doc.Contains("chess_blitz")
            ? BsonSerializer.Deserialize<ChessBlitz>(doc["chess_blitz"].AsBsonDocument)
            : null;
        stats.tactics = doc.Contains("tactics")
            ? BsonSerializer.Deserialize<Tactics>(doc["tactics"].AsBsonDocument)
            : null;

        return stats;
    }

    public static ChessPlayer DeserializeChessPlayer(BsonDocument doc)
    {
        var chessPlayer = new ChessPlayer
        {
            apiURL = doc.Contains("@id") ? doc["@id"].AsString : null,
            avatar = doc.Contains("avatar") ? doc["avatar"].AsString : null,
            player_id = doc.Contains("player_id") ? doc["player_id"].AsInt32 : 0,
            url = doc.Contains("url") ? doc["url"].AsString : null,
            name = doc.Contains("name") ? doc["name"].AsString : null,
            username = doc.Contains("username") ? doc["username"].AsString : null,
            followers = doc.Contains("followers") ? doc["followers"].AsInt32 : 0,
            country = doc.Contains("country") ? doc["country"].AsString : null,
            location = doc.Contains("location") ? doc["location"].AsString : null,
            last_online = doc.Contains("last_online") ? doc["last_online"].AsInt32 : 0,
            joined = doc.Contains("joined") ? doc["joined"].AsInt32 : 0,
            status = doc.Contains("status") ? doc["status"].AsString : null,
            is_streamer = doc.Contains("is_streamer") ? doc["is_streamer"].AsBoolean : false,
            verified = doc.Contains("verified") ? doc["verified"].AsBoolean : false,
            league = doc.Contains("league") ? doc["league"].AsString : null,
            streaming_platforms = doc.Contains("streaming_platforms") 
                ? doc["streaming_platforms"].AsBsonArray
                    .Select(platform => BsonSerializer.Deserialize<StreamingPlatform>(platform.AsBsonDocument))
                    .ToList()
                : new List<StreamingPlatform>(),
            FetchedAtDate = doc.Contains("fetchedAtDate") 
                ? DateOnly.FromDateTime(doc["FetchedAtDate"].ToUniversalTime().Date) 
                : DateOnly.FromDateTime(DateTime.UtcNow),
            FetchedAtTime = doc.Contains("FetchedAtTime")
                ? doc["FetchedAtTime"].ToUniversalTime().TimeOfDay
                : TimeSpan.Zero,
            UpdatedAtDate = DateOnly.FromDateTime(DateTime.Now.Date),
            UpdatedAtTime = DateTime.Now.TimeOfDay,
            Weight = doc.Contains("Weight") ? doc["Weight"].AsInt32 : 0
        };

        return chessPlayer;
    }

}