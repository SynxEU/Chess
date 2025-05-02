using ChessAPI.Models;
using MongoDB.Bson.Serialization;

namespace ChessAPI.DataCollect.Helpers
{
    public static class IgnoreExtraFields
    {
        public static void RegisterClassMaps()
        {
            RegisterClassMapIfNotRegistered<ChessDaily>();
            RegisterClassMapIfNotRegistered<ChessPlayer>();
            RegisterClassMapIfNotRegistered<ChessBullet>();
            RegisterClassMapIfNotRegistered<ChessBlitz>();
            RegisterClassMapIfNotRegistered<Stats>();
            RegisterClassMapIfNotRegistered<ChessRapid>();
            RegisterClassMapIfNotRegistered<Last>();
            RegisterClassMapIfNotRegistered<Record>();
            RegisterClassMapIfNotRegistered<Best>();
            RegisterClassMapIfNotRegistered<Tactics>();
            RegisterClassMapIfNotRegistered<Highest>();
            RegisterClassMapIfNotRegistered<Lowest>();
        }

        private static void RegisterClassMapIfNotRegistered<T>()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
            {
                BsonClassMap.RegisterClassMap<T>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }
    }
}