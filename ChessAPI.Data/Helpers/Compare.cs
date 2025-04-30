using ChessAPI.DataCollect.Models.Base;
using ChessAPI.Models;

namespace ChessAPI.DataCollect.Helpers;

public class Compare
{
    public static bool CompareGameModes(Stats a, Stats b)
    {
        return CompareChessMode(a.chess_daily, b.chess_daily) &&
               CompareChessMode(a.chess_rapid, b.chess_rapid) &&
               CompareChessMode(a.chess_blitz, b.chess_blitz) &&
               CompareChessMode(a.chess_bullet, b.chess_bullet) &&
               CompareTactics(a.tactics, b.tactics);
    }

    private static bool CompareChessMode<T>(T a, T b) where T : class, IChessMode
    {
        bool IsEmpty(T mode) =>
            mode == null ||
            (mode.last == null && mode.best == null && mode.record == null) ;

        if (IsEmpty(a) && IsEmpty(b)) return true;
        if (IsEmpty(a) || IsEmpty(b)) return false;

        return CompareLast(a.last, b.last) &&
               CompareBest(a.best, b.best) &&
               CompareRecord(a.record, b.record);
    }


    private static bool CompareLast(Last a, Last b)
    {
        return a.rating == b.rating &&
               a.date == b.date;
    }

    private static bool CompareBest(Best a, Best b)
    {
        return a.rating == b.rating &&
               a.date == b.date &&
               a.game == b.game;
    }

    private static bool CompareRecord(Record a, Record b)
    {
        return a.win == b.win &&
               a.loss == b.loss &&
               a.draw == b.draw;
    }

    private static bool CompareTactics(Tactics a, Tactics b)
    {
        return a.lowest == b.lowest &&
               a.highest == b.highest;
    }


}