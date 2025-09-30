namespace GreekMythology.Tests;

using GreekMythology.Core.Models;
using GreekMythology.Core.Services;
using Xunit;

/// <summary>
/// Tests unitaires pour le calculateur de tournoi
/// </summary>
public class TournamentCalculatorTests
{
    private readonly TournamentCalculator _calculator;

    public TournamentCalculatorTests()
    {
        _calculator = new TournamentCalculator();
    }

    [Fact]
    public void CalculateResult_ShouldDetermineCorrectWinner_WhenPlayer1Wins()
    {
        // Arrange
        var match = new Match
        {
            Joueur1 = new Player
            {
                Nom = "Achille",
                Moves = new List<string> { "Zeus", "Zeus", "Zeus" }
            },
            Joueur2 = new Player
            {
                Nom = "Hector",
                Moves = new List<string> { "Ares", "Ares", "Ares" }
            }
        };

        // Act
        var result = _calculator.CalculateResult(match);

        // Assert
        Assert.Equal("Achille", result.Winner);
        Assert.Equal(3, result.Player1Wins);
        Assert.Equal(0, result.Player2Wins);
        Assert.Equal(0, result.Draws);
    }

    [Fact]
    public void CalculateResult_ShouldDetermineCorrectWinner_WhenPlayer2Wins()
    {
        // Arrange
        var match = new Match
        {
            Joueur1 = new Player
            {
                Nom = "Achille",
                Moves = new List<string> { "Ares", "Ares", "Ares" }
            },
            Joueur2 = new Player
            {
                Nom = "Hector",
                Moves = new List<string> { "Zeus", "Zeus", "Zeus" }
            }
        };

        // Act
        var result = _calculator.CalculateResult(match);

        // Assert
        Assert.Equal("Hector", result.Winner);
        Assert.Equal(0, result.Player1Wins);
        Assert.Equal(3, result.Player2Wins);
        Assert.Equal(0, result.Draws);
    }

    [Fact]
    public void CalculateResult_ShouldHandleDraws()
    {
        // Arrange
        var match = new Match
        {
            Joueur1 = new Player
            {
                Nom = "Achille",
                Moves = new List<string> { "Zeus", "Zeus", "Zeus" }
            },
            Joueur2 = new Player
            {
                Nom = "Hector",
                Moves = new List<string> { "Zeus", "Zeus", "Zeus" }
            }
        };

        // Act
        var result = _calculator.CalculateResult(match);

        // Assert
        Assert.Equal("Égalité", result.Winner);
        Assert.Equal(0, result.Player1Wins);
        Assert.Equal(0, result.Player2Wins);
        Assert.Equal(3, result.Draws);
    }

    [Fact]
    public void CalculateResult_ShouldCalculateCorrectWinRates()
    {
        // Arrange
        var match = new Match
        {
            Joueur1 = new Player
            {
                Nom = "Achille",
                Moves = new List<string> { "Zeus", "Zeus", "Ares", "Ares" }
            },
            Joueur2 = new Player
            {
                Nom = "Hector",
                Moves = new List<string> { "Ares", "Ares", "Zeus", "Zeus" }
            }
        };

        // Act
        var result = _calculator.CalculateResult(match);

        // Assert
        Assert.Equal(4, result.TotalRounds);
        Assert.Equal(50.0, result.Player1WinRate);
        Assert.Equal(50.0, result.Player2WinRate);
    }

    [Fact]
    public void CalculateResult_ShouldTrackGodUsage()
    {
        // Arrange
        var match = new Match
        {
            Joueur1 = new Player
            {
                Nom = "Achille",
                Moves = new List<string> { "Zeus", "Zeus", "Athena", "Ares" }
            },
            Joueur2 = new Player
            {
                Nom = "Hector",
                Moves = new List<string> { "Poseidon", "Poseidon", "Poseidon", "Hades" }
            }
        };

        // Act
        var result = _calculator.CalculateResult(match);

        // Assert
        Assert.Equal(2, result.Player1GodUsage["Zeus"]);
        Assert.Equal(1, result.Player1GodUsage["Athena"]);
        Assert.Equal(1, result.Player1GodUsage["Ares"]);
        Assert.Equal(3, result.Player2GodUsage["Poseidon"]);
        Assert.Equal(1, result.Player2GodUsage["Hades"]);
    }

    [Fact]
    public void GetMostUsedGods_ShouldReturnTopGods()
    {
        // Arrange
        var godUsage = new Dictionary<string, int>
        {
            { "Zeus", 100 },
            { "Athena", 200 },
            { "Ares", 50 },
            { "Poseidon", 150 }
        };

        // Act
        var topGods = _calculator.GetMostUsedGods(godUsage, 2);

        // Assert
        Assert.Equal(2, topGods.Count);
        Assert.Equal("Athena", topGods[0].God);
        Assert.Equal(200, topGods[0].Count);
        Assert.Equal("Poseidon", topGods[1].God);
        Assert.Equal(150, topGods[1].Count);
    }

    [Fact]
    public void CalculateResult_ShouldHandleMixedResults()
    {
        // Arrange - Scénario réaliste avec différents résultats
        var match = new Match
        {
            Joueur1 = new Player
            {
                Nom = "Achille",
                Moves = new List<string> 
                { 
                    "Zeus",     // Zeus bat Ares -> Joueur1 gagne
                    "Athena",   // Athena bat Zeus -> Joueur1 gagne
                    "Hades",    // Hades = Hades -> Égalité
                    "Ares",     // Ares perd contre Athena -> Joueur2 gagne
                    "Poseidon"  // Poseidon bat Athena -> Joueur1 gagne
                }
            },
            Joueur2 = new Player
            {
                Nom = "Hector",
                Moves = new List<string> 
                { 
                    "Ares",     // Ares perd contre Zeus
                    "Zeus",     // Zeus perd contre Athena
                    "Hades",    // Hades = Hades
                    "Athena",   // Athena bat Ares
                    "Athena"    // Athena perd contre Poseidon
                }
            }
        };

        // Act
        var result = _calculator.CalculateResult(match);

        // Assert
        Assert.Equal(5, result.TotalRounds);
        Assert.Equal(3, result.Player1Wins);
        Assert.Equal(1, result.Player2Wins);
        Assert.Equal(1, result.Draws);
        Assert.Equal("Achille", result.Winner);
        Assert.Equal(60.0, result.Player1WinRate);
        Assert.Equal(20.0, result.Player2WinRate);
    }
}
