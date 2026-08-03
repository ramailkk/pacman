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
                new int[] { 6, 6, 6, 6, 6, 1, 2, 1, 1, 0, 1, 1, 1, 5, 5, 1, 1, 1, 0, 1, 1, 2, 1, 6, 6, 6, 6, 6 },
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
            PacMan pacman = new PacMan(15, 24, 2, board1, 3);

            int screenWidth = (int)(board1.Grid.GetLength(1) * TileSize * DrawScale);
            int screenHeight = (int)(board1.Grid.GetLength(0) * TileSize * DrawScale) + 70; // room for two HUD lines

            Raylib.InitWindow(screenWidth, screenHeight, "PacMan - Raylib Test Harness");
            Raylib.SetTargetFPS(60);

            while (!Raylib.WindowShouldClose())
            {
                HandleInput(pacman);
                pacman.Move();
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                DrawBoard(board1);
                DrawPacMan(pacman);
                DrawHud(board1, pacman);

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
                            break; // nothing to draw
                    }
                }
            }
        }

        static void DrawPacMan(PacMan pacman)
        {
            float screenX = pacman.PixelPosX * DrawScale;
            float screenY = pacman.PixelPosY * DrawScale;
            float radius = TileSize * DrawScale * 0.45f;

            Raylib.DrawCircle((int)screenX, (int)screenY, radius, Color.Yellow);
        }

        static void DrawHud(Board board, PacMan pacman)
        {
            int hudY = board.Grid.GetLength(0) * TileSize * (int)DrawScale + 5;

            // Line 1: Score, Lives, Dots left
            string line1 = $"Score: {board.Score}   Lives: {pacman.LIVES}   Dots left: {board.DotCounter}";
            Raylib.DrawText(line1, 10, hudY, 20, Color.White);

            // Line 2: Current tile coordinates
            (int tileX, int tileY) = pacman.ConvertPixelToTile(pacman.PixelPosX, pacman.PixelPosY);
            // string line2 = $"Tile: ({tileX}, {tileY})";
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

        }
    }
}
