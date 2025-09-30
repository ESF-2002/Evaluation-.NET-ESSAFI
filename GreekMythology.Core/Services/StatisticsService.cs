namespace GreekMythology.Core.Services;

using GreekMythology.Core.Models;

public class StatisticsService
{
    private readonly DominanceRules _dominanceRules;

    public StatisticsService()
    {
        _dominanceRules = new DominanceRules();
    }

 
    public AdvancedStatistics CalculateAdvancedStatistics(Match match, TournamentResult result)
    {
        var stats = new AdvancedStatistics();

        // Calcul des statistiques par dieu pour chaque joueur
        stats.Player1GodStats = CalculateGodStatistics(
            match.Joueur1.Moves, 
            match.Joueur2.Moves, 
            result.TotalRounds,
            isPlayer1: true
        );

        stats.Player2GodStats = CalculateGodStatistics(
            match.Joueur2.Moves, 
            match.Joueur1.Moves, 
            result.TotalRounds,
            isPlayer1: false
        );

        // Historique des rounds
        stats.RoundHistory = BuildRoundHistory(match);

        // Calcul des séries de victoires
        var (p1Streak, p2Streak) = CalculateWinStreaks(stats.RoundHistory, match.Joueur1.Nom, match.Joueur2.Nom);
        stats.LongestPlayer1WinStreak = p1Streak;
        stats.LongestPlayer2WinStreak = p2Streak;

        // Matchups tête-à-tête (quel dieu contre quel dieu)
        stats.HeadToHeadMatchups = CalculateHeadToHeadMatchups(match);

        return stats;
    }

    private List<GodStatistic> CalculateGodStatistics(
        List<string> playerMoves, 
        List<string> opponentMoves, 
        int totalRounds,
        bool isPlayer1)
    {
        var godStats = new Dictionary<string, GodStatistic>();

        for (int i = 0; i < totalRounds; i++)
        {
            var playerGodStr = playerMoves[i];
            var opponentGodStr = opponentMoves[i];

            if (!godStats.ContainsKey(playerGodStr))
            {
                godStats[playerGodStr] = new GodStatistic { GodName = playerGodStr };
            }

            var stat = godStats[playerGodStr];
            stat.TimesUsed++;

            if (Enum.TryParse<God>(playerGodStr, out var playerGod) && 
                Enum.TryParse<God>(opponentGodStr, out var opponentGod))
            {
                int result = _dominanceRules.DetermineWinner(playerGod, opponentGod);
                
                if (result == 1)
                {
                    stat.Wins++;
                }
                else if (result == 2)
                {
                    stat.Losses++;
                }
                else
                {
                    stat.Draws++;
                }
            }
        }

        // Calcul des taux
        foreach (var stat in godStats.Values)
        {
            stat.WinRate = stat.TimesUsed > 0 
                ? (double)stat.Wins / stat.TimesUsed * 100 
                : 0;
            stat.UsageRate = totalRounds > 0 
                ? (double)stat.TimesUsed / totalRounds * 100 
                : 0;
        }

        return godStats.Values
            .OrderByDescending(s => s.TimesUsed)
            .ToList();
    }

    private List<RoundDetail> BuildRoundHistory(Match match)
    {
        var history = new List<RoundDetail>();
        int totalRounds = Math.Min(match.Joueur1.Moves.Count, match.Joueur2.Moves.Count);

        for (int i = 0; i < totalRounds; i++)
        {
            var god1Str = match.Joueur1.Moves[i];
            var god2Str = match.Joueur2.Moves[i];

            string winner = "Draw";
            if (Enum.TryParse<God>(god1Str, out var god1) && 
                Enum.TryParse<God>(god2Str, out var god2))
            {
                int result = _dominanceRules.DetermineWinner(god1, god2);
                winner = result == 1 ? match.Joueur1.Nom : 
                         result == 2 ? match.Joueur2.Nom : 
                         "Draw";
            }

            history.Add(new RoundDetail
            {
                RoundNumber = i + 1,
                Player1God = god1Str,
                Player2God = god2Str,
                Winner = winner
            });
        }

        return history;
    }

    private (WinStreak player1, WinStreak player2) CalculateWinStreaks(
        List<RoundDetail> history, 
        string player1Name, 
        string player2Name)
    {
        var p1BestStreak = new WinStreak();
        var p2BestStreak = new WinStreak();
        
        var p1CurrentStreak = new WinStreak();
        var p2CurrentStreak = new WinStreak();

        for (int i = 0; i < history.Count; i++)
        {
            var round = history[i];

            if (round.Winner == player1Name)
            {
                if (p1CurrentStreak.Length == 0)
                {
                    p1CurrentStreak.StartRound = round.RoundNumber;
                }
                p1CurrentStreak.Length++;
                p1CurrentStreak.EndRound = round.RoundNumber;

                // Reset player 2 streak
                if (p1CurrentStreak.Length > p1BestStreak.Length)
                {
                    p1BestStreak = new WinStreak
                    {
                        Length = p1CurrentStreak.Length,
                        StartRound = p1CurrentStreak.StartRound,
                        EndRound = p1CurrentStreak.EndRound
                    };
                }

                p2CurrentStreak = new WinStreak();
            }
            else if (round.Winner == player2Name)
            {
                if (p2CurrentStreak.Length == 0)
                {
                    p2CurrentStreak.StartRound = round.RoundNumber;
                }
                p2CurrentStreak.Length++;
                p2CurrentStreak.EndRound = round.RoundNumber;

                if (p2CurrentStreak.Length > p2BestStreak.Length)
                {
                    p2BestStreak = new WinStreak
                    {
                        Length = p2CurrentStreak.Length,
                        StartRound = p2CurrentStreak.StartRound,
                        EndRound = p2CurrentStreak.EndRound
                    };
                }

                p1CurrentStreak = new WinStreak();
            }
            else
            {
                // Draw resets both streaks
                p1CurrentStreak = new WinStreak();
                p2CurrentStreak = new WinStreak();
            }
        }

        return (p1BestStreak, p2BestStreak);
    }

    private Dictionary<string, int> CalculateHeadToHeadMatchups(Match match)
    {
        var matchups = new Dictionary<string, int>();
        int totalRounds = Math.Min(match.Joueur1.Moves.Count, match.Joueur2.Moves.Count);

        for (int i = 0; i < totalRounds; i++)
        {
            var god1 = match.Joueur1.Moves[i];
            var god2 = match.Joueur2.Moves[i];
            
            var matchupKey = $"{god1} vs {god2}";
            matchups[matchupKey] = matchups.GetValueOrDefault(matchupKey, 0) + 1;
        }

        return matchups.OrderByDescending(kvp => kvp.Value)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
}
