using System.Numerics;
using Raylib_cs;

namespace PacManGame
{
    class Program
    {
        const int TileSize = 8;
        const float DrawScale = 3f;

        static bool isPaused = true;
        static bool isFrozen;
        static int BufferFrames;
        static bool ResetGame;

        static int PacManAnimFrameIndex;
        static int GhostAnimFrameIndex;
        static bool playDeathAnim;    

        static int PacManAnimSwitcher;
        static int GhostAnimTimer;
        static int PelletAnimTimer;
        static int FruitPointsTimer;
        
        static Texture2D emptyBoardSheet;
        static Texture2D fullBoardSheet;
        static Texture2D spriteSheet;

        static void Main(string[] args)
        {

            int[][] board = LevelSpecs.board;
            isFrozen = false;
            PacManAnimFrameIndex = 2;
            Board board1 = new(board, TileSize, TileSize);
            Fruit fruit = new Fruit(13, 26, board1);
            PacMan pacman = new PacMan(13, 26, board1, fruit);
            Ghost blinky = new Ghost(13, 14, board1, 26, 0, pacman, GhostType.Blinky);
            Ghost pinky = new Ghost(13, 17, board1, 3, 0, pacman, GhostType.Pinky);
            Ghost inky = new Ghost(11, 17, board1, 28, 35, pacman, GhostType.Inky);
            Ghost clyde = new Ghost(15, 17, board1, 0, 35, pacman, GhostType.Clyde);
            inky.SetBlinky(blinky);
            List<Ghost> ghosts = [blinky, inky, clyde, pinky];
            LevelTimer timer = new LevelTimer(ghosts);
            pacman.SetGhosts(ghosts);
            pacman.SetTimer(timer);
            timer.SetCurrentLevel(board1.LEVEL);

            int screenWidth = (int)(board1.Grid.GetLength(1) * TileSize * DrawScale);
            int screenHeight = (int)(board1.Grid.GetLength(0) * TileSize * DrawScale) + 100;

            
            Raylib.InitWindow(screenWidth, screenHeight, "PacMan");

            spriteSheet = Raylib.LoadTexture("assets/AllSprites.png");
            emptyBoardSheet = Raylib.LoadTexture("assets/empty_board.png");
            fullBoardSheet = Raylib.LoadTexture("assets/full_board.png");
            
            Raylib.SetTargetFPS(60);

            while (!Raylib.WindowShouldClose())
            {
                if (Raylib.IsKeyPressed(KeyboardKey.P))
                    isPaused = !isPaused;

                if (!isPaused)
                {
                    if (ResetGame)
                    {
                        ResetGame = false;
                        GameReset(pacman);
                    }
                    CanChangeAnimation();
                    HandleInput(pacman);
                    pacman.UpdateLoop(isFrozen);
                       if (pacman.HasDied){
                        PacManAnimFrameIndex = 0;
                        playDeathAnim = true;
                        isFrozen = true;
                        pacman.HasDied = false;
                    }
                    if (!pacman.IsValidMove(pacman.direction) && !playDeathAnim)
                        PacManAnimFrameIndex = 1;
                    foreach (var ghost in ghosts)
                    {
                        ghost.Move(isFrozen);
                    }
                    if (!playDeathAnim)
                        pacman.CheckGhostCollisions();
                    timer.UpdateTimer();
                }
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                DrawBoard(board1);
                DrawFruit(fruit, board1.LEVEL);
                if (!playDeathAnim)
                    DrawGhosts(ghosts);
                DrawPacMan(pacman);
                if (isPaused)
                    DrawPauseOverlay();

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }

        static void HandleInput(PacMan pacman)
        {
            if (Raylib.IsKeyDown(KeyboardKey.Up) || Raylib.IsKeyDown(KeyboardKey.W))
                pacman.ChangeBufferDirection(Vector2D.Up);
            else if (Raylib.IsKeyDown(KeyboardKey.Down) || Raylib.IsKeyDown(KeyboardKey.S))
                pacman.ChangeBufferDirection(Vector2D.Down);
            else if (Raylib.IsKeyDown(KeyboardKey.Left) || Raylib.IsKeyDown(KeyboardKey.A))
                pacman.ChangeBufferDirection(Vector2D.Left);
            else if (Raylib.IsKeyDown(KeyboardKey.Right) || Raylib.IsKeyDown(KeyboardKey.D))
                pacman.ChangeBufferDirection(Vector2D.Right);
        }

        static void DrawBoard(Board board)
        {
            for (int row = 3; row < board.Grid.GetLength(0)-2; row++)
            {
                for (int col = 0; col < board.Grid.GetLength(1); col++)
                {
                    Tile tile = board.Grid[row, col];
                    float x = col * TileSize * DrawScale;
                    float y = row * TileSize * DrawScale;
                    float size = TileSize * DrawScale;
                    Rectangle src = new Rectangle(col*board.TileHeight, (row-3)*board.TileWidth, board.TileWidth, board.TileHeight);
                    Rectangle dest = new Rectangle(x, y, size, size);
                    Vector2 origin = Vector2.Zero;

                    if (tile.HasDot())
                        Raylib.DrawTexturePro(fullBoardSheet, src, dest, origin, 0f, Color.White);
                    else if (tile.HasPowerPellet())
                    {
                        if (PelletAnimTimer == 0)
                            Raylib.DrawTexturePro(emptyBoardSheet, src, dest, origin, 0f, Color.White);
                        else
                            Raylib.DrawTexturePro(fullBoardSheet, src, dest, origin, 0f, Color.White);
                    }
                    else
                        Raylib.DrawTexturePro(emptyBoardSheet, src, dest, origin, 0f, Color.White);

                }
            }
        }

        static void DrawPauseOverlay()
        {
            int screenWidth = Raylib.GetScreenWidth();
            int screenHeight = Raylib.GetScreenHeight();
            Color overlayColor = new Color(0, 0, 0, 180);
            Raylib.DrawRectangle(0, 0, screenWidth, screenHeight, overlayColor);
            string pauseText = "PAUSED";
            string resumeText = "Press 'P' to Resume";
            int fontSize = 60;
            int textWidth = Raylib.MeasureText(pauseText, fontSize);
            int textX = (screenWidth - textWidth) / 2;
            int textY = (screenHeight / 2) - 60;
            Raylib.DrawText(pauseText, textX, textY, fontSize, Color.Yellow);
            fontSize = 30;
            textWidth = Raylib.MeasureText(resumeText, fontSize);
            textX = (screenWidth - textWidth) / 2;
            textY = (screenHeight / 2) + 20;
            Raylib.DrawText(resumeText, textX, textY, fontSize, Color.White);
        }
        
        static void DrawFruit(Fruit fruit, int level)
        {
            if (!fruit.IsActive() && fruit.PointsTimer == 0)
                return;
            float screenX = fruit.PixelPosX * DrawScale;
            float screenY = fruit.PixelPosY * DrawScale;
            float size = TileSize * DrawScale * 2.0f;
            Rectangle src;
            if (fruit.PointsTimer == 0)
                src = Sprites.FruitSelector(LevelSpecs.GetFruitEntry(level));
            else
                src = Sprites.FruitPointsSelector(LevelSpecs.GetFruitEntry(level));
            Rectangle dest = new Rectangle(screenX, screenY, size, size);
            Vector2 origin = new Vector2(size / 2, size / 2); 
            Raylib.DrawTexturePro(spriteSheet, src, dest, origin, 0f, Color.White);
        }
        
        static void DrawPacMan(PacMan pacman)
        {
            float screenX = pacman.PixelPosX * DrawScale;
            float screenY = pacman.PixelPosY * DrawScale;
            float size = TileSize * DrawScale * 2.0f;
            Rectangle src;
            if (!playDeathAnim)
                src = Sprites.PacManDirectionSelector(pacman.direction)[PacManAnimFrameIndex];
            else
                src = Sprites.PacManDeathSelector()[PacManAnimFrameIndex];
            Rectangle dest = new Rectangle(screenX, screenY, size, size);
            Vector2 origin = new Vector2(size / 2, size / 2);
            Raylib.DrawTexturePro(spriteSheet, src, dest, origin, 0f, Color.White);
        }
        static void DrawGhosts(List<Ghost> ghosts)
        {
            foreach (var ghost in ghosts)
            {
                float screenX = ghost.PixelPosX * DrawScale;
                float screenY = ghost.PixelPosY * DrawScale;
                float size = TileSize * DrawScale * 2.0f; 
                Rectangle src;
                if (ghost.CurrentMode.Equals(ModeType.Dead))
                    src = Sprites.GhostDeadSelector(ghost.direction);
                else if (ghost.CurrentMode.Equals(ModeType.Fright))
                    src = Sprites.GhostFrightSelector(ghost.PacMan.timer.isBlue)[GhostAnimFrameIndex];
                else
                    src = Sprites.GhostTypeAndDirectionSelector(ghost.ghostType, ghost.direction)[GhostAnimFrameIndex];
                Rectangle dest = new Rectangle(screenX, screenY, size, size);
                Vector2 origin = new Vector2(size / 2, size / 2); 
                Raylib.DrawTexturePro(spriteSheet, src, dest, origin, 0f, Color.White);
            }
        }

        static void DrawHud(Board board, PacMan pacman, List<Ghost> ghosts, LevelTimer timer)
        {
            int hudY = board.Grid.GetLength(0) * TileSize * (int)DrawScale + 5;
            string line1 = $"Score: {board.Score}   Lives: {pacman.LIVES}   Dots left: {board.RemainingDots}  FPS: {Raylib.GetFPS()}";
            Raylib.DrawText(line1, 10, hudY, 20, Color.White);
            (int tileX, int tileY) = pacman.ConvertPixelToTile(pacman.PixelPosX, pacman.PixelPosY);
            string dir = pacman.direction.Equals(Vector2D.Zero) ? "None" :
                        pacman.direction.Equals(Vector2D.Up) ? "Up" :
                        pacman.direction.Equals(Vector2D.Down) ? "Down" :
                        pacman.direction.Equals(Vector2D.Left) ? "Left" : "Right";
            string buf = pacman.bufferDirection.Equals(Vector2D.Zero) ? "None" :
                        pacman.bufferDirection.Equals(Vector2D.Up) ? "Up" :
                        pacman.bufferDirection.Equals(Vector2D.Down) ? "Down" :
                        pacman.bufferDirection.Equals(Vector2D.Left) ? "Left" : "Right";
            string line2 = $"Tile: ({tileX}, {tileY})  Dir: {dir}  Buffer: {buf}";
            Raylib.DrawText(line2, 10, hudY + 25, 20, Color.LightGray);
            string modeText = timer.GetCurrentMode().ToString();
            float timeRemaining;
            if (timer.GetCurrentMode() == ModeType.Fright)
            {
                timeRemaining = timer.FrightTimer / 60f;
            }
            else
            {
                int phaseIndex = timer.ModeTimerIndex;
                int phaseLengthFrames = phaseIndex < timer.GlobalSchedule[timer.GetGlobalLevelIndex()].Length
                    ? timer.GlobalSchedule[timer.GetGlobalLevelIndex()][phaseIndex]
                    : 0;
                int framesRemaining = phaseLengthFrames - timer.ModeTimer;
                timeRemaining = framesRemaining > 0 ? framesRemaining / 60f : 0f;
            }

            string line3 = $"Ghost Mode: {modeText}  Time remaining: {timeRemaining:F1}s";
            Raylib.DrawText(line3, 10, hudY + 50, 20, Color.Orange);

            // Show ghost positions with their names
            string ghostInfo = "Ghosts: ";
            foreach (var ghost in ghosts)
            {
                (int gTileX, int gTileY) = ghost.ConvertPixelToTile(ghost.PixelPosX, ghost.PixelPosY);
                string ghostName = ghost.ghostType.ToString();
                // Abbreviate for space
                ghostName = ghostName.Length > 0 ? ghostName.Substring(0, 1) : "?";
                ghostInfo += $"{ghostName}({gTileX},{gTileY}) ";
            }
            string line4 = ghostInfo;
            Raylib.DrawText(line4, 10, hudY + 75, 20, Color.LightGray);

            // Add pause status to HUD
            string pauseStatus = isPaused ? "PAUSED" : "RUNNING";
            Color pauseColor = isPaused ? Color.Red : Color.Green;
            string line5 = $"Status: {pauseStatus} (Press P to toggle)";
            Raylib.DrawText(line5, 10, hudY + 100, 20, pauseColor);
        }
        public static void CanChangeAnimation()
        {

            if (!playDeathAnim)
                IncrementPacManTimers(Sprites.PacmanDirectionList[0].Count, 1);
            else
                IncrementPacManTimers(Sprites.PacManDead.Count, 10);
            IncrementGhostTimer(Sprites.BlinkyDirectionList[0].Count, 3);
            
            if (PelletAnimTimer == 0)
                    PelletAnimTimer = 25;
            else
                PelletAnimTimer--;
        }
        public static void GameReset(PacMan pacMan)
        {
            playDeathAnim = false;
            isFrozen = false;
            PacManAnimFrameIndex = Sprites.PacmanDirectionList[0].Count - 1;
            pacMan.ResetGame();
        }

        public static void IncrementPacManTimers(int AvailableFrames, int FrameBuffer)
        {
            if (PacManAnimSwitcher == 0)
            {
                PacManAnimFrameIndex = (PacManAnimFrameIndex + 1) % AvailableFrames;
                if (PacManAnimFrameIndex+1 == Sprites.PacManDead.Count)
                {
                    ResetGame = true;
                }
                PacManAnimSwitcher = FrameBuffer;
            }
            else
                PacManAnimSwitcher--;
        }
        public static bool IsBufferFrame()
        {
            if (BufferFrames == 0)
                return false;
            else
                BufferFrames--;
            return true;
        }
        public static void IncrementGhostTimer(int AvailableFrames, int FrameBuffer)
        {
            if (GhostAnimTimer == 0)
            {
                GhostAnimFrameIndex = (GhostAnimFrameIndex + 1) % AvailableFrames;
                GhostAnimTimer = FrameBuffer;
            }
            else
                GhostAnimTimer--;
        }
        
    }
}