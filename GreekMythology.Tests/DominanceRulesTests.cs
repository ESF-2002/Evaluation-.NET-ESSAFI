namespace GreekMythology.Tests;

using GreekMythology.Core.Models;
using GreekMythology.Core.Services;
using Xunit;

/// <summary>
/// Tests unitaires pour les règles de dominance
/// </summary>
public class DominanceRulesTests
{
    private readonly DominanceRules _dominanceRules;

    public DominanceRulesTests()
    {
        _dominanceRules = new DominanceRules();
    }

    [Theory]
    [InlineData(God.Zeus, God.Ares, 1)]
    [InlineData(God.Zeus, God.Poseidon, 1)]
    [InlineData(God.Athena, God.Zeus, 1)]
    [InlineData(God.Athena, God.Ares, 1)]
    [InlineData(God.Hades, God.Zeus, 1)]
    [InlineData(God.Hades, God.Artemis, 1)]
    [InlineData(God.Ares, God.Hades, 1)]
    [InlineData(God.Ares, God.Artemis, 1)]
    [InlineData(God.Poseidon, God.Athena, 1)]
    [InlineData(God.Poseidon, God.Hades, 1)]
    [InlineData(God.Artemis, God.Poseidon, 1)]
    [InlineData(God.Artemis, God.Athena, 1)]
    public void DetermineWinner_ShouldReturnCorrectWinner_WhenGod1Wins(God god1, God god2, int expected)
    {
        // Act
        var result = _dominanceRules.DetermineWinner(god1, god2);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(God.Ares, God.Zeus, 2)]
    [InlineData(God.Poseidon, God.Zeus, 2)]
    [InlineData(God.Zeus, God.Athena, 2)]
    [InlineData(God.Ares, God.Athena, 2)]
    [InlineData(God.Zeus, God.Hades, 2)]
    [InlineData(God.Artemis, God.Hades, 2)]
    public void DetermineWinner_ShouldReturnCorrectWinner_WhenGod2Wins(God god1, God god2, int expected)
    {
        // Act
        var result = _dominanceRules.DetermineWinner(god1, god2);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(God.Zeus, God.Zeus)]
    [InlineData(God.Athena, God.Athena)]
    [InlineData(God.Hades, God.Hades)]
    [InlineData(God.Ares, God.Ares)]
    [InlineData(God.Poseidon, God.Poseidon)]
    [InlineData(God.Artemis, God.Artemis)]
    public void DetermineWinner_ShouldReturnDraw_WhenSameGod(God god1, God god2)
    {
        // Act
        var result = _dominanceRules.DetermineWinner(god1, god2);

        // Assert
        Assert.Equal(0, result);
    }

    [Theory]
    [InlineData(God.Zeus, God.Ares, true)]
    [InlineData(God.Zeus, God.Poseidon, true)]
    [InlineData(God.Zeus, God.Athena, false)]
    [InlineData(God.Athena, God.Zeus, true)]
    [InlineData(God.Athena, God.Poseidon, false)]
    public void Beats_ShouldReturnCorrectResult(God god1, God god2, bool expected)
    {
        // Act
        var result = _dominanceRules.Beats(god1, god2);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void DominanceRules_ShouldBeSymmetric()
    {
        // Arrange
        var allGods = Enum.GetValues<God>();

        // Act & Assert
        foreach (var god1 in allGods)
        {
            foreach (var god2 in allGods)
            {
                if (god1 == god2) continue;

                var result1 = _dominanceRules.DetermineWinner(god1, god2);
                var result2 = _dominanceRules.DetermineWinner(god2, god1);

                // Si god1 bat god2, alors god2 ne doit pas battre god1
                if (result1 == 1)
                {
                    Assert.Equal(2, result2);
                }
                else if (result1 == 2)
                {
                    Assert.Equal(1, result2);
                }
            }
        }
    }
}
