using System.Collections;

namespace PacManGame
{
    public enum ModeType
    {
        Chase,
        Scatter,
        Fright,
        Dead
    }
    // up, left, down, right preference for tiles if all are same distance away from targetTile. 
    public class Ghost : Actor
    {
        private Dictionary<ModeType, (int X, int Y)> modeTargetTiles;
        public ModeType CurrentMode;
        public PacMan PacMan;
        private Vector2D pendingDirection = Vector2D.Zero;
        private int pendingTileX, pendingTileY;
        public (int X, int Y) ScatterTarget => modeTargetTiles[ModeType.Scatter];

        public Ghost(int TilePosX, int TilePosY, int speed, Board board, int ScatterTileX, int ScatterTileY, int FrightTileX, int FrightTileY, PacMan pacMan) : base(TilePosX, TilePosY, speed, board)
        {
            PacMan = pacMan;
            modeTargetTiles = new Dictionary<ModeType, (int X, int Y)>
            {
                { ModeType.Scatter, (ScatterTileX, ScatterTileY) },
                { ModeType.Chase, pacMan.GetPacManTile() },
                { ModeType.Fright, (FrightTileX, FrightTileY) },
                { ModeType.Dead, (FrightTileX, FrightTileY) }
            };

            direction = Vector2D.Left;
            CurrentMode = ModeType.Chase;
        }

        public void Move()
        {
            if (!this.CanMoveThisTick())
                return;
            modeTargetTiles[ModeType.Chase] = PacMan.GetPacManTile();
            (int TileX, int TileY) = ConvertPixelToTile(PixelPosX, PixelPosY);

            if ((PixelPosX, PixelPosY) == ConvertTileToPixel(TileX, TileY))
            {
                // Arrived at the tile a previous decision was made for — commit it now
                if (TileX == pendingTileX && TileY == pendingTileY && !pendingDirection.Equals(Vector2D.Zero))
                    direction = pendingDirection;

                // Pre-decide the turn for the *next* tile, to be committed when we get there
                int nextTileX = TileX + direction.X;
                int nextTileY = TileY + direction.Y;
                (nextTileX, nextTileY) = CheckForTunnelTile(nextTileX, nextTileY);
                if (!CurrentMode.Equals(ModeType.Fright))
                    pendingDirection = NormalLookAhead(nextTileX, nextTileY);
                else
                    pendingDirection = FrightLookAhead(nextTileX, nextTileY);
                pendingTileX = nextTileX;
                pendingTileY = nextTileY;
            }
            // Safety net: never step with an unresolved/dead-end direction
            if (!direction.Equals(Vector2D.Zero))
                (PixelPosX, PixelPosY) = CheckForTunnel(PixelPosX + direction.X, PixelPosY + direction.Y);
        }

        public Vector2D FrightLookAhead(int tileX, int tileY)
        {
            // if not found at random first then go clockwise
            var directions = new Vector2D[] { Vector2D.Up, Vector2D.Right, Vector2D.Down, Vector2D.Left};
            var randomDirection = directions[Random.Shared.Next(directions.Length)];
            if (!IsValidTile(tileX, tileY, randomDirection) || randomDirection.Equals(direction.Reverse()))
            {
                foreach (var dir in directions)
                {
                    if (dir.Equals(direction.Reverse()))
                        continue;
                    if (IsValidTile(tileX, tileY, dir))
                        return dir;
                }
            }
            return randomDirection;
        }
        public Vector2D NormalLookAhead(int tileX, int tileY)
        {
            // up, left, down, right -> is the order we need in intersectio
            var directions = new Vector2D[] { Vector2D.Up, Vector2D.Left, Vector2D.Down, Vector2D.Right };
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
            if (viableDirections.Count == 0)
                return Vector2D.Zero;
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
                int dist = EuclideanDistanceBetweenTiles(tileX + dir.X, tileY + dir.Y, targetTileX, targetTileY);
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
            (newTileX, newTileY) = CheckForTunnelTile(newTileX, newTileY);
            Tile targetTile = board.Grid[newTileY, newTileX];
            return IsTileWalkable(targetTile);
        }

        public void UpdateMode(ModeType mode)
        {
            if (mode.Equals(ModeType.Fright))
            {
                // Apply Fright timer starting logic here
            }
            else if (mode.Equals(ModeType.Dead))
            {
                // Apply being dead logic here
            }
            CurrentMode = mode;
        }

        public static int ManhattanDistanceBetweenTiles(int TileX, int TileY, int targetTileX, int targetTileY)
        {
            return (int)(Math.Abs(TileX - targetTileX) + Math.Abs(TileY - targetTileY));
        }
        public static int EuclideanDistanceBetweenTiles(int TileX, int TileY, int targetTileX, int targetTileY)
        {
            int dx = TileX - targetTileX;
            int dy = TileY - targetTileY;
            return (int)Math.Sqrt(dx * dx + dy * dy);
        }

        public (int X, int Y) GetTargetForMode(ModeType mode)
        {
            return modeTargetTiles[mode];
        }
    }
}