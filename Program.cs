using System;
using Raylib_cs;

namespace PacManGame
{
    class Program
    {
        // Internal simulation tile size (matches Actor's pixel-coordinate math).
        // Don't change this without also re-checking Actor's movement/collision math.
        const int TileSize = 8;
        // Purely visual multiplier so an 8px tile isn't a postage stamp on screen.
        const float DrawScale = 3f;
        // Mode timing
        const float ModeDuration = 10.0f; // seconds per mode
        static float modeTimer = ModeDuration;
        static ModeType currentMode = ModeType.Scatter; // Start in Scatter

        static void Main(string[] args)
        {
            int[][] board =
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
                new int[] { 1, 2, 2, 2, 2, 2, 2, 1, 1, 4, 2, 2, 2, 1, 1, 2, 2, 2, 2, 1, 1, 2, 2, 2, 2, 2, 2, 1 },
                new int[] { 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1 },
                new int[] { 6, 6, 6, 6, 6, 1, 2, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 2, 1, 6, 6, 6, 6, 6 },
                new int[] { 6, 6, 6, 6, 6, 1, 2, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 2, 1, 6, 6, 6, 6, 6 },
                new int[] { 6, 6, 6, 6, 6, 1, 2, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 2, 1, 6, 6, 6, 6, 6 },
                new int[] { 1, 1, 1, 1, 1, 1, 2, 1, 1, 0, 1, 5, 5, 5, 5, 5, 5, 1, 0, 1, 1, 2, 1, 1, 1, 1, 1, 1 },
                new int[] { 7, 7, 7, 7, 7, 7, 2, 0, 0, 0, 1, 5, 5, 5, 5, 5, 5, 1, 0, 2, 2, 7, 7, 7, 7, 7, 7, 7 },
                new int[] { 1, 1, 1, 1, 1, 1, 2, 1, 1, 0, 1, 5, 5, 5, 5, 5, 5, 1, 0, 1, 1, 2, 1, 1, 1, 1, 1, 1 },
                new int[] { 6, 6, 6, 6, 6, 1, 2, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 2, 1, 6, 6, 6, 6, 6 },
                new int[] { 6, 6, 6, 6, 6, 1, 2, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 2, 1, 6, 6, 6, 6, 6 },
                new int[] { 6, 6, 6, 6, 6, 1, 2, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 2, 1, 6, 6, 6, 6, 6 },
                new int[] { 1, 1, 1, 1, 1, 1, 2, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 2, 1, 1, 1, 1, 1, 1 },
                new int[] { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1 },
                new int[] { 1, 2, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2, 1 },
                new int[] { 1, 2, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2, 1 },
                new int[] { 1, 3, 2, 2, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 2, 2, 3, 1 },
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
            Board board1 = new(board, TileSize, TileSize);

            // Row 23, col 14 is an open dot tile below the ghost house — safe spawn.
            PacMan pacman = new PacMan(15, 24, 100, board1, 3);

            // Create one ghost with scatter corner (top-left) and fright tile (center)
            Ghost blinky = new Ghost(14, 13, 80, board1, 3, 0, 14, 13, pacman);

            int screenWidth = (int)(board1.Grid.GetLength(1) * TileSize * DrawScale);
            int screenHeight = (int)(board1.Grid.GetLength(0) * TileSize * DrawScale) + 100;

            Raylib.InitWindow(screenWidth, screenHeight, "PacMan - Raylib Test Harness");
            Raylib.SetTargetFPS(60);

            while (!Raylib.WindowShouldClose())
            {
                // Update mode timer
                modeTimer -= Raylib.GetFrameTime();
                if (modeTimer <= 0)
                {
                    currentMode = (currentMode == ModeType.Scatter) ? ModeType.Chase : ModeType.Scatter;
                    blinky.UpdateMode(currentMode);
                    modeTimer = ModeDuration;
                }

                HandleInput(pacman);
                pacman.Move();
                blinky.Move();

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                DrawBoard(board1);
                DrawManhattanPath(blinky);  // Draw Manhattan path visualization
                DrawScatterTarget(blinky);
                DrawPacMan(pacman);
                DrawGhost(blinky);
                DrawHud(board1, pacman, blinky);

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

        static void DrawManhattanPath(Ghost ghost)
        {
            // Get ghost's current tile position
            (int ghostTileX, int ghostTileY) = ghost.ConvertPixelToTile(ghost.PixelPosX, ghost.PixelPosY);

            // Get target tile for current mode
            (int targetX, int targetY) = ghost.GetTargetForMode(ghost.CurrentMode);

            // Get the direction the ghost is currently moving
            Vector2D currentDir = ghost.direction;

            // Draw the Manhattan path - step by step
            int stepX = Math.Sign(targetX - ghostTileX);
            int stepY = Math.Sign(targetY - ghostTileY);

            int currentX = ghostTileX;
            int currentY = ghostTileY;

            // Draw path from ghost to target
            Color pathColor = new Color(255, 255, 0, 100); // Yellow with transparency

            // Draw horizontal path first (Manhattan style)
            if (stepX != 0)
            {
                // Draw horizontal line
                int endX = targetX;
                int startX = ghostTileX;

                float screenY = (currentY * TileSize * DrawScale) + (TileSize * DrawScale / 2);
                float startScreenX = (startX * TileSize * DrawScale) + (TileSize * DrawScale / 2);
                float endScreenX = (endX * TileSize * DrawScale) + (TileSize * DrawScale / 2);

                Raylib.DrawLine(
                    (int)startScreenX, (int)screenY,
                    (int)endScreenX, (int)screenY,
                    pathColor
                );

                // Mark each tile in the horizontal path
                int minX = Math.Min(startX, endX);
                int maxX = Math.Max(startX, endX);
                for (int x = minX; x <= maxX; x++)
                {
                    float tileX = x * TileSize * DrawScale + (TileSize * DrawScale / 2);
                    float tileY = currentY * TileSize * DrawScale + (TileSize * DrawScale / 2);
                    Raylib.DrawCircle((int)tileX, (int)tileY, TileSize * DrawScale * 0.1f, pathColor);
                }
            }

            // Draw vertical path (from ghost tile to target)
            if (stepY != 0)
            {
                // Draw vertical line
                int endY = targetY;
                int startY = ghostTileY;

                float screenX = (targetX * TileSize * DrawScale) + (TileSize * DrawScale / 2);
                float startScreenY = (startY * TileSize * DrawScale) + (TileSize * DrawScale / 2);
                float endScreenY = (endY * TileSize * DrawScale) + (TileSize * DrawScale / 2);

                Raylib.DrawLine(
                    (int)screenX, (int)startScreenY,
                    (int)screenX, (int)endScreenY,
                    pathColor
                );

                // Mark each tile in the vertical path
                int minY = Math.Min(startY, endY);
                int maxY = Math.Max(startY, endY);
                for (int y = minY; y <= maxY; y++)
                {
                    float tileX = targetX * TileSize * DrawScale + (TileSize * DrawScale / 2);
                    float tileY = y * TileSize * DrawScale + (TileSize * DrawScale / 2);
                    Raylib.DrawCircle((int)tileX, (int)tileY, TileSize * DrawScale * 0.1f, pathColor);
                }
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

                    float startX = ghostTileX * TileSize * DrawScale + (TileSize * DrawScale / 2);
                    float startY = ghostTileY * TileSize * DrawScale + (TileSize * DrawScale / 2);
                    float endX = nextX * TileSize * DrawScale + (TileSize * DrawScale / 2);
                    float endY = nextY * TileSize * DrawScale + (TileSize * DrawScale / 2);

                    // Draw viable direction in green
                    Color viableColor = new Color(0, 255, 0, 150);
                    Raylib.DrawLine((int)startX, (int)startY, (int)endX, (int)endY, viableColor);

                    // Draw a small green arrow head
                    float arrowSize = TileSize * DrawScale * 0.15f;
                    float angle = (float)Math.Atan2(dir.Y, dir.X);
                    float arrowX = endX - arrowSize * (float)Math.Cos(angle);
                    float arrowY = endY - arrowSize * (float)Math.Sin(angle);

                    // Draw arrow tip
                    Raylib.DrawCircle((int)endX, (int)endY, TileSize * DrawScale * 0.08f, viableColor);
                }
            }

            // Draw the selected direction (ghost's current direction) in bold red
            if (!ghost.direction.Equals(Vector2D.Zero))
            {
                int nextX = ghostTileX + ghost.direction.X;
                int nextY = ghostTileY + ghost.direction.Y;

                float startX = ghostTileX * TileSize * DrawScale + (TileSize * DrawScale / 2);
                float startY = ghostTileY * TileSize * DrawScale + (TileSize * DrawScale / 2);
                float endX = nextX * TileSize * DrawScale + (TileSize * DrawScale / 2);
                float endY = nextY * TileSize * DrawScale + (TileSize * DrawScale / 2);

                // Draw selected direction in bold red
                Raylib.DrawLine((int)startX, (int)startY, (int)endX, (int)endY, Color.Red);
                Raylib.DrawCircle((int)endX, (int)endY, TileSize * DrawScale * 0.12f, Color.Red);
            }

            // Draw a small label showing distance
            int distance = Ghost.ManhattanDistanceBetweenTiles(ghostTileX, ghostTileY, targetX, targetY);
            float labelX = ghost.PixelPosX * DrawScale + 10;
            float labelY = ghost.PixelPosY * DrawScale - 20;
            string distText = $"Dist: {distance}";
            Raylib.DrawText(distText, (int)labelX, (int)labelY, 15, Color.White);
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
            float radius = TileSize * DrawScale * 0.45f;

            Raylib.DrawCircle((int)screenX, (int)screenY, radius, Color.Yellow);
        }

        static void DrawGhost(Ghost ghost)
        {
            float screenX = ghost.PixelPosX * DrawScale;
            float screenY = ghost.PixelPosY * DrawScale;
            float radius = TileSize * DrawScale * 0.45f;

            Color ghostColor = ghost.CurrentMode switch
            {
                ModeType.Chase => Color.Red,
                ModeType.Scatter => new Color(255, 182, 193, 255),
                ModeType.Fright => Color.Blue,
                _ => Color.Red
            };

            Raylib.DrawCircle((int)screenX, (int)screenY, radius, ghostColor);
        }

        static void DrawHud(Board board, PacMan pacman, Ghost ghost)
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

            string modeText = ghost.CurrentMode.ToString();
            string line3 = $"Ghost Mode: {modeText}  Time remaining: {modeTimer:F1}s";
            Raylib.DrawText(line3, 10, hudY + 50, 20, Color.Orange);

            // Add distance info to HUD
            (int gTileX, int gTileY) = ghost.ConvertPixelToTile(ghost.PixelPosX, ghost.PixelPosY);
            (int targetX, int targetY) = ghost.GetTargetForMode(ghost.CurrentMode);
            int distance = Ghost.ManhattanDistanceBetweenTiles(gTileX, gTileY, targetX, targetY);
            string line4 = $"Ghost Tile: ({gTileX}, {gTileY})  Target: ({targetX}, {targetY})  Dist: {distance}";
            Raylib.DrawText(line4, 10, hudY + 75, 20, Color.LightGray);
        }
    }
}