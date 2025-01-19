using MyFirstProgram.Enums;

namespace MyFirstProgram.Models;

public class GamesHistoryItem
{

    public required int Id { get; set; }
    public required string PlayerName { get; set; }
    public GameOptions GameType { get; set; }
    public DateTime GameDate { get; set; }
    public required string GamePlay { get; set; }
    public GameResult GameResult { get; set; }

}