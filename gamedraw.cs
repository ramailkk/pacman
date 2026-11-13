using System.Numerics;
using System.Runtime.InteropServices;
using Raylib_cs;

namespace PacManGame
{
    public static class GameDraw
    {

        const float ArtOffsetX = 1f;
        public const int TileSize = 8;
        public const float DrawScale = 2f;
        const int flashInterval = 15;       // power pellet blink: frames per on/off half-cycle
        const int levelFlashInterval = 15;  // level-end board flash: frames per white/blue half-cycle

        const int BoardSheetRowOffset = 3; // the board PNGs start at grid row 3 (rows 0-2 are the score area)
        static RenderTexture2D boardTex;
        static bool boardTexLoaded = false;
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


        // The board is drawn ONCE into an off-screen texture (boardTex) at game-pixel size (224x288).
        // Every frame that texture is drawn to the screen in a single call, scaled by DrawScale.
        // When a dot is eaten only that one tile is painted over inside the texture.

        // Call once after Raylib.InitWindow (render textures need a window/GL context).
        public static void InitBoardTexture(Board board)
        {
            boardTex = Raylib.LoadRenderTexture(board.Grid.GetLength(1) * TileSize,
                                                board.Grid.GetLength(0) * TileSize);
            boardTexLoaded = true;
            board.NeedsFullRedraw = true;
        }

        public static void UnloadBoardTexture()
        {
            if (boardTexLoaded)
                Raylib.UnloadRenderTexture(boardTex);
            boardTexLoaded = false;
        }

        // Call every frame BEFORE Raylib.BeginDrawing. Re-bakes the whole board when needed
        // (start / new level), otherwise only erases tiles that were eaten since last frame.
        public static void UpdateBoardTexture(Board board)
        {
            if (!board.NeedsFullRedraw && board.ClearedTiles.Count == 0)
                return;

            Raylib.BeginTextureMode(boardTex);
            if (board.NeedsFullRedraw)
            {
                Raylib.ClearBackground(Color.Black);
                Raylib.DrawTexture(fullBoardSheet, 0, BoardSheetRowOffset * TileSize, Color.White);
                board.NeedsFullRedraw = false;
            }
            foreach (var (row, col) in board.ClearedTiles)
            {
                Rectangle src = BoardSheetSrc(col, row);
                Raylib.DrawTextureRec(emptyBoardSheet, src, new Vector2(col * TileSize, row * TileSize), Color.White);
            }
            board.ClearedTiles.Clear();
            Raylib.EndTextureMode();
        }

        // Draws the baked board (one draw call) plus the blinking power pellets on top.
        // During the level-start flash the plain white/blue boards are drawn instead.
        public static void DrawBoard(Board board, bool levelStartActive)
        {
            if (levelStartActive)
            {
                // Divide first, then take % 2: the colour holds for levelFlashInterval frames before switching
                bool isWhite = TimerManager.GetValue(TimerType.LevelStart) / levelFlashInterval % 2 == 0;
                Texture2D sheet = isWhite ? whiteSheet : emptyBoardSheet;

                Vector2 pos = new Vector2(0, BoardSheetRowOffset * TileSize * DrawScale);
                Raylib.DrawTextureEx(sheet, pos, 0f, DrawScale, Color.White);
                return;
            }

            Texture2D t = boardTex.Texture;
            Rectangle src = new Rectangle(0, 0, t.Width, -t.Height); // render textures are stored upside down
            Rectangle dest = new Rectangle(0, 0, t.Width * DrawScale, t.Height * DrawScale);
            Raylib.DrawTexturePro(t, src, dest, Vector2.Zero, 0f, Color.White);

            DrawPowerPellets(board);
        }

        // Pellets blink, so they're drawn every frame on top of the baked board:
        // the full-board tile when visible, the empty-board tile to hide it.
        static void DrawPowerPellets(Board board)
        {
            bool isPelletVisible = TimerManager.GetValue(TimerType.PelletAnim) / flashInterval % 2 == 0;
            foreach (var (row, col) in board.PowerPelletsCords)
            {
                if (!board.Grid[row, col].HasPowerPellet())
                    continue; // already erased from the baked board
                Texture2D sheet = isPelletVisible ? fullBoardSheet : emptyBoardSheet;
                Rectangle dest = new Rectangle(col * TileSize * DrawScale, row * TileSize * DrawScale,
                                               TileSize * DrawScale, TileSize * DrawScale);
                Raylib.DrawTexturePro(sheet, BoardSheetSrc(col, row), dest, Vector2.Zero, 0f, Color.White);
            }
        }

        // Source rectangle of one grid tile inside the board PNGs.
        static Rectangle BoardSheetSrc(int col, int row)
        {
            return new Rectangle(col * TileSize, (row - BoardSheetRowOffset) * TileSize, TileSize, TileSize);
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