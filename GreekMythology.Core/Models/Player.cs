namespace GreekMythology.Core.Models;

/// <summary>
/// représente un joueur avec son nom et ses coups
/// </summary>
public class Player
{
    public string Nom { get; set; } = string.Empty;
    public List<string> Moves { get; set; } = new();
}
