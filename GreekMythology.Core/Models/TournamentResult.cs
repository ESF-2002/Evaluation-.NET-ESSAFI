namespace GreekMythology.Core.Models;

/// <summary>
/// le resultt du tournoi avec statistiques
/// </summary>
public class TournamentResult
{
    public string Winner { get; set; } = string.Empty;
    public int Player1Wins { get; set; }
    public int Player2Wins { get; set; }
    public int Draws { get; set; }
    public int TotalRounds { get; set; }
    public Dictionary<string, int> Player1GodUsage { get; set; } = new();
    public Dictionary<string, int> Player2GodUsage { get; set; } = new();
    public double Player1WinRate { get; set; }
    public double Player2WinRate { get; set; }
}
