using ChessAPI.Models;

namespace ChessAPI.DataCollect.Models.Base;

public interface IChessMode
{
    Last? last { get; set; }
    Best? best { get; set; }
    Record? record { get; set; }
}
