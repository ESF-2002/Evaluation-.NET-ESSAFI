namespace GreekMythology.Core.Models;

/// <summary>
/// Représente un match entre deux joueurs
/// </summary>
public class Match
{
    public Player Joueur1 { get; set; } = null!;
    public Player Joueur2 { get; set; } = null!;
}
