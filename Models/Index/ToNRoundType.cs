using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToNSaveManager.Models.Index {
    public enum ToNRoundType {
        Unknown, // Default

        // Normal
        Classic, Fog,
        Punished, Sabotage,
        Cracked, Bloodbath,

        // Contains alternates
        Midnight, Alternate,

        // Moons // Replace spaces with underscore
        Mystic_Moon, Blood_Moon, Twilight, Solstice,

        // Special // Replace 8 with Eight
        RUN, Eight_Pages,

        // New
        Cold_Night,

        Unbound, // Don't know how it works yet
        Ghost,

        Fog_Alternate,
        Ghost_Alternate,

        GIGABYTE,

        Double_Trouble, // Bloodbath - Two killers have same id
        EX              // Bloodbath - All killers have same id
    }

    public enum ToNRoundResult {
        R, // Respawn
        W, // Win
        D, // Leaving
    }
}
