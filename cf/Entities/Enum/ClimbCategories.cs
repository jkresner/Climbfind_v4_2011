using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Entities.Enum
{
    /// <summary>
    /// 
    /// </summary>
    public enum ClimbTagCategory : int
    {
        Unknown = 0,
        //-- Climb types (no longer in use)
        Indoor = 101,
        OutdoorRock = 102,
        Ice = 104,
        Mixed = 106,
        Summit = 110,
        Boulder = 201,
        TopRope = 203,
        //-- method?
        Sport = 205,
        Trad = 207,
        Solo = 210,
        WaterSolo = 211,
        HighballSolo = 213,
        Aid = 218,
        DryTool = 222,
        Multipitch = 230,
        //-- Climb angle
        Vertical = 301,
        Overhang = 303,
        Slab = 305,
        Traverse = 310,
        Scramble = 313,
        Roof = 315,
        TopOut = 320,
        //-- Outdoor Climb holds
        Handles = 407,
        Jugs = 410,
        DimeEdgeCrimps = 413,
        Crimps = 415,
        Edges = 417,
        Ledges = 419,
        Slopers = 420,
        Slots = 423,
        Monos = 427,
        TwoFingerPockets = 431,
        Pockets = 434,
        Pinches = 441,
        Volumes = 451,
        Knobs = 453,
        //-- Climb features
        Crack = 510,
        Face = 515,
        Arete = 520,
        Dihedral = 522,
        Prow = 525,
        Bulge = 529,
        Column = 533,
        Tufa = 535,
        Scoop = 537,
        Blocky = 540,
        Stalactite = 543,
        Smears = 545,
        //-- Climb style
        Balancy = 610,
        Bridging = 613,
        Technical = 615,
        Dynamic = 616,
        Sustained = 617,
        Thuggy = 619,
        Powerful = 620,
        LockOffs = 622,
        Deadpoint = 625,
        Dyno = 629,
        Reachy = 633,
        Scrunchy = 635,
        PartyTrick = 637,
        Awkward = 640,
        BetaIntensive = 643,
        Tweaky = 645,
        Kneebar = 647,
        DropKnee = 648,
        HeelHook = 649,
        ToeHook = 651,
        RingLock = 654,
        //-- Crack goodness
        Seam = 707, 
        Tips = 711,
        Fingers = 713,
        Hands = 715,
        StackedHands = 717,
        Fists = 719,
        OffWidth = 721,
        Chimney = 725,
        Corner = 727,
        Flaring = 729
    }
}
