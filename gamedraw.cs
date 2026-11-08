using System.Numerics;
using System.Runtime.InteropServices;
using Raylib_cs;

namespace PacManGame
{
    public static class GameDraw
    {

        const float ArtOffsetX = 1f;
        const int TileSize = 8;
        const float DrawScale = 3f;
        static Texture2D spriteSheet = Raylib.LoadTexture("assets/sprites/AllSprites.png");
        static Texture2D emptyBoardSheet = Raylib.LoadTexture("assets/sprites/empty_board.png");
        static Texture2D fullBoardSheet = Raylib.LoadTexture("assets/sprites/full_board.png");
        static Texture2D textSheet = Raylib.LoadTexture("assets/sprites/text.png");
        static Texture2D whiteSheet = Raylib.LoadTexture("assets/sprites/white_board.png");
        
        // Draws a sprite at its native sheet size (16x16, or 20x16 for some point values) times
        // DrawScale, centred on a position given in game pixels. Size comes from the source
        // rectangle, so no hardcoded multiplier is needed and non-square sprites keep their shape.
        public static void DrawSpriteCentered(Rectangle src, float gameX, float gameY, float rotation = 0f)
        {
            Rectangle dest = new Rectangle(gameX * DrawScale, gameY * DrawScale,
                                           src.Width * DrawScale, src.Height * DrawScale);
            Vector2 origin = new Vector2(dest.Width / 2, dest.Height / 2);
            Raylib.DrawTexturePro(spriteSheet, src, dest, origin, rotation, Color.White);
        }

        // Same as above but anchored at the top-left corner of a tile (used for the HUD row).
        public static void DrawSpriteAtTile(Rectangle src, int col, int row)
        {
            Rectangle dest = new Rectangle(col * TileSize * DrawScale, row * TileSize * DrawScale,
                                           src.Width * DrawScale, src.Height * DrawScale);
            Raylib.DrawTexturePro(spriteSheet, src, dest, Vector2.Zero, 0f, Color.White);
        }


        // Drawing Board logic need to make it optimized: 
        static bool IsPelletVisible()
        {
            int value = TimerManager.GetValue(TimerType.PelletAnim);
            const int halfPeriod = 15; // frames per on/off phase — tune to taste
            return (value / halfPeriod) % 2 == 0;
        }
        public static void DrawBoardOnce(Board board)
        {
            for (int row = 3; row < board.Grid.GetLength(0) - 2; row++)
            {
                for (int col = 0; col < board.Grid.GetLength(1); col++)
                {
                    Tile tile = board.Grid[row, col];
                    float x = col * TileSize * DrawScale;
                    float y = row * TileSize * DrawScale;
                    float size = TileSize * DrawScale;
                    Rectangle src = new Rectangle(col * board.TileHeight, (row - 3) * board.TileWidth, board.TileWidth, board.TileHeight);
                    Rectangle dest = new Rectangle(x, y, size, size);
                    Vector2 origin = Vector2.Zero;

                    if (tile.HasDot())
                        Raylib.DrawTexturePro(fullBoardSheet, src, dest, origin, 0f, Color.White);
                    else
                        Raylib.DrawTexturePro(emptyBoardSheet, src, dest, origin, 0f, Color.White);
                }
            }
        }
        public static void DrawBoard(Board board)
        {
            bool levelStartActive = !TimerManager.IsPaused(TimerType.LevelStart);
            int levelStartValue = TimerManager.GetValue(TimerType.LevelStart);

            const int flashInterval = 15;
            bool showWhite = (levelStartValue / flashInterval) % 2 == 0;

            for (int row = 3; row < board.Grid.GetLength(0) - 2; row++)
            {
                for (int col = 0; col < board.Grid.GetLength(1); col++)
                {
                    Tile tile = board.Grid[row, col];
                    float x = col * TileSize * DrawScale;
                    float y = row * TileSize * DrawScale;
                    float size = TileSize * DrawScale;
                    Rectangle src = new Rectangle(col * board.TileHeight, (row - 3) * board.TileWidth, board.TileWidth, board.TileHeight);
                    Rectangle dest = new Rectangle(x, y, size, size);
                    Vector2 origin = Vector2.Zero;

                    if (levelStartActive)
                    {
                        Texture2D sheet = showWhite ? whiteSheet : emptyBoardSheet;
                        Raylib.DrawTexturePro(sheet, src, dest, origin, 0f, Color.White);
                        continue;
                    }

                    if (tile.HasDot())
                        Raylib.DrawTexturePro(fullBoardSheet, src, dest, origin, 0f, Color.White);
                    else if (tile.HasPowerPellet() && IsPelletVisible())
                        Raylib.DrawTexturePro(fullBoardSheet, src, dest, origin, 0f, Color.White);
                    else
                        Raylib.DrawTexturePro(emptyBoardSheet, src, dest, origin, 0f, Color.White);
                }
            }
        }

