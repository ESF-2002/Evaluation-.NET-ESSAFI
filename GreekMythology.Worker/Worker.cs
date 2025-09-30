namespace GreekMythology.Worker;

using System.Text.Json;
using GreekMythology.Core.Models;
using GreekMythology.Core.Services;
using System.Collections.Generic;

/// <summary>
/// Worker service qui télécharge les données et calcule le gagnant du tournoi
/// </summary>
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly TournamentCalculator _calculator;
    private readonly StatisticsService _statisticsService;
    private const string DataUrl = "https://raw.githubusercontent.com/JLou/dotnet-course/refs/heads/main/e1.json";

    public Worker(ILogger<Worker> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _calculator = new TournamentCalculator();
        _statisticsService = new StatisticsService();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Démarrage du tournoi de mythologie grecque à {time}", DateTimeOffset.Now);

            // Téléchargement des données via HTTP
            _logger.LogInformation("Téléchargement des données depuis {url}", DataUrl);
            var match = await DownloadMatchDataAsync(stoppingToken);

            if (match == null)
            {
                _logger.LogError("Impossible de télécharger les données du match");
                return;
            }

            _logger.LogInformation("Match chargé: {player1} vs {player2}", 
                match.Joueur1.Nom, match.Joueur2.Nom);

            // Calcul des résultats
            _logger.LogInformation("Calcul des résultats du tournoi...");
            var result = _calculator.CalculateResult(match);

            var advancedStats = _statisticsService.CalculateAdvancedStatistics(match, result);

            // Affichage des résultats dans la console
            DisplayResults(result, match, advancedStats);

            _logger.LogInformation("Tournoi terminé à {time}", DateTimeOffset.Now);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'exécution du tournoi");
        }
    }

    private async Task<Match?> DownloadMatchDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(DataUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var matches = JsonSerializer.Deserialize<List<Match>>(jsonContent, options);
            var match = matches?.FirstOrDefault();
            return match;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du téléchargement des données");
            return null;
        }
    }

    private void DisplayResults(TournamentResult result, Match match, AdvancedStatistics advancedStats)
    {
        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════════════════════════");
        Console.WriteLine("    TOURNOI DE MYTHOLOGIE GRECQUE - RÉSULTATS FINAUX");
        Console.WriteLine("═══════════════════════════════════════════════════════════");
        Console.WriteLine();
        
        Console.WriteLine($"🏆 VAINQUEUR: {result.Winner}");
        Console.WriteLine();
        
        Console.WriteLine("─────────────────────────────────────────────────────────");
        Console.WriteLine("  STATISTIQUES DU MATCH");
        Console.WriteLine("─────────────────────────────────────────────────────────");
        Console.WriteLine($"  Total de rounds:        {result.TotalRounds}");
        Console.WriteLine($"  Victoires {match.Joueur1.Nom}:      {result.Player1Wins} ({result.Player1WinRate:F2}%)");
        Console.WriteLine($"  Victoires {match.Joueur2.Nom}:       {result.Player2Wins} ({result.Player2WinRate:F2}%)");
        Console.WriteLine($"  Égalités:                {result.Draws}");
        Console.WriteLine();

        Console.WriteLine("─────────────────────────────────────────────────────────");
        Console.WriteLine($"  DIEUX LES PLUS UTILISÉS - {match.Joueur1.Nom}");
        Console.WriteLine("─────────────────────────────────────────────────────────");
        var topGods1 = _calculator.GetMostUsedGods(result.Player1GodUsage, 3);
        for (int i = 0; i < topGods1.Count; i++)
        {
            var (god, count) = topGods1[i];
            var percentage = (double)count / result.TotalRounds * 100;
            Console.WriteLine($"  {i + 1}. {god,-12} {count,4} fois ({percentage:F1}%)");
        }
        Console.WriteLine();

        Console.WriteLine("─────────────────────────────────────────────────────────");
        Console.WriteLine($"  DIEUX LES PLUS UTILISÉS - {match.Joueur2.Nom}");
        Console.WriteLine("─────────────────────────────────────────────────────────");
        var topGods2 = _calculator.GetMostUsedGods(result.Player2GodUsage, 3);
        for (int i = 0; i < topGods2.Count; i++)
        {
            var (god, count) = topGods2[i];
            var percentage = (double)count / result.TotalRounds * 100;
            Console.WriteLine($"  {i + 1}. {god,-12} {count,4} fois ({percentage:F1}%)");
        }
        Console.WriteLine();

        DisplayAdvancedStatistics(match, advancedStats);

        Console.WriteLine("═══════════════════════════════════════════════════════════");
    }

    private void DisplayAdvancedStatistics(Match match, AdvancedStatistics stats)
    {
        Console.WriteLine("─────────────────────────────────────────────────────────");
        Console.WriteLine("  STATISTIQUES AVANCÉES");
        Console.WriteLine("─────────────────────────────────────────────────────────");
        Console.WriteLine();

        // Séries de victoires
        Console.WriteLine($"  Plus longue série de victoires {match.Joueur1.Nom}:");
        Console.WriteLine($"    {stats.LongestPlayer1WinStreak.Length} victoires consécutives " +
                         $"(rounds {stats.LongestPlayer1WinStreak.StartRound}-{stats.LongestPlayer1WinStreak.EndRound})");
        Console.WriteLine();
        
        Console.WriteLine($"  Plus longue série de victoires {match.Joueur2.Nom}:");
        Console.WriteLine($"    {stats.LongestPlayer2WinStreak.Length} victoires consécutives " +
                         $"(rounds {stats.LongestPlayer2WinStreak.StartRound}-{stats.LongestPlayer2WinStreak.EndRound})");
        Console.WriteLine();

        // Performance par dieu - Joueur 1
        Console.WriteLine($"  PERFORMANCE PAR DIEU - {match.Joueur1.Nom}");
        Console.WriteLine("  ┌──────────────┬────────┬──────┬────────┬─────────┬──────────┐");
        Console.WriteLine("  │ Dieu         │ Utilisé│ Wins │ Losses │ Draws   │ Win Rate │");
        Console.WriteLine("  ├──────────────┼────────┼──────┼────────┼─────────┼──────────┤");
        foreach (var godStat in stats.Player1GodStats.Take(6))
        {
            Console.WriteLine($"  │ {godStat.GodName,-12} │ {godStat.TimesUsed,6} │ {godStat.Wins,4} │ {godStat.Losses,6} │ {godStat.Draws,7} │ {godStat.WinRate,7:F1}% │");
        }
        Console.WriteLine("  └──────────────┴────────┴──────┴────────┴─────────┴──────────┘");
        Console.WriteLine();

        // Performance par dieu - Joueur 2
        Console.WriteLine($"  PERFORMANCE PAR DIEU - {match.Joueur2.Nom}");
        Console.WriteLine("  ┌──────────────┬────────┬──────┬────────┬─────────┬──────────┐");
        Console.WriteLine("  │ Dieu         │ Utilisé│ Wins │ Losses │ Draws   │ Win Rate │");
        Console.WriteLine("  ├──────────────┼────────┼──────┼────────┼─────────┼──────────┤");
        foreach (var godStat in stats.Player2GodStats.Take(6))
        {
            Console.WriteLine($"  │ {godStat.GodName,-12} │ {godStat.TimesUsed,6} │ {godStat.Wins,4} │ {godStat.Losses,6} │ {godStat.Draws,7} │ {godStat.WinRate,7:F1}% │");
        }
        Console.WriteLine("  └──────────────┴────────┴──────┴────────┴─────────┴──────────┘");
        Console.WriteLine();

        // Matchups les plus fréquents
        Console.WriteLine("  MATCHUPS LES PLUS FRÉQUENTS (Top 5)");
        var topMatchups = stats.HeadToHeadMatchups.Take(5);
        foreach (var matchup in topMatchups)
        {
            Console.WriteLine($"    {matchup.Key,-30} {matchup.Value,4} fois");
        }
        Console.WriteLine();
    }
}
