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
        static bool ResetGame;

        static int PacManAnimFrameIndex;
        static int GhostAnimFrameIndex;
        static bool playDeathAnim;
        static Texture2D emptyBoardSheet;
        static Texture2D fullBoardSheet;
        static Texture2D spriteSheet;
        static Texture2D textSheet;

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

            TimerManager.Initialize();
            
           
            int screenWidth = (int)(board1.Grid.GetLength(1) * TileSize * DrawScale);
            int screenHeight = (int)(board1.Grid.GetLength(0) * TileSize * DrawScale);

            
            Raylib.InitWindow(screenWidth, screenHeight, "PacMan");

            spriteSheet = Raylib.LoadTexture("assets/AllSprites.png");
            emptyBoardSheet = Raylib.LoadTexture("assets/empty_board.png");
            fullBoardSheet = Raylib.LoadTexture("assets/full_board.png");
            textSheet = Raylib.LoadTexture("assets/text.png");

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
                        bool anyGhostEaten = ghosts.Any(g => g.HasDied);
                        if (anyGhostEaten)
                        {
                            TimerManager.Resume(TimerType.GhostEaten);
                            if (TimerManager.IsDone(TimerType.GhostEaten))
                            {
                                TimerManager.PauseAndReset(TimerType.GhostEaten);
                                foreach (var g in ghosts)
                                    g.HasDied = false;
                                anyGhostEaten = false;   // pause is over as of this frame
                            }
            
                        }
                        isFrozen = TimerManager.IsRunning(TimerType.GameStart)  || TimerManager.IsRunning(TimerType.StartTimer) || playDeathAnim || anyGhostEaten || pacman.HasDied || !TimerManager.IsPaused(TimerType.LevelStart) || !TimerManager.IsPaused(TimerType.GameOver);

                        // 2. Everyone uses that same, already-final isFrozen value.
                        CanChangeAnimation();
                        HandleInput(pacman);
                        pacman.UpdateLoop(isFrozen);
                        if (board1.RemainingDots == 0)
                        {   PacManAnimFrameIndex = 2;
                            TimerManager.Resume(TimerType.LevelStart);
                            if (TimerManager.IsDone(TimerType.LevelStart))
                            {
                                TimerManager.PauseAndReset(TimerType.LevelStart);
                                pacman.ResetForNextLevel();
                                TimerManager.ResumeAndReset(TimerType.GameStart);
                            }
                        }
                        if (!pacman.IsValidMove(pacman.direction) && !playDeathAnim)
                            PacManAnimFrameIndex = 1;

                        foreach (var ghost in ghosts)
                            ghost.Move(isFrozen);

                        if (!playDeathAnim)
                            pacman.CheckGhostCollisions();
                        
                        if (pacman.HasDied)
                        {
                            TimerManager.Resume(TimerType.PacManDeath);
                            if (TimerManager.IsDone(TimerType.PacManDeath))
                        {
                            TimerManager.PauseAndReset(TimerType.PacManDeath);
                            PacManAnimFrameIndex = 0;
                            playDeathAnim = true;
                            pacman.HasDied = false;
                        }
                    }
                        if (pacman.LIVES == 0 && !playDeathAnim && !pacman.HasDied)
                        {
                            TimerManager.Pause(TimerType.GameStart);
                            TimerManager.Resume(TimerType.GameOver);
                            if (TimerManager.IsDone(TimerType.GameOver))
                            {
                                TimerManager.PauseAndReset(TimerType.GameOver);
                                return;   
                            }
                        }

                        timer.UpdateTimer(isFrozen);
                        TimerManager.Update();
                    }
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                DrawBoard(board1);
                DrawFruit(fruit, board1.LEVEL);
                if (!playDeathAnim && !TimerManager.IsRunning(TimerType.StartTimer) && TimerManager.IsPaused(TimerType.LevelStart) && TimerManager.IsPaused(TimerType.GameOver))
                    DrawGhosts(ghosts);
                if (!TimerManager.IsRunning(TimerType.StartTimer) && TimerManager.IsPaused(TimerType.GameOver))
                    DrawPacMan(pacman);
                if (isPaused)
                    DrawPauseOverlay();
                DrawAllMessages();
                DrawMessage(ScreenMessages.GetScore(board1.Score,26,1),TextColor.White);
                DrawMessage(ScreenMessages.GetScore(board1.Score,12,1),TextColor.White);
                DrawBottom(pacman.LIVES, board1.LEVEL);
                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
        static void DrawAllMessages()
        {
            if (!TimerManager.IsPaused(TimerType.StartTimer))
                DrawMessage(ScreenMessages.PlayerOne, TextColor.Cyan);
            if (!TimerManager.IsPaused(TimerType.GameStart))
                DrawMessage(ScreenMessages.Ready, TextColor.Yellow);
            if (!TimerManager.IsPaused(TimerType.GameOver))
                DrawMessage(ScreenMessages.GameOver, TextColor.Red);
            DrawMessage(ScreenMessages.HighScore, TextColor.White);
            DrawMessage(ScreenMessages.OneUp, TextColor.White);
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
                    else if (tile.HasPowerPellet() && TimerManager.IsRunning(TimerType.PelletAnim))
                        Raylib.DrawTexturePro(fullBoardSheet, src, dest, origin, 0f, Color.White);
                    else
                        Raylib.DrawTexturePro(emptyBoardSheet, src, dest, origin, 0f, Color.White);
                }
            }
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
                float size = TileSize * DrawScale ;
                
                // Get the source rectangle from TextSprites
                Rectangle src = TextSprites.GetChar(color, c);
                Rectangle dest = new Rectangle(x, y, size, size);
                Vector2 origin = Vector2.Zero;
                
                Raylib.DrawTexturePro(textSheet, src, dest, origin, 0f, Color.White);
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
            if (pacman.IsGhostDead())
                src = new Rectangle(0, 0, 0, 0);
            else if (!playDeathAnim)
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
                if (ghost.HasDied)
                    src = Sprites.GhostPointsSelector(ghost.PacMan.EatenGhostsCounter);
                else if (ghost.CurrentMode.Equals(ModeType.Dead))
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

        static void DrawBottom(int LIVES, int Level)
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
            float x = col * TileSize * DrawScale;
            float y = row * TileSize * DrawScale;
            float size = TileSize * DrawScale * 2.0f; // 2x size of normal tile
            Rectangle src = Sprites.FruitSelector(LevelSpecs.GetFruitEntry(Level));
            Rectangle dest = new Rectangle(x, y, size, size);
            Vector2 origin = Vector2.Zero;
            Raylib.DrawTexturePro(spriteSheet, src, dest, origin, 0f, Color.White);
        }
        static void DrawPacManLife(int col, int row)
        {
            float x = col * TileSize * DrawScale;
            float y = row * TileSize * DrawScale;
            float size = TileSize * DrawScale * 2.0f; // 2x size of normal tile
            Rectangle src = Sprites.GetPacManLife();
            Rectangle dest = new Rectangle(x, y, size, size);
            Vector2 origin = Vector2.Zero;
            Raylib.DrawTexturePro(spriteSheet, src, dest, origin, 0f, Color.White);
        }
        public static void CanChangeAnimation()
        {
            if (isFrozen && !playDeathAnim)
            {
                TimerManager.Pause(TimerType.PacManAnim);
                TimerManager.Pause(TimerType.GhostAnim);
                return;
            }
            TimerManager.Resume(TimerType.PacManAnim);
            TimerManager.Resume(TimerType.GhostAnim);

            if (!playDeathAnim)
                IncrementPacManTimers(Sprites.PacmanDirectionList[0].Count, 2);
            else
                IncrementPacManTimers(Sprites.PacManDead.Count, 10);

            IncrementGhostTimer(Sprites.BlinkyDirectionList[0].Count);
        
            if (TimerManager.IsDone(TimerType.PelletAnim))
                    TimerManager.ResetTimer(TimerType.PelletAnim);
        }
        public static void GameReset(PacMan pacMan)
        {
            playDeathAnim = false;
            isFrozen = false;
            PacManAnimFrameIndex = Sprites.PacmanDirectionList[0].Count - 1;
            TimerManager.ResumeAndReset(TimerType.GameStart);
            pacMan.ResetGame();
        }

        public static void IncrementPacManTimers(int AvailableFrames, int FrameBuffer)
        {
            if (TimerManager.IsDone(TimerType.PacManAnim))
            {
                PacManAnimFrameIndex = (PacManAnimFrameIndex + 1) % AvailableFrames;
                if (PacManAnimFrameIndex+1 == Sprites.PacManDead.Count)
                    ResetGame = true;
                TimerManager.ResetTimer(TimerType.PacManAnim, FrameBuffer);
            }
        }
        public static void IncrementGhostTimer(int AvailableFrames)
        {
            if (TimerManager.IsDone(TimerType.GhostAnim)){
                GhostAnimFrameIndex = (GhostAnimFrameIndex + 1) % AvailableFrames;
                TimerManager.ResetTimer(TimerType.GhostAnim);
            }
        }
    }
}