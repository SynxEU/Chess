using ChessAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ChessAPI.DataCollect.Helpers;

public static class DataInclude
{
    public static IQueryable<Stats> IncludeAllStats(this IQueryable<Stats> query)
    {
        return query
            .IncludeDaily()
            .IncludeRapid()
            .IncludeBlitz()
            .IncludeBullet()
            .IncludeTactic();
    }
    
    private static IQueryable<Stats> IncludeDaily(this IQueryable<Stats> query)
    {
        return query
            .Include(s => s.chess_daily)
            .ThenInclude(c => c.best)
            .Include(s => s.chess_daily)
            .ThenInclude(c => c.last)
            .Include(s => s.chess_daily)
            .ThenInclude(c => c.record);
    }
    private static IQueryable<Stats> IncludeRapid(this IQueryable<Stats> query)
    {
        return query
            .Include(s => s.chess_rapid)
            .ThenInclude(c => c.best)
            .Include(s => s.chess_rapid)
            .ThenInclude(c => c.last)
            .Include(s => s.chess_rapid)
            .ThenInclude(c => c.record);
    }
    private static IQueryable<Stats> IncludeBullet(this IQueryable<Stats> query)
    {
        return query
            .Include(s => s.chess_bullet)
            .ThenInclude(c => c.best)
            .Include(s => s.chess_bullet)
            .ThenInclude(c => c.last)
            .Include(s => s.chess_bullet)
            .ThenInclude(c => c.record);
    }
    private static IQueryable<Stats> IncludeBlitz(this IQueryable<Stats> query)
    {
        return query
            .Include(s => s.chess_blitz)
            .ThenInclude(c => c.best)
            .Include(s => s.chess_blitz)
            .ThenInclude(c => c.last)
            .Include(s => s.chess_blitz)
            .ThenInclude(c => c.record);
    }
    private static IQueryable<Stats> IncludeTactic(this IQueryable<Stats> query)
    {
        return query
            .Include(s => s.tactics)
            .ThenInclude(t => t.highest)
            .Include(s => s.tactics)
            .ThenInclude(t => t.lowest);
    }
    
    public static IQueryable<ChessPlayer> IncludeAllPlayerData(this IQueryable<ChessPlayer> query)
    {
        return query
            .Include(p => p.streaming_platforms)
            .Include(p => p.Stats)
                .ThenInclude(s => s.chess_daily)
            .Include(p => p.Stats)
                .ThenInclude(s => s.chess_rapid)
            .Include(p => p.Stats)
                .ThenInclude(s => s.chess_blitz)
            .Include(p => p.Stats)
                .ThenInclude(s => s.chess_bullet)
            .Include(p => p.Stats)
                .ThenInclude(s => s.tactics);
    }

}