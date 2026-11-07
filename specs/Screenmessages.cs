namespace PacManGame
{
    static class ScreenMessages
    {
        // Builds a (col,row) -> char map for a horizontal string starting at (startCol, startRow), one character per column.
        public static Dictionary<(int col, int row), char> BuildMessage(string text, int startCol, int startRow)
        {
            var map = new Dictionary<(int col, int row), char>();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ')
                    continue;

                map[(startCol + i, startRow)] = text[i];
            }
            return map;
        }

        // "READY!" — shown when a level/life starts, centered in the maze.
        public static readonly Dictionary<(int col, int row), char> Ready =
            BuildMessage("READY!", 11, 20);

        // "PAUSED!" — same slot as READY!, shown while the game is paused.
        public static readonly Dictionary<(int col, int row), char> Paused =
            BuildMessage("PAUSED!", 11, 20);

        // "GAME OVER" — same slot as READY!, shown on game over (should be drawn red).
        public static readonly Dictionary<(int col, int row), char> GameOver =
            BuildMessage("GAME  OVER", 9, 20);

        // "PLAYER ONE" — shown once at the start of a new game, above the maze.
        public static readonly Dictionary<(int col, int row), char> PlayerOne =
            BuildMessage("PLAYER ONE", 9, 14);

        // "1UP" — static label, top-left of the HUD row above the maze.
        public static readonly Dictionary<(int col, int row), char> OneUp =
            BuildMessage("1UP", 2, 0);

        // "2UP" — static label, top-right; only shown in 2-player mode.
        public static readonly Dictionary<(int col, int row), char> TwoUp =
            BuildMessage("2UP", 22, 0);

        // "HIGH SCORE" — static label, top-center of the HUD row.
        public static readonly Dictionary<(int col, int row), char> HighScore =
            BuildMessage("HIGH SCORE", 7, 0);

        // "CREDIT" — bottom-left, followed by the credit count digit(s).
        public static readonly Dictionary<(int col, int row), char> Credit =
            BuildMessage("CREDIT", 0, 35);
        public static readonly Dictionary<(int col, int row), char> ScoreLabel =
            BuildMessage("SCORE", 3, 1);
        public static readonly Dictionary<(int col, int row), char> MyMessage =
            BuildMessage("LOVE FROM", 19, 0);
        public static readonly Dictionary<(int col, int row), char> MyName =
            BuildMessage("RAMAIL", 21, 1);
        public static Dictionary<(int col, int row), char> GetScore(int score, int col, int row)
        {
            String scoreString = score.ToString();
            if (scoreString.Length > Math.Abs(col - 28))
            {
                col = 28 - scoreString.Length;
            }
            return BuildMessage(scoreString, col, row);
        }
    }
}