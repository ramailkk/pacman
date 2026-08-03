using System.Collections;

namespace PacManGame
{
    public enum ModeType
    {
        Chase,
        Scatter,
        Fright
    }
    // up, left, down, right preference for tiles if all are same distance away from targetTile. 
    public class Ghost : Actor
    {
        private Dictionary<ModeType, (int X, int Y)> modeTargetTiles;
        public ModeType CurrentMode;
        public Ghost(int TilePosX, int TilePosY, int speed, Board board, int ScatterTileX, int ScatterTileY, int FrightTileX, int FrightTileY) : base(TilePosX, TilePosY, speed, board)
        {
            modeTargetTiles = new Dictionary<ModeType, (int X, int Y)>
            {
                { ModeType.Scatter, (ScatterTileX, ScatterTileY) },
                { ModeType.Chase, (0, 0) },
                { ModeType.Fright, (FrightTileX, FrightTileY) },
            };
            direction = Vector2D.Left;
            CurrentMode = ModeType.Chase;
        }

        public void Move()
        {
            (int TileX, int TileY) = ConvertPixelToTile(PixelPosX, PixelPosY);

            // if ghosts are centralized to one tile only then make a decision about the next Tile and direction
            if ((PixelPosX, PixelPosY) == ConvertTileToPixel(TileX, TileY))
                this.ChangeDirection(LookAhead(TileX + direction.X, TileY + direction.Y));

            // now keep moving in whatever direction you have
            (PixelPosX, PixelPosY) = (PixelPosX + direction.X, PixelPosY + direction.Y);
        }

        public Vector2D LookAhead(int tileX, int tileY)
        {
            // up, left, down, right -> is the order we need in intersectio
            var directions = new Vector2D[] {Vector2D.Up, Vector2D.Left, Vector2D.Down, Vector2D.Right};
            var viableDirections = new List<Vector2D>(4);

            foreach (var dir in directions)
            {
                // No going back
                if (dir.Equals(direction.Reverse()))
                    continue;

                if (IsValidTile(tileX, tileY, dir))
                    viableDirections.Add(dir);
            }
            // deal with intersection of mmultiple and return 1 direction
            if (viableDirections.Count > 1)
                return Intersection(tileX, tileY, viableDirections);
            // only possible direction so return just 1
            else
                return viableDirections[0];
        }

        public Vector2D Intersection(int tileX, int tileY, List<Vector2D> viableDirections)
        {
            // 
            int low = int.MaxValue;
            Vector2D viableDirection = Vector2D.Zero;
            (int targetTileX, int targetTileY) = modeTargetTiles[CurrentMode];
            foreach (var dir in viableDirections)
            {
                int dist = ManhattanDistanceBetweenTiles(tileX + dir.X, tileY + dir.Y, targetTileX, targetTileY);
                if (dist < low)
                {
                    low = dist;
                    viableDirection = dir;
                }
            }
            return viableDirection;
        }
        public bool IsValidTile(int tileX, int tileY, Vector2D currentDirection)
        {
            (int newTileX, int newTileY) = (tileX + currentDirection.X, tileY + currentDirection.Y);
            Tile targetTile = board.Grid[newTileY, newTileX];
            return IsTileWalkable(targetTile);
        }

        public static int ManhattanDistanceBetweenTiles(int TileX, int TileY, int targetTileX, int targetTileY)
        {
            return (int)(Math.Abs(TileX - targetTileX) + Math.Abs(TileY - targetTileY));
        }
    }
}