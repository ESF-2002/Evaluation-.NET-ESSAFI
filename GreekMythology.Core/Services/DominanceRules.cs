namespace GreekMythology.Core.Services;

using GreekMythology.Core.Models;


public class DominanceRules
{
    private readonly Dictionary<God, HashSet<God>> _dominanceMap;

    public DominanceRules()
    {
        // Initialisation des règles de dominance selon le tableau fourni
        _dominanceMap = new Dictionary<God, HashSet<God>>
        {
            { God.Zeus, new HashSet<God> { God.Ares, God.Poseidon } },
            { God.Athena, new HashSet<God> { God.Zeus, God.Ares } },
            { God.Hades, new HashSet<God> { God.Zeus, God.Artemis } },
            { God.Ares, new HashSet<God> { God.Hades, God.Artemis } },
            { God.Poseidon, new HashSet<God> { God.Athena, God.Hades } },
            { God.Artemis, new HashSet<God> { God.Poseidon, God.Athena } }
        };
    }
    
    public int DetermineWinner(God god1, God god2)
    {
        if (god1 == god2)
        {
            return 0; // Égalité
        }

        if (_dominanceMap[god1].Contains(god2))
        {
            return 1; // god1 bat god2
        }

        if (_dominanceMap[god2].Contains(god1))
        {
            return 2; // god2 bat god1
        }

        return 0; // Égalité (ne devrait pas arriver selon les règles)
    }

  
    public bool Beats(God god1, God god2)
    {
        return _dominanceMap[god1].Contains(god2);
    }
}
