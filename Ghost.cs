using System.Collections;
using PacManGame;
namespace PacManGame
{
    public enum ModeType
    {
        Chase,
        Scatter,
        Fright,
        Dead,
        Home
    }
     public enum GhostType
    {
        Blinky,
        Pinky,
        Inky,
        Clyde
    }
    // up, left, down, right preference for tiles if all are same distance away from targetTile. 
    public class Ghost : Actor
    {
        private Dictionary<ModeType, (int X, int Y)> modeTargetTiles;
        public ModeType CurrentMode;
        public bool canReverse;
        public PacMan PacMan;
        private Vector2D pendingDirection = Vector2D.Zero;
        private int pendingTileX, pendingTileY;
        public (int X, int Y) ScatterTarget => modeTargetTiles[ModeType.Scatter];

        public int dotCounter;
        public int dotLimit;
        public GhostType ghostType;
        public Ghost Blinky;

        public Ghost(int TilePosX, int TilePosY, int speed, Board board, int ScatterTileX, int ScatterTileY, int FrightTileX, int FrightTileY, PacMan pacMan, GhostType ghostType) : base(TilePosX, TilePosY, speed, board)
        {
            PacMan = pacMan;
            modeTargetTiles = new Dictionary<ModeType, (int X, int Y)>
            {
                { ModeType.Scatter, (ScatterTileX, ScatterTileY) },
                { ModeType.Chase, CalculateTargetTileForEachGhost() },
                { ModeType.Dead, (FrightTileX, FrightTileY) },
            };

            direction = Vector2D.Left;
            CurrentMode = ModeType.Scatter;
            this.ghostType = ghostType;
        }

        public bool isGhostinHouse()
        {
            (int TileX, int TileY) = ConvertPixelToTile(PixelPosX,PixelPosY);
            return board.Grid[TileY,TileX].IsGhostHouse();
        }
        public void GhostLookAhead()
        {
            
        }
        public void Move()
        {
             (int TileX, int TileY) = ConvertPixelToTile(PixelPosX, PixelPosY);
            // check Speed first
            CheckSpeedForGhost(TileX,TileY);
            if (!CanMoveThisTick())
                return;
            modeTargetTiles[ModeType.Chase] = CalculateTargetTileForEachGhost();

            if ((PixelPosX, PixelPosY) == ConvertTileToPixel(TileX, TileY))
            {
                // Arrived at the tile a previous decision was made for — commit it now
                if (TileX == pendingTileX && TileY == pendingTileY && !pendingDirection.Equals(Vector2D.Zero))
                    direction = pendingDirection;

                // Pre-decide the turn for the *next* tile, to be committed when we get there
                int nextTileX = TileX + direction.X;
                int nextTileY = TileY + direction.Y;
                (nextTileX, nextTileY) = CheckForTunnelTile(nextTileX, nextTileY);

                if (CurrentMode.Equals(ModeType.Home))
                {
                    
                }

                else if (!CurrentMode.Equals(ModeType.Fright))
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

        public void CheckSpeedForGhost(int tileX, int tileY)
        {
            int Level = board.LEVEL;
            Tile currentTile = board.Grid[tileY, tileX];
            if (currentTile.IsTunnel())
                speed = LevelSpecs.GetEntry(Level, LevelSpecs.GhostTunnelSpeed);
            else if (CurrentMode.Equals(ModeType.Fright))
                speed = LevelSpecs.GetEntry(Level, LevelSpecs.FrightGhostSpeed);
            else if (CurrentMode.Equals(ModeType.Chase) || CurrentMode.Equals(ModeType.Scatter))
                speed = LevelSpecs.GetEntry(Level, LevelSpecs.GhostSpeed);
            else if (CurrentMode.Equals(ModeType.Dead))
                speed = 90;
        }
        public Vector2D FrightLookAhead(int tileX, int tileY)
        {
            // if not found at random first then go clockwise
            var directions = new Vector2D[] { Vector2D.Up, Vector2D.Right, Vector2D.Down, Vector2D.Left };
            var randomDirection = directions[Random.Shared.Next(directions.Length)];
            if (!IsValidTile(tileX, tileY, randomDirection) || randomDirection.Equals(direction.Reverse()))
            {
                foreach (var dir in directions)
                {
                    if (canReverse)
                    {
                        canReverse = false;
                        return direction.Reverse();
                    }
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
                if (canReverse){
                        canReverse = false;
                        return direction.Reverse();
                    }
                // No going back
                if (dir.Equals(direction.Reverse()))
                    continue;

                // REDZONE CANT GO UP IN THESE TILES
                if (board.Grid[tileY,tileX].IsRedZone() && dir.Equals(Vector2D.Up))
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
            // Reversal logic
            if (CurrentMode.Equals(ModeType.Chase) && (mode.Equals(ModeType.Scatter) || mode.Equals(ModeType.Fright)))
                canReverse = true;
            if (CurrentMode.Equals(ModeType.Scatter) && mode.Equals(ModeType.Chase))
                canReverse = true;
            CurrentMode = mode;
        }

        public static int ManhattanDistanceBetweenTiles(int TileX, int TileY, int targetTileX, int targetTileY)
        {
            return (int)(Math.Abs(TileX - targetTileX) + Math.Abs(TileY - targetTileY));
        }
        public static int SquaredEuclideanDistanceBetweenTiles(int TileX, int TileY, int targetTileX, int targetTileY)
            {
                int dx = TileX - targetTileX;
                int dy = TileY - targetTileY;
                return dx * dx + dy * dy;
            }
        public static int EuclideanDistanceBetweenTiles(int TileX, int TileY, int targetTileX, int targetTileY)
            {
                int dx = TileX - targetTileX;
                int dy = TileY - targetTileY;
                return (int)Math.Sqrt(dx * dx + dy * dy);
            }

        
        public (int TileX,int TileY) CalculateTargetTileForEachGhost()
        {
            (int PacTileX, int PacTileY) = PacMan.GetPacManTile();
            
            if (ghostType.Equals(GhostType.Blinky)){
             return (PacTileX,PacTileY);
            }
            else if (ghostType.Equals(GhostType.Pinky))
            {
                return (PacTileX + PacMan.direction.X * 4,
                        PacTileY + PacMan.direction.Y * 4);
            }
            else if (ghostType.Equals(GhostType.Inky))
{
                PacTileX = PacTileX + PacMan.direction.X * 2;
                PacTileY = PacTileY + PacMan.direction.Y * 2;

                (int BlinkyTileX, int BlinkyTileY) = ConvertPixelToTile(Blinky.PixelPosX, Blinky.PixelPosY);

                int TargetTileX = PacTileX + (PacTileX - BlinkyTileX);
                int TargetTileY = PacTileY + (PacTileY - BlinkyTileY);
                
                return (TargetTileX, TargetTileY);
            }
            //  for Clyde
            else
            {
                (int myTileX, int myTileY) = ConvertPixelToTile(PixelPosX, PixelPosY);
                if (EuclideanDistanceBetweenTiles(PacTileX,PacTileY, myTileX, myTileY) > 8)
                    return (PacTileX,PacTileY);
                else
                    return modeTargetTiles[ModeType.Scatter];
            }
        }
        public (int X, int Y) GetTargetForMode(ModeType mode)
        {
            if (mode.Equals(ModeType.Fright))
                return ConvertPixelToTile(PixelPosX,PixelPosY);
            return modeTargetTiles[mode];
        }

        public void SetBlinky(Ghost blinky)
        {
            Blinky = blinky;
        }
    }
}