namespace PacManGame
{
    public static class LevelSpecs
    {
        public const int BaseSpeedPxPerSec = 76; // 75.75757625 rounded — 100% speed reference

        // Levels 1 through 21+ (index 0 = level 1, index 20 = level 21+)
        public static readonly int[] PacManSpeed          = { 80, 90, 90, 90, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 90 };
        public static readonly int[] PacManDotsSpeed       = { 71, 79, 79, 79, 87, 87, 87, 87, 87, 87, 87, 87, 87, 87, 87, 87, 87, 87, 87, 87, 79 };
        public static readonly int[] GhostSpeed            = { 75, 85, 85, 85, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 };
        public static readonly int[] GhostTunnelSpeed      = { 40, 45, 45, 45, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50 };

        public static readonly int[] Elroy1DotsLeft        = { 20, 30, 40, 40, 40, 50, 50, 50, 60, 60, 60, 80, 80, 80, 100, 100, 100, 100, 120, 120, 120 };
        public static readonly int[] Elroy1Speed           = { 80, 90, 90, 90, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
        public static readonly int[] Elroy2DotsLeft        = { 10, 15, 20, 20, 20, 25, 25, 25, 30, 30, 30, 40, 40, 40, 50, 50, 50, 50, 60, 60, 60 };
        public static readonly int[] Elroy2Speed           = { 85, 95, 95, 95, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105 };

        // 0 = no fright phase at this level (levels 17, 19, 20, 21+ — the "–" columns in the table)
        public static readonly int[] FrightPacManSpeed     = { 90, 95, 95, 95, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 0, 100, 0, 0, 0 };
        public static readonly int[] FrightPacManDotsSpeed = { 79, 83, 83, 83, 87, 87, 87, 87, 87, 87, 87, 87, 87, 87, 87, 87, 0, 87, 0, 0, 0 };
        public static readonly int[] FrightGhostSpeed      = { 50, 55, 55, 55, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 0, 60, 0, 0, 0 };
        public static readonly int[] FrightTimeSeconds     = { 6, 5, 4, 3, 2, 5, 2, 2, 1, 5, 2, 1, 1, 3, 1, 5, 0, 1, 0, 0, 0 };
        public static readonly int[] FlashCount            = { 5, 5, 5, 5, 5, 5, 5, 5, 3, 5, 5, 3, 3, 5, 3, 5, 0, 3, 0, 0, 0 };

        public static int Level;

        public static int GetEntry(int Level, int[] array)
        {
            Level = Math.Min(Level, array.Length) - 1;
            return array[Level];
        }
        
    }
}