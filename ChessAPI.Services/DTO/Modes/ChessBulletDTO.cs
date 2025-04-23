using ChessAPI.Services.DTO.Records;

namespace ChessAPI.Services.DTO.Modes;

public class ChessBulletDTO
{
    public LastDTO Last { get; set; }
    public BestDTO Best { get; set; }
    public RecordDTO Record { get; set; }
}