        // Drawing all entities 
        public static void DrawPacMan(PacMan pacman, int PacManAnimFrameIndex, bool playDeathAnim)
        {
            Rectangle src;
            float rotation = 0f;
            if (pacman.IsGhostDead())
                src = new Rectangle(0, 0, 0, 0);
            else if (!playDeathAnim)
                src = Sprites.PacManDirectionSelector(pacman.direction)[PacManAnimFrameIndex];
            else
                src = Sprites.PacManDeathSelector()[PacManAnimFrameIndex];

            DrawSpriteCentered(src, pacman.PixelPosX, pacman.PixelPosY, rotation);
        }

        // Drawing ghost with the 1px offset problem being solved here
        public static void DrawGhosts(List<Ghost> ghosts, int GhostAnimFrameIndex)
        {
            foreach (var ghost in ghosts)
            {
                Rectangle src;
                if (ghost.DeathState.Equals(DiedTransitionState.JustDied))
                    src = Sprites.GhostPointsSelector(ghost.PacMan.EatenGhostsCounter);
                else if (ghost.CurrentMode.Equals(ModeType.Dead))
                    src = Sprites.GhostDeadSelector(ghost.direction);
                else if (ghost.CurrentMode.Equals(ModeType.Fright))
                    src = Sprites.GhostFrightSelector(ghost.PacMan.timer.isBlue)[GhostAnimFrameIndex];
                else
                    src = Sprites.GhostTypeAndDirectionSelector(ghost.ghostType, ghost.direction)[GhostAnimFrameIndex];
                // Ghost art is 14px wide inside a 16px cell with the blank columns on the LEFT
                // (cols 2-15), so its visual centre sits 1px right of the cell centre. Shift left
                // by 1 game pixel so ghosts are centred in corridors and symmetric in the house.
                DrawSpriteCentered(src, ghost.PixelPosX - ArtOffsetX, ghost.PixelPosY);
            }
        }

        public static void DrawFruit(Fruit fruit, int level)
        {
            if (!fruit.IsActive() && fruit.PointsTimer == 0)
                return;
            Rectangle src;
            if (fruit.PointsTimer == 0)
                src = Sprites.FruitSelector(LevelSpecs.GetFruitEntry(level));
            else
                src = Sprites.FruitPointsSelector(LevelSpecs.GetFruitEntry(level));
            // Points sprites have different margins, so only the fruit art gets the shift
            float offsetX = fruit.PointsTimer == 0 ? ArtOffsetX : 0f;
            DrawSpriteCentered(src, fruit.PixelPosX - offsetX, fruit.PixelPosY);
        }

        // Drawing HUD elements here
        public static void DrawBottom(int LIVES, int Level)
        {
            int startCol = 3;
            int endCol = 25;
            int startRow = 34;
            int spacing = 2;

            for (int i = 0; i < LIVES; i++)
            {
                DrawPacManLife(startCol + (i * spacing), startRow);
            }
            Level = Math.Min(Level, 21);

            // Show at most the last 7 levels' fruit, never going below level 1
            int startLevel = Math.Max(1, Level - 6);

            int j = 0;
            for (int lvl = startLevel; lvl <= Level; lvl++)
            {
                DrawBottomFruit(lvl, endCol - (j * spacing), 34);
                j++;
            }
        }
        static void DrawBottomFruit(int Level, int col, int row)
        {
            DrawSpriteAtTile(Sprites.FruitSelector(LevelSpecs.GetFruitEntry(Level)), col, row);
        }
        static void DrawPacManLife(int col, int row)
        {
            DrawSpriteAtTile(Sprites.GetPacManLife(), col, row);
        }

        // Drawing Messages here
        public static void DrawAllMessages(Board board1, PacMan pacman)
        {
            if (!TimerManager.IsPaused(TimerType.StartTimer))
                DrawMessage(ScreenMessages.PlayerOne, TextColor.Cyan);
            if (!TimerManager.IsPaused(TimerType.GameStart))
                DrawMessage(ScreenMessages.Ready, TextColor.Yellow);
            if (!TimerManager.IsPaused(TimerType.GameOver))
                DrawMessage(ScreenMessages.GameOver, TextColor.Red);
            DrawMessage(ScreenMessages.HighScore, TextColor.White);
            DrawMessage(ScreenMessages.OneUp, TextColor.White);
            DrawMessage(ScreenMessages.MyMessage, TextColor.Pink);
            DrawMessage(ScreenMessages.MyName, TextColor.White);
            DrawMessage(ScreenMessages.GetScore(board1.Score, 2, 1), TextColor.White);
            DrawMessage(ScreenMessages.GetScore(board1.HighScore, 10, 1), TextColor.White);
            DrawBottom(pacman.LIVES - 1, board1.LEVEL);
        }
        static void DrawMessage(Dictionary<(int col, int row), char> message, TextColor color)
        {
            foreach (var kvp in message)
            {
                int col = kvp.Key.col;
                int row = kvp.Key.row;
                char c = kvp.Value;

                // Calculate screen position
                float x = col * TileSize * DrawScale;
                float y = row * TileSize * DrawScale;
                float size = TileSize * DrawScale;

                // Get the source rectangle from TextSprites
                Rectangle src = TextSprites.GetChar(color, c);
                Rectangle dest = new Rectangle(x, y, size, size);
                Vector2 origin = Vector2.Zero;

                Raylib.DrawTexturePro(textSheet, src, dest, origin, 0f, Color.White);
            }
        }
    }
}