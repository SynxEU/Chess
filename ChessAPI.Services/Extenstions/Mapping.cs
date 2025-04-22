using ChessAPI.Models;
using ChessAPI.Services.DTO.Modes;
using ChessAPI.Services.DTO.PlayerStats;
using ChessAPI.Services.DTO.Records;
using ChessAPI.Services.DTO.Scale;
using ChessAPI.Services.DTO.UserData;

namespace ChessAPI.Services.Extenstions;

public static class Mapping
{
    public static StreamingPlatformDTO ToDTO(this StreamingPlatform model) => new()
    {
        Type = model.type,
        ChannelUrl = model.channel_url
    };

    public static StreamingPlatform ToModel(this StreamingPlatformDTO dto) => new()
    {
        type = dto.Type,
        channel_url = dto.ChannelUrl
    };

    public static ChessPlayerDTO ToDTO(this ChessPlayer model) => new()
    {
        Player_id = model.player_id,
        Avatar = model.avatar,
        Url = model.url,
        Name = model.name,
        Username = model.username,
        Followers = model.followers, 
        Country = model.country,
        Location = model.location,
        LastOnline = model.last_online,
        Joined = model.joined,
        Status = model.status,
        IsStreamer = model.is_streamer,
        Verified = model.verified,
        League = model.league,
        FetchedAt = model.FetchedAt,
        StreamingPlatforms = model.streaming_platforms?
            .Select(p => p.ToDTO())
            .ToList(),
        Stats = model.Stats?
            .Select(s => s.ToDTO())
            .ToList()
    };

    public static ChessPlayer ToModel(this ChessPlayerDTO dto) => new()
    {
        player_id = dto.Player_id,
        avatar = dto.Avatar,
        url = dto.Url,
        name = dto.Name,
        username = dto.Username,
        followers = dto.Followers,
        country = dto.Country,
        location = dto.Location,
        last_online = dto.LastOnline,
        joined = dto.Joined,
        status = dto.Status,
        is_streamer = dto.IsStreamer,
        verified = dto.Verified,
        league = dto.League,
        FetchedAt = dto.FetchedAt,
        streaming_platforms = dto.StreamingPlatforms?
            .Select(p => p.ToModel())
            .ToList(),
        Stats = dto.Stats?
            .Select(s => s.ToModel())
            .ToList()
    };

    public static BestDTO ToDTO(this Best model) => new()
    {
        Rating = model.rating,
        Date = model.date,
        Game = model.game,
        TotalAttempts = model.total_attempts,
        Score = model.score
    };

    public static Best ToModel(this BestDTO dto) => new()
    {
        rating = dto.Rating,
        date = dto.Date,
        game = dto.Game,
        total_attempts = dto.TotalAttempts,
        score = dto.Score
    };

    public static LastDTO ToDTO(this Last model) => new()
    {
        Rating = model.rating,
        Date = model.date,
    };

    public static Last ToModel(this LastDTO dto) => new()
    {
        rating = dto.Rating,
        date = dto.Date,
    };

    public static RecordDTO ToDTO(this Record model) => new()
    {
        Win = model.win,
        Loss = model.loss,
        Draw = model.draw,
    };

    public static Record ToModel(this RecordDTO dto) => new()
    {
        win = dto.Win,
        loss = dto.Loss,
        draw = dto.Draw,
    };

    public static HighestDTO ToDTO(this Highest model) => new()
    {
        Rating = model.rating,
        Date = model.date,
    };

    public static Highest ToModel(this HighestDTO dto) => new()
    {
        rating = dto.Rating,
        date = dto.Date,
    };

    public static LowestDTO ToDTO(this Lowest model) => new()
    {
        Rating = model.rating,
        Date = model.date,
    };

    public static Lowest ToModel(this LowestDTO dto) => new()
    {
        rating = dto.Rating,
        date = dto.Date,
    };

    public static TacticsDTO ToDTO(this Tactics model) => new()
    {
        Highest = model.highest?.ToDTO(),
        Lowest = model.lowest?.ToDTO(),
    };

    public static Tactics ToModel(this TacticsDTO dto) => new()
    {
        highest = dto.Highest?.ToModel(),
        lowest = dto.Lowest?.ToModel(),
    };

    public static PlayerStatsDTO ToDTO(this Stats model) => new()
    {
        ChessRapid = model.chess_rapid?.ToDTO(),
        ChessBullet = model.chess_bullet?.ToDTO(),
        ChessBlitz = model.chess_blitz?.ToDTO(),
        ChessDaily = model.chess_daily?.ToDTO(),
        Tactics = model.tactics?.ToDTO(),
    };

    public static Stats ToModel(this PlayerStatsDTO dto) => new()
    {
        chess_rapid = dto.ChessRapid?.ToModel(),
        chess_bullet = dto.ChessBullet?.ToModel(),
        chess_blitz = dto.ChessBlitz?.ToModel(),
        chess_daily = dto.ChessDaily?.ToModel(),
        tactics = dto.Tactics?.ToModel(),
    };

    public static ChessRapidDTO ToDTO(this ChessRapid model) => new()
    {
        Last = model.last?.ToDTO(),
        Best = model.best?.ToDTO(),
        Record = model.record?.ToDTO()
    };

    public static ChessRapid ToModel(this ChessRapidDTO dto) => new()
    {
        last = dto.Last?.ToModel(),
        best = dto.Best?.ToModel(),
        record = dto.Record?.ToModel()
    };

    public static ChessBulletDTO ToDTO(this ChessBullet model) => new()
    {
        Last = model.last?.ToDTO(),
        Best = model.best?.ToDTO(),
        Record = model.record?.ToDTO()
    };

    public static ChessBullet ToModel(this ChessBulletDTO dto) => new()
    {
        last = dto.Last?.ToModel(),
        best = dto.Best?.ToModel(),
        record = dto.Record?.ToModel()
    };

    public static ChessBlitzDTO ToDTO(this ChessBlitz model) => new()
    {
        Last = model.last?.ToDTO(),
        Best = model.best?.ToDTO(),
        Record = model.record?.ToDTO()
    };

    public static ChessBlitz ToModel(this ChessBlitzDTO dto) => new()
    {
        last = dto.Last?.ToModel(),
        best = dto.Best?.ToModel(),
        record = dto.Record?.ToModel()
    };

    public static ChessDailyDTO ToDTO(this ChessDaily model) => new()
    {
        Last = model.last?.ToDTO(),
        Best = model.best?.ToDTO(),
        Record = model.record?.ToDTO()
    };

    public static ChessDaily ToModel(this ChessDailyDTO dto) => new()
    {
        last = dto.Last?.ToModel(),
        best = dto.Best?.ToModel(),
        record = dto.Record?.ToModel()
    };
}