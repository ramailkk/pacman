using Raylib_cs;

namespace PacManGame
{
    public enum TextColor
    {
        White,
        Red,
        Pink,
        Cyan,
        Orange,
        Salmon,
        Yellow
    }

    static class TextSprites
    {
        private const int TileSize = 8;

        // Top-to-bottom order of the solid-color blocks in the sprite sheet.
        // (Verified by sampling one pixel per block: each block is a single
        // flat RGB value, no anti-aliasing.)
        private static readonly TextColor[] ColorOrder =
        {
            TextColor.White,   // (224,221,255)
            TextColor.Red,     // (255,  0,  0)
            TextColor.Pink,    // (252,181,255)
            TextColor.Cyan,    // (  0,255,255)
            TextColor.Orange,  // (248,187, 85)
            TextColor.Salmon,  // (250,185,176)
            TextColor.Yellow,  // (255,255,  0)
        };

        // Each color block is 4 rows of 8x8 tiles, 16 tiles wide.
        // These are the characters left-to-right in each row.
        // Row 1's trailing "PTS" (cols 13-15) duplicates the P/T/S glyphs
        // already placed at cols 0/4/3, so it's handled separately below
        // instead of overwriting those dictionary entries.
        private static readonly string[] RowChars =
        {
            "ABCDEFGHIJKLMNO",     // row 0, cols 0-14
            "PQRSTUVWXYZ!\u00A9",  // row 1, cols 0-12  (\u00A9 = ©)
            "0123456789/-\"",      // row 2, cols 0-12
            "namco",                // row 3, cols 0-4
        };

        private const int PtsRow = 1;
        private const int PtsStartCol = 13;

        public static readonly Dictionary<TextColor, Dictionary<char, Rectangle>> CharMap;
        public static readonly Dictionary<TextColor, List<Rectangle>> PtsMap;

        static TextSprites()
        {
            CharMap = new Dictionary<TextColor, Dictionary<char, Rectangle>>();
            PtsMap = new Dictionary<TextColor, List<Rectangle>>();

            for (int blockIndex = 0; blockIndex < ColorOrder.Length; blockIndex++)
            {
                TextColor color = ColorOrder[blockIndex];
                int blockY = blockIndex * TileSize * RowChars.Length;

                var charDict = new Dictionary<char, Rectangle>();

                for (int row = 0; row < RowChars.Length; row++)
                {
                    string chars = RowChars[row];
                    int y = blockY + row * TileSize;

                    for (int col = 0; col < chars.Length; col++)
                    {
                        int x = col * TileSize;
                        charDict[chars[col]] = new Rectangle(x, y, TileSize, TileSize);
                    }
                }

                CharMap[color] = charDict;

                // "PTS" tile trio, used as one contiguous chunk for score text
                int ptsY = blockY + PtsRow * TileSize;
                PtsMap[color] = new List<Rectangle>
                {
                    new Rectangle((PtsStartCol + 0) * TileSize, ptsY, TileSize, TileSize),
                    new Rectangle((PtsStartCol + 1) * TileSize, ptsY, TileSize, TileSize),
                    new Rectangle((PtsStartCol + 2) * TileSize, ptsY, TileSize, TileSize),
                };
            }
        }

        // Looks up a single glyph, falling back to the opposite case
        // (letters are uppercase except "namco", which is lowercase).
        public static Rectangle GetChar(TextColor color, char ch)
        {
            var dict = CharMap[color];
            if (dict.TryGetValue(ch, out Rectangle rect))
                return rect;

            char alt = char.IsUpper(ch) ? char.ToLowerInvariant(ch) : char.ToUpperInvariant(ch);
            return dict[alt];
        }

        // Full run of rectangles for an arbitrary string, e.g. "100" or "PACMAN".
        public static List<Rectangle> GetString(TextColor color, string text)
        {
            var rects = new List<Rectangle>(text.Length);
            foreach (char ch in text)
                rects.Add(GetChar(color, ch));
            return rects;
        }
        public static List<Rectangle> GetPts(TextColor color) => PtsMap[color];
    }
}