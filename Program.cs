using System.Numerics;
using System.Runtime.InteropServices;
using Raylib_cs;

namespace PacManGame
{
    enum LevelTransitionState { None, LevelEnding, LevelStarting }
    class Program
    {
        static bool isFrozen = false; // to ensure all entities remain in place
        static bool ResetGame = false; // flag for reseting games
        static int PacManAnimFrameIndex = 2; // to switch pacmans sprite animation where 2 is the starting circle
        static int GhostAnimFrameIndex;  // to switch ghosts sprite animation 
        static bool playDeathAnim = false; // to play pacmans death animation
        static LevelTransitionState transitionState = LevelTransitionState.None;
        static void Main(string[] args)
        {   
            Board board = new(LevelSpecs.board, GameDraw.TileSize, GameDraw.TileSize);
            
            Fruit fruit = new Fruit(13, 26, board);
            PacMan pacman = new PacMan(13, 26, board, fruit);
            Ghost blinky = new Ghost(13, 14, board, 26, 0, pacman, GhostType.Blinky);
            Ghost pinky = new Ghost(13, 17, board, 3, 0, pacman, GhostType.Pinky);
            Ghost inky = new Ghost(11, 17, board, 28, 35, pacman, GhostType.Inky);
            Ghost clyde = new Ghost(15, 17, board, 0, 35, pacman, GhostType.Clyde);
            inky.SetBlinky(blinky);
            List<Ghost> ghosts = [blinky, inky, clyde, pinky];
            LevelTimer timer = new LevelTimer(ghosts, board.LEVEL);
            
            pacman.SetGhosts(ghosts);
            pacman.SetTimer(timer);
            
            TimerManager.Initialize();
            SoundManager.Initialize();
            
            int screenWidth = (int)(board.Grid.GetLength(1) * GameDraw.TileSize * GameDraw.DrawScale);
            int screenHeight = (int)(board.Grid.GetLength(0) * GameDraw.TileSize * GameDraw.DrawScale);

            Raylib.InitWindow(screenWidth, screenHeight, "Pac-Man");
            Image icon = Raylib.LoadImage("assets/icons/Pacman.png");
            Raylib.SetWindowIcon(icon);
            Raylib.UnloadImage(icon);
            Raylib.SetTargetFPS(60);
            GameDraw.InitBoardTexture(board);
            EnableDarkTitleBar();

            SoundManager.Play(SfxType.Start);

            bool ComputeIsFrozen(bool anyGhostEaten) =>
                TimerManager.IsRunning(TimerType.GameStart)
                || TimerManager.IsRunning(TimerType.StartTimer)
                || playDeathAnim || anyGhostEaten || pacman.HasDied
                || !TimerManager.IsPaused(TimerType.LevelStart)
                || !TimerManager.IsPaused(TimerType.GameOver)
                || !transitionState.Equals(LevelTransitionState.None)
                || !TimerManager.IsPaused(TimerType.LevelEnd);

            while (!Raylib.WindowShouldClose())
            {
                if (ResetGame)
                {
                    ResetGame = false;
                    GameReset(pacman);
                }
                bool anyGhostEaten = ghosts.Any(g => g.DeathState.Equals(DiedTransitionState.JustDied));
                if (anyGhostEaten)
                {
                    TimerManager.Resume(TimerType.GhostEaten);
                    if (TimerManager.IsDone(TimerType.GhostEaten))
                    {
                        TimerManager.PauseAndReset(TimerType.GhostEaten);
                        foreach (var g in ghosts)
                            if (g.DeathState.Equals(DiedTransitionState.JustDied))
                                g.DeathState = DiedTransitionState.LateDied;
                        anyGhostEaten = false;   // pause is over as of this frame
                    }

                }
                isFrozen = ComputeIsFrozen(anyGhostEaten);

                MusicType currentMusic = DecideSiren(board, pacman, timer);
                bool inLevelTransition = !transitionState.Equals(LevelTransitionState.None);
                if (inLevelTransition || (isFrozen && !currentMusic.Equals(MusicType.Fright)))
                    SoundManager.StopSiren();
                else
                    SoundManager.PlaySiren(currentMusic);
                SoundManager.UpdateSiren();
                // 2. Everyone uses that same, already-final isFrozen value.
                CanChangeAnimation();
                HandleInput(pacman);
                pacman.UpdateLoop(isFrozen);
               
                if (board.isLevelFinished())
                {
                    PacManAnimFrameIndex = 2;
                    
                    switch (transitionState)
                    {
                        case LevelTransitionState.None:
                            transitionState = LevelTransitionState.LevelEnding;
                            TimerManager.ResumeAndReset(TimerType.LevelEnd);
                            break;

                        case LevelTransitionState.LevelEnding:
                            if (TimerManager.IsDone(TimerType.LevelEnd))
                            {
                                TimerManager.PauseAndReset(TimerType.LevelEnd);
                                TimerManager.ResumeAndReset(TimerType.LevelStart);
                                transitionState = LevelTransitionState.LevelStarting;
                            }
                            break;

                        case LevelTransitionState.LevelStarting:
                            if (TimerManager.IsDone(TimerType.LevelStart))
                            {
                                TimerManager.PauseAndReset(TimerType.LevelStart);
                                pacman.ResetForNextLevel();
                                TimerManager.ResumeAndReset(TimerType.GameStart);
                                transitionState = LevelTransitionState.None;
                            }
                            break;
                    }
                    isFrozen = ComputeIsFrozen(anyGhostEaten);
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
                    // SoundManager.Play(SfxType.Death_0);
                    if (TimerManager.IsDone(TimerType.PacManDeath))
                    {
                        SoundManager.Play(SfxType.Death_0);
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
                        // DO WHATEVER WE NEED TO DO WHEN THE GAME ENDS
                        board.UpdateHighScore();
                        TimerManager.PauseAndReset(TimerType.GameOver);
                        break;
                    }
                }

                timer.UpdateTimer(isFrozen);
                TimerManager.Update();

                // Off-screen board updates must happen outside BeginDrawing/EndDrawing
                GameDraw.UpdateBoardTexture(board);

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                GameDraw.DrawBoard(board, !TimerManager.IsPaused(TimerType.LevelStart));
                GameDraw.DrawFruit(fruit, board.LEVEL);
                if (!playDeathAnim && !TimerManager.IsRunning(TimerType.StartTimer) && TimerManager.IsPaused(TimerType.LevelStart) && TimerManager.IsPaused(TimerType.GameOver))
                    GameDraw.DrawGhosts(ghosts, GhostAnimFrameIndex);
                if (!TimerManager.IsRunning(TimerType.StartTimer) && TimerManager.IsPaused(TimerType.GameOver))
                    GameDraw.DrawPacMan(pacman, PacManAnimFrameIndex, playDeathAnim);
                GameDraw.DrawAllMessages(board, pacman);

                Raylib.EndMode2D();
                Raylib.EndDrawing();
            }
            GameDraw.UnloadBoardTexture();
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

        static MusicType DecideSiren(Board board, PacMan pacMan, LevelTimer timer)
        {
            if (pacMan.IsGhostRunningToHome())
                return MusicType.Eyes;
            else if (timer.IsFrightMode())
                return MusicType.Fright;
            switch (board.RemainingDots)
            {
                case > 104:
                    return MusicType.Siren0;
                case > 76:
                    return MusicType.Siren1;
                case > 34:
                    return MusicType.Siren2;
                case > 10:
                    return MusicType.Siren3;
                case >= 0:
                    return MusicType.Siren4;
                default:
                    return MusicType.Siren0;
            }
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
                IncrementPacManTimers(Sprites.PacmanDirectionList[0].Count, 3);
            else
                IncrementPacManTimers(Sprites.PacManDead.Count, 10);

            IncrementGhostTimer(Sprites.BlinkyDirectionList[0].Count);

            if (TimerManager.IsDone(TimerType.PelletAnim))
                TimerManager.ResetTimer(TimerType.PelletAnim);
        }
        public static void GameReset(PacMan pacMan)
        {
            playDeathAnim = false;
            SoundManager.Play(SfxType.Death_1);
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
                if (PacManAnimFrameIndex + 1 == Sprites.PacManDead.Count)
                    ResetGame = true;
                TimerManager.ResetTimer(TimerType.PacManAnim, FrameBuffer);
            }
        }
        public static void IncrementGhostTimer(int AvailableFrames)
        {
            if (TimerManager.IsDone(TimerType.GhostAnim))
            {
                GhostAnimFrameIndex = (GhostAnimFrameIndex + 1) % AvailableFrames;
                TimerManager.ResetTimer(TimerType.GhostAnim);
            }
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int valueSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        static unsafe void EnableDarkTitleBar()
        {
            if (!OperatingSystem.IsWindows())
                return;
            IntPtr hwnd = (IntPtr)Raylib.GetWindowHandle();
            int enabled = 1;
            DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref enabled, sizeof(int));
        }
    }
}