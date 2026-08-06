using System;
using Raylib_cs;

namespace PacManGame
{
    // Add the GhostType enum at namespace level

    class Program
    {
        // Internal simulation tile size (matches Actor's pixel-coordinate math).
        // Don't change this without also re-checking Actor's movement/collision math.
        const int TileSize = 8;
        // Purely visual multiplier so an 8px tile isn't a postage stamp on screen.
        const float DrawScale = 3f;
        // Mode timing
        static ModeType currentMode = ModeType.Scatter; // Start in Scatter

        // Add a global pause flag
        static bool isPaused = true; // Start paused

        static void Main(string[] args)
        {
            int[][] board = LevelSpecs.board;
            Board board1 = new(board, TileSize, TileSize);
            PacMan pacman = new PacMan(13, 26, board1, 3);
            Ghost blinky = new Ghost(13, 14, board1, 0, 0, pacman, GhostType.Blinky);
            Ghost pinky = new Ghost(13, 17, board1, 26, 0, pacman, GhostType.Pinky);
            Ghost inky = new Ghost(11, 17, board1, 26, 35, pacman, GhostType.Inky);
            Ghost clyde = new Ghost(15, 17, board1, 0, 35, pacman, GhostType.Clyde);
            inky.SetBlinky(blinky);
            List<Ghost> ghosts = [blinky, inky, clyde, pinky];
            LevelTimer timer = new LevelTimer(ghosts);
            pacman.SetGhosts(ghosts);
            pacman.SetTimer(timer);



            int screenWidth = (int)(board1.Grid.GetLength(1) * TileSize * DrawScale);
            int screenHeight = (int)(board1.Grid.GetLength(0) * TileSize * DrawScale) + 100;

            Raylib.InitWindow(screenWidth, screenHeight, "PacMan - Raylib Test Harness");
            Raylib.SetTargetFPS(60);

            while (!Raylib.WindowShouldClose())
            {
                // Handle pause toggle
                if (Raylib.IsKeyPressed(KeyboardKey.P))
                {
                    isPaused = !isPaused;
                }

                // Only update game logic if not paused
                if (!isPaused)
                {
                    HandleInput(pacman);
                    pacman.UpdateLoop();

                    // Update all ghosts
                    foreach (var ghost in ghosts)
                    {
                        ghost.Move();
                    }

                    pacman.CheckGhostCollisions();
                    timer.UpdateTimer();
                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                DrawBoard(board1);

                // Draw visualizations for all ghosts
                foreach (var ghost in ghosts)
                {
                    DrawEuclideanPath(ghost);
                    DrawScatterTarget(ghost);
                }

                DrawPacMan(pacman);

                // Draw all ghosts
                DrawGhosts(ghosts);

                DrawHud(board1, pacman, ghosts, timer);

                // Draw pause overlay if paused
                if (isPaused)
                {
                    DrawPauseOverlay();
                }

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
            for (int row = 0; row < board.Grid.GetLength(0); row++)
            {
                for (int col = 0; col < board.Grid.GetLength(1); col++)
                {
                    Tile tile = board.Grid[row, col];

                    float x = col * TileSize * DrawScale;
                    float y = row * TileSize * DrawScale;
                    float size = TileSize * DrawScale;

                    switch (tile.Type)
                    {
                        case TileType.Wall:
                            Raylib.DrawRectangle((int)x, (int)y, (int)size, (int)size, Color.DarkBlue);
                            break;
                        case TileType.Dot:
                            Raylib.DrawCircle((int)(x + size / 2), (int)(y + size / 2), size * 0.1f, Color.Beige);
                            break;
                        case TileType.PowerPellet:
                            Raylib.DrawCircle((int)(x + size / 2), (int)(y + size / 2), size * 0.25f, Color.Beige);
                            break;
                        case TileType.Fruit:
                            Raylib.DrawCircle((int)(x + size / 2), (int)(y + size / 2), size * 0.3f, Color.Red);
                            break;
                        case TileType.GhostHouse:
                            Raylib.DrawRectangle((int)x, (int)y, (int)size, (int)size, new Color(30, 30, 30, 255));
                            break;
                        case TileType.DeadSpace:
                        case TileType.Empty:
                        default:
                            break;
                    }
                }
            }
        }

        static void DrawPauseOverlay()
        {
            int screenWidth = Raylib.GetScreenWidth();
            int screenHeight = Raylib.GetScreenHeight();

            // Draw semi-transparent overlay
            Color overlayColor = new Color(0, 0, 0, 180);
            Raylib.DrawRectangle(0, 0, screenWidth, screenHeight, overlayColor);

            // Draw pause text
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

        static void DrawEuclideanPath(Ghost ghost)
        {
            // Get ghost's current tile position
            (int ghostTileX, int ghostTileY) = ghost.ConvertPixelToTile(ghost.PixelPosX, ghost.PixelPosY);

            // Get target tile for current mode
            (int targetX, int targetY) = ghost.GetTargetForMode(ghost.CurrentMode);

            // Calculate Euclidean distance
            double distance = Math.Sqrt(Math.Pow(targetX - ghostTileX, 2) + Math.Pow(targetY - ghostTileY, 2));

            // Convert to screen coordinates (center of tiles)
            float startScreenX = ghostTileX * TileSize * DrawScale + (TileSize * DrawScale / 2);
            float startScreenY = ghostTileY * TileSize * DrawScale + (TileSize * DrawScale / 2);
            float endScreenX = targetX * TileSize * DrawScale + (TileSize * DrawScale / 2);
            float endScreenY = targetY * TileSize * DrawScale + (TileSize * DrawScale / 2);

            // Draw the straight Euclidean path in cyan
            Color pathColor = new Color(0, 255, 255, 150); // Cyan with transparency
            Raylib.DrawLine(
                (int)startScreenX, (int)startScreenY,
                (int)endScreenX, (int)endScreenY,
                pathColor
            );

            // Draw small dots along the path
            int numPoints = 10;
            for (int i = 0; i <= numPoints; i++)
            {
                float t = i / (float)numPoints;
                float x = startScreenX + (endScreenX - startScreenX) * t;
                float y = startScreenY + (endScreenY - startScreenY) * t;
                Raylib.DrawCircle((int)x, (int)y, TileSize * DrawScale * 0.06f, pathColor);
            }

            // Draw the viable directions from the ghost's current position
            var directions = new Vector2D[] { Vector2D.Up, Vector2D.Left, Vector2D.Down, Vector2D.Right };
            foreach (var dir in directions)
            {
                // Check if this direction is viable
                if (ghost.IsValidTile(ghostTileX, ghostTileY, dir))
                {
                    int nextX = ghostTileX + dir.X;
                    int nextY = ghostTileY + dir.Y;

                    float dirStartX = ghostTileX * TileSize * DrawScale + (TileSize * DrawScale / 2);
                    float dirStartY = ghostTileY * TileSize * DrawScale + (TileSize * DrawScale / 2);
                    float dirEndX = nextX * TileSize * DrawScale + (TileSize * DrawScale / 2);
                    float dirEndY = nextY * TileSize * DrawScale + (TileSize * DrawScale / 2);

                    // Draw viable direction in green
                    Color viableColor = new Color(0, 255, 0, 150);
                    Raylib.DrawLine((int)dirStartX, (int)dirStartY, (int)dirEndX, (int)dirEndY, viableColor);

                    // Draw a small green arrow head
                    float arrowSize = TileSize * DrawScale * 0.15f;
                    float angle = (float)Math.Atan2(dir.Y, dir.X);
                    float arrowX = dirEndX - arrowSize * (float)Math.Cos(angle);
                    float arrowY = dirEndY - arrowSize * (float)Math.Sin(angle);

                    // Draw arrow tip
                    Raylib.DrawCircle((int)dirEndX, (int)dirEndY, TileSize * DrawScale * 0.08f, viableColor);
                }
            }

            // Draw the selected direction (ghost's current direction) in bold red
            if (!ghost.direction.Equals(Vector2D.Zero))
            {
                int nextX = ghostTileX + ghost.direction.X;
                int nextY = ghostTileY + ghost.direction.Y;

                float dirStartX = ghostTileX * TileSize * DrawScale + (TileSize * DrawScale / 2);
                float dirStartY = ghostTileY * TileSize * DrawScale + (TileSize * DrawScale / 2);
                float dirEndX = nextX * TileSize * DrawScale + (TileSize * DrawScale / 2);
                float dirEndY = nextY * TileSize * DrawScale + (TileSize * DrawScale / 2);

                // Draw selected direction in bold red
                Raylib.DrawLine((int)dirStartX, (int)dirStartY, (int)dirEndX, (int)dirEndY, Color.Red);
                Raylib.DrawCircle((int)dirEndX, (int)dirEndY, TileSize * DrawScale * 0.12f, Color.Red);
            }

            // Draw a small label showing Euclidean distance
            float labelX = ghost.PixelPosX * DrawScale + 10;
            float labelY = ghost.PixelPosY * DrawScale - 20;
            string distText = $"Euclidean Dist: {distance:F1}";
            Raylib.DrawText(distText, (int)labelX, (int)labelY, 15, Color.DarkGray);
        }

        static void DrawScatterTarget(Ghost ghost)
        {
            (int scatterX, int scatterY) = ghost.ScatterTarget;

            float screenX = scatterX * TileSize * DrawScale + (TileSize * DrawScale / 2);
            float screenY = scatterY * TileSize * DrawScale + (TileSize * DrawScale / 2);
            float radius = TileSize * DrawScale * 0.3f;

            // Draw a semi-transparent circle
            Color markerColor = new Color(255, 182, 193, 128);
            Raylib.DrawCircle((int)screenX, (int)screenY, radius, markerColor);

            // Draw a small "X" marker
            float crossSize = TileSize * DrawScale * 0.2f;
            Raylib.DrawLine(
                (int)(screenX - crossSize), (int)(screenY - crossSize),
                (int)(screenX + crossSize), (int)(screenY + crossSize),
                new Color(255, 182, 193, 200)
            );
            Raylib.DrawLine(
                (int)(screenX - crossSize), (int)(screenY + crossSize),
                (int)(screenX + crossSize), (int)(screenY - crossSize),
                new Color(255, 182, 193, 200)
            );
        }

        static void DrawPacMan(PacMan pacman)
        {
            float screenX = pacman.PixelPosX * DrawScale;
            float screenY = pacman.PixelPosY * DrawScale;
            float baseRadius = TileSize * DrawScale * 0.45f;
            float overlapRadius = baseRadius * 1.6f; // Overlap neighboring tiles

            // Draw glow/aura effect
            Color glowColor = new Color(255, 255, 0, 60);
            Raylib.DrawCircle((int)screenX, (int)screenY, overlapRadius * 1.2f, glowColor);

            // Draw main body
            Raylib.DrawCircle((int)screenX, (int)screenY, overlapRadius, Color.Yellow);

            // Draw Pac-Man's mouth (pie slice)
            float mouthAngle = 0.3f; // Radians
            float startAngle = -mouthAngle;
            float endAngle = mouthAngle;

            // Mouth based on direction
            if (pacman.direction.Equals(Vector2D.Right))
            {
                startAngle = -mouthAngle;
                endAngle = mouthAngle;
            }
            else if (pacman.direction.Equals(Vector2D.Left))
            {
                startAngle = (float)Math.PI - mouthAngle;
                endAngle = (float)Math.PI + mouthAngle;
            }
            else if (pacman.direction.Equals(Vector2D.Up))
            {
                startAngle = -(float)Math.PI / 2 - mouthAngle;
                endAngle = -(float)Math.PI / 2 + mouthAngle;
            }
            else if (pacman.direction.Equals(Vector2D.Down))
            {
                startAngle = (float)Math.PI / 2 - mouthAngle;
                endAngle = (float)Math.PI / 2 + mouthAngle;
            }

            // // Draw mouth (black triangle/pie slice)
            // Raylib.Cricle(
            //     (int)screenX, (int)screenY,
            //     overlapRadius,
            //     startAngle * 180 / (float)Math.PI,
            //     endAngle * 180 / (float)Math.PI,
            //     10,
            //     Color.Black
            // );

            // Draw eye
            float eyeX = screenX + overlapRadius * 0.3f;
            float eyeY = screenY - overlapRadius * 0.2f;
            float eyeRadius = overlapRadius * 0.2f;
            Raylib.DrawCircle((int)eyeX, (int)eyeY, eyeRadius, Color.Black);
        }

        static void DrawGhosts(List<Ghost> ghosts)
        {
            foreach (var ghost in ghosts)
            {
                float screenX = ghost.PixelPosX * DrawScale;
                float screenY = ghost.PixelPosY * DrawScale;
                float baseRadius = TileSize * DrawScale * 0.45f;
                float overlapRadius = baseRadius * 1.6f;

                // Assign different colors based on ghost type
                Color ghostColor;
                if (ghost.CurrentMode == ModeType.Fright)
                {
                    ghostColor = Color.Blue;
                }
                else
                {
                    ghostColor = ghost.ghostType switch
                    {
                        GhostType.Blinky => Color.Red,
                        GhostType.Pinky => new Color(255, 182, 193, 255),
                        GhostType.Inky => Color.SkyBlue,
                        GhostType.Clyde => new Color(255, 165, 0, 255),
                        _ => Color.Red
                    };
                }

                // Draw glow effect
                Color glowColor = Color.Gold;
                Raylib.DrawCircle((int)screenX, (int)screenY, overlapRadius * 1.3f, glowColor);

                // Ghost body - circular
                Raylib.DrawCircle((int)screenX, (int)screenY, overlapRadius, ghostColor);

                // Ghost bottom wavy edge (floating effect)
                float waveOffset = overlapRadius * 0.15f;
                for (int i = -3; i <= 3; i++)
                {
                    float x = screenX + i * (overlapRadius * 0.4f);
                    float y = screenY + overlapRadius - Math.Abs(i) * waveOffset * 0.5f;
                    float r = overlapRadius * 0.2f;
                    Raylib.DrawCircle((int)x, (int)y, r, ghostColor);
                }

                // Eyes (larger for overlap version)
                float eyeOffset = overlapRadius * 0.35f;
                float eyeRadius = overlapRadius * 0.3f;

                // White eyes
                Raylib.DrawCircle((int)(screenX - eyeOffset), (int)(screenY - eyeOffset * 0.3f), eyeRadius, Color.White);
                Raylib.DrawCircle((int)(screenX + eyeOffset), (int)(screenY - eyeOffset * 0.3f), eyeRadius, Color.White);

                // Pupils
                float pupilRadius = eyeRadius * 0.5f;
                float pupilOffset = eyeRadius * 0.3f;

                if (ghost.direction.Equals(Vector2D.Left))
                {
                    Raylib.DrawCircle((int)(screenX - eyeOffset - pupilOffset), (int)(screenY - eyeOffset * 0.3f), pupilRadius, Color.Black);
                    Raylib.DrawCircle((int)(screenX + eyeOffset - pupilOffset), (int)(screenY - eyeOffset * 0.3f), pupilRadius, Color.Black);
                }
                else if (ghost.direction.Equals(Vector2D.Right))
                {
                    Raylib.DrawCircle((int)(screenX - eyeOffset + pupilOffset), (int)(screenY - eyeOffset * 0.3f), pupilRadius, Color.Black);
                    Raylib.DrawCircle((int)(screenX + eyeOffset + pupilOffset), (int)(screenY - eyeOffset * 0.3f), pupilRadius, Color.Black);
                }
                else if (ghost.direction.Equals(Vector2D.Up))
                {
                    Raylib.DrawCircle((int)(screenX - eyeOffset), (int)(screenY - eyeOffset * 0.3f - pupilOffset), pupilRadius, Color.Black);
                    Raylib.DrawCircle((int)(screenX + eyeOffset), (int)(screenY - eyeOffset * 0.3f - pupilOffset), pupilRadius, Color.Black);
                }
                else
                {
                    Raylib.DrawCircle((int)(screenX - eyeOffset), (int)(screenY - eyeOffset * 0.3f + pupilOffset), pupilRadius, Color.Black);
                    Raylib.DrawCircle((int)(screenX + eyeOffset), (int)(screenY - eyeOffset * 0.3f + pupilOffset), pupilRadius, Color.Black);
                }
            }
        }

        static void DrawHud(Board board, PacMan pacman, List<Ghost> ghosts, LevelTimer timer)
        {
            int hudY = board.Grid.GetLength(0) * TileSize * (int)DrawScale + 5;

            string line1 = $"Score: {board.Score}   Lives: {pacman.LIVES}   Dots left: {board.DotCounter}";
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
            string line3 = $"Ghost Mode: {modeText}  Time remaining: {timer.ModeTimer:F1}s";
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
    }
}