namespace GreekMythology.Core.Models;

/// <summary>
/// Statistiques avancé du tournoi
/// </summary>
public class AdvancedStatistics
{
    public List<GodStatistic> Player1GodStats { get; set; } = new();
    public List<GodStatistic> Player2GodStats { get; set; } = new();
    public List<RoundDetail> RoundHistory { get; set; } = new();
    public WinStreak LongestPlayer1WinStreak { get; set; } = new();
    public WinStreak LongestPlayer2WinStreak { get; set; } = new();
    public Dictionary<string, int> HeadToHeadMatchups { get; set; } = new();
}

/// <summary>
/// Statistiques pour un dieu spécif
/// </summary>
public class GodStatistic
{
    public string GodName { get; set; } = string.Empty;
    public int TimesUsed { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
    public double WinRate { get; set; }
    public double UsageRate { get; set; }
}

/// <summary>
/// Détail d'un round individuel
/// </summary>
public class RoundDetail
{
    public int RoundNumber { get; set; }
    public string Player1God { get; set; } = string.Empty;
    public string Player2God { get; set; } = string.Empty;
    public string Winner { get; set; } = string.Empty;
}

/// <summary>
/// Série de victoires consécutives
/// </summary>
public class WinStreak
{
    public int Length { get; set; }
    public int StartRound { get; set; }
    public int EndRound { get; set; }
}
