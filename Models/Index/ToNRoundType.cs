using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToNSaveManager.Models.Index {
    public enum ToNRoundType {
        Unknown = 0,

        // Normal
        Classic,         //  1
        Fog,             //  2
        Punished,        //  3
        Sabotage,        //  4
        Cracked,         //  5
        Bloodbath,       //  6
        Double_Trouble,  //  7 // Bloodbath - Two killers have same id
        EX,              //  8 // Bloodbath - All killers have same id
        Ghost,           //  9
        Unbound,         // 10

        // Contains alternates
        Midnight     =  50,
        Alternate,   // 51
        Fog_Alt,     // 52
        Ghost_Alt,   // 53

        // Moons // Replace spaces with underscore
        Mystic_Moon  =  100,
        Blood_Moon,  // 101
        Twilight,    // 102
        Solstice,    // 103

        // Specials
        RUN,         // 104
        Eight_Pages, // 105 // Replace 8 with Eight
        GIGABYTE,    // 106

        // Events
        Cold_Night   =  150 // Winterfest
    }

    public enum ToNRoundResult {
        R, // Respawn
        W, // Win
        D, // Leaving
    }
}
