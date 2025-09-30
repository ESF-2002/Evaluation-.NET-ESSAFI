namespace GreekMythology.Core.Services;

using GreekMythology.Core.Models;


public class TournamentCalculator
{
    private readonly DominanceRules _dominanceRules;

    public TournamentCalculator()
    {
        _dominanceRules = new DominanceRules();
    }

   
    public TournamentResult CalculateResult(Match match)
    {
        var result = new TournamentResult
        {
            TotalRounds = Math.Min(match.Joueur1.Moves.Count, match.Joueur2.Moves.Count)
        };

        // Compteurs pour les statistiques
        var player1GodUsage = new Dictionary<string, int>();
        var player2GodUsage = new Dictionary<string, int>();

        // Calcul des résultats round par round
        for (int i = 0; i < result.TotalRounds; i++)
        {
            var god1Str = match.Joueur1.Moves[i];
            var god2Str = match.Joueur2.Moves[i];

            // Comptage de l'utilisation des dieux
            player1GodUsage[god1Str] = player1GodUsage.GetValueOrDefault(god1Str, 0) + 1;
            player2GodUsage[god2Str] = player2GodUsage.GetValueOrDefault(god2Str, 0) + 1;

            // Conversion en enum
            if (Enum.TryParse<God>(god1Str, out var god1) && 
                Enum.TryParse<God>(god2Str, out var god2))
            {
                int roundWinner = _dominanceRules.DetermineWinner(god1, god2);
                
                if (roundWinner == 1)
                {
                    result.Player1Wins++;
                }
                else if (roundWinner == 2)
                {
                    result.Player2Wins++;
                }
                else
                {
                    result.Draws++;
                }
            }
        }

        // Détermination du gagnant final
        if (result.Player1Wins > result.Player2Wins)
        {
            result.Winner = match.Joueur1.Nom;
        }
        else if (result.Player2Wins > result.Player1Wins)
        {
            result.Winner = match.Joueur2.Nom;
        }
        else
        {
            result.Winner = "Égalité";
        }

        // Calcul des taux de victoire
        result.Player1WinRate = result.TotalRounds > 0 
            ? (double)result.Player1Wins / result.TotalRounds * 100 
            : 0;
        result.Player2WinRate = result.TotalRounds > 0 
            ? (double)result.Player2Wins / result.TotalRounds * 100 
            : 0;

        // Statistiques d'utilisation des dieux
        result.Player1GodUsage = player1GodUsage;
        result.Player2GodUsage = player2GodUsage;

        return result;
    }

    public List<(string God, int Count)> GetMostUsedGods(Dictionary<string, int> godUsage, int top = 3)
    {
        return godUsage
            .OrderByDescending(kvp => kvp.Value)
            .Take(top)
            .Select(kvp => (kvp.Key, kvp.Value))
            .ToList();
    }
}
