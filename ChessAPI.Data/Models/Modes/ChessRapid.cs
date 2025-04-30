using ChessAPI.DataCollect.Models.Base;

namespace ChessAPI.Models;

public class ChessRapid : IChessMode
{
    public int id { get; set; }
    public Last? last { get; set; }
    public Best? best { get; set; }
    public Record? record { get; set; }
    public int Weight { get; set; }
}