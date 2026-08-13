namespace PacManGame
{
    public static class LevelSpecs
    {
        public static readonly int[][] board =
            [
                new int[] { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 },
                new int[] { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 },
                new int[] { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 },
                new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                new int[] { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1 },
                new int[] { 1, 2, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2, 1 },
                new int[] { 1, 3, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 3, 1 },
                new int[] { 1, 2, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2, 1 },
                new int[] { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1 },
                new int[] { 1, 2, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 1, 1, 2, 1 },
                new int[] { 1, 2, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 1, 1, 2, 1 },
                new int[] { 1, 2, 2, 2, 2, 2, 2, 1, 1, 2, 2, 2, 2, 1, 1, 2, 2, 2, 2, 1, 1, 2, 2, 2, 2, 2, 2, 1 },
                new int[] { 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1 },
                new int[] { 6, 6, 6, 6, 6, 1, 2, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 2, 1, 6, 6, 6, 6, 6 },
                new int[] { 6, 6, 6, 6, 6, 1, 2, 1, 1, 0, 0, 0, 8, 8, 8, 8, 0, 0, 0, 1, 1, 2, 1, 6, 6, 6, 6, 6 },
                new int[] { 6, 6, 6, 6, 6, 1, 2, 1, 1, 0, 1, 1, 1, 9, 9, 1, 1, 1, 0, 1, 1, 2, 1, 6, 6, 6, 6, 6 },
                new int[] { 1, 1, 1, 1, 1, 1, 2, 1, 1, 0, 1, 5, 5, 5, 5, 5, 5, 1, 0, 1, 1, 2, 1, 1, 1, 1, 1, 1 },
                new int[] { 7, 7, 7, 7, 7, 7, 2, 0, 0, 0, 1, 5, 5, 5, 5, 5, 5, 1, 0, 0, 0, 2, 7, 7, 7, 7, 7, 7 },
                new int[] { 1, 1, 1, 1, 1, 1, 2, 1, 1, 0, 1, 5, 5, 5, 5, 5, 5, 1, 0, 1, 1, 2, 1, 1, 1, 1, 1, 1 },
                new int[] { 6, 6, 6, 6, 6, 1, 2, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 2, 1, 6, 6, 6, 6, 6 },
                new int[] { 6, 6, 6, 6, 6, 1, 2, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 2, 1, 6, 6, 6, 6, 6 },
                new int[] { 6, 6, 6, 6, 6, 1, 2, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 2, 1, 6, 6, 6, 6, 6 },
                new int[] { 1, 1, 1, 1, 1, 1, 2, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 2, 1, 1, 1, 1, 1, 1 },
                new int[] { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1 },
                new int[] { 1, 2, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2, 1 },
                new int[] { 1, 2, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2, 1 },
                new int[] { 1, 3, 2, 2, 1, 1, 2, 2, 2, 2, 2, 10, 10, 8, 8, 10, 10, 2, 2, 2, 2, 2, 1, 1, 2, 2, 3, 1 },
                new int[] { 1, 1, 1, 2, 1, 1, 2, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 2, 1, 1, 1 },
                new int[] { 1, 1, 1, 2, 1, 1, 2, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 2, 1, 1, 1 },
                new int[] { 1, 2, 2, 2, 2, 2, 2, 1, 1, 2, 2, 2, 2, 1, 1, 2, 2, 2, 2, 1, 1, 2, 2, 2, 2, 2, 2, 1 },
                new int[] { 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1 },
                new int[] { 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1 },
                new int[] { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1 },
                new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                new int[] { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 },
                new int[] { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 }
            ];

        // Levels 1 through 21+ (index 0 = level 1, index 20 = level 21+)
        public static readonly int[] PacManSpeed = { 80, 90, 90, 90, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 90 };
        public static readonly int[] PacManDotsSpeed = { 71, 79, 79, 79, 87, 87, 87, 87, 87, 87, 87, 87, 87, 87, 87, 87, 87, 87, 87, 87, 79 };
        public static readonly int[] GhostSpeed = { 80, 85, 85, 85, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 };
        public static readonly int[] GhostTunnelSpeed = { 40, 45, 45, 45, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50 };

        public static readonly int[] Elroy1DotsLeft = { 20, 30, 40, 40, 40, 50, 50, 50, 60, 60, 60, 80, 80, 80, 100, 100, 100, 100, 120, 120, 120 };
        public static readonly int[] Elroy1Speed = { 80, 90, 90, 90, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
        public static readonly int[] Elroy2DotsLeft = { 10, 15, 20, 20, 20, 25, 25, 25, 30, 30, 30, 40, 40, 40, 50, 50, 50, 50, 60, 60, 60 };
        public static readonly int[] Elroy2Speed = { 85, 95, 95, 95, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105 };

        // 0 = no fright phase at this level (levels 17, 19, 20, 21+ — the "–" columns in the table)
        public static readonly int[] FrightPacManSpeed = { 90, 95, 95, 95, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 0, 100, 0, 0, 0 };
        public static readonly int[] FrightPacManDotsSpeed = { 79, 83, 83, 83, 87, 87, 87, 87, 87, 87, 87, 87, 87, 87, 87, 87, 0, 87, 0, 0, 0 };
        public static readonly int[] FrightGhostSpeed = { 50, 55, 55, 55, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 0, 60, 0, 0, 0 };
        public static readonly int[] FrightTimeSeconds = { 6, 5, 4, 3, 2, 5, 2, 2, 1, 5, 2, 1, 1, 3, 1, 5, 0, 1, 0, 0, 0 };
        public static readonly int[] FlashCount = { 5, 5, 5, 5, 5, 5, 5, 5, 3, 5, 5, 3, 3, 5, 3, 5, 0, 3, 0, 0, 0 };

        //  Local Limit Counters -> Blinky is already outside and Pinky goes first

        public static readonly int[] InkyLocalDotLimit = { 30, 0, 0 };
        // just add the first Inkys entries to Clyde Local Dot entries to simplify logic
        public static readonly int[] ClydeLocalDotLimit = { 90, 50, 0 };

        // For Fruits
        public static readonly FruitType[] BonusFruit =
        {   FruitType.Cherries,   // level 1
            FruitType.Strawberry, // level 2
            FruitType.Peach,      // level 3
            FruitType.Peach,      // level 4
            FruitType.Apple,      // level 5
            FruitType.Apple,      // level 6
            FruitType.Grapes,     // level 7
            FruitType.Grapes,     // level 8
            FruitType.Galaxian,   // level 9
            FruitType.Galaxian,   // level 10
            FruitType.Bell,       // level 11
            FruitType.Bell,       // level 12
            FruitType.Key,        // level 13
            FruitType.Key,        // level 14
            FruitType.Key,        // level 15
            FruitType.Key,        // level 16
            FruitType.Key,        // level 17
            FruitType.Key,        // level 18
            FruitType.Key,        // level 19
            FruitType.Key,        // level 20
            FruitType.Key,        // level 21+
        };
        public static readonly int[] BonusPoints ={ 100, 300, 500, 500, 700, 700, 1000, 1000, 2000, 2000,3000, 3000, 5000};
        public static int GetEntry(int Level, int[] array)
        {
            Level = Math.Min(Level, array.Length) - 1;
            return array[Level];
        }
        public static FruitType GetFruitEntry(int Level)
        {
            Level = Math.Min(Level, BonusFruit.Length) - 1;
            return BonusFruit[Level];
        }

    }
}