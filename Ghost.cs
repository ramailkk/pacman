using System.Collections;
using System.Numerics;
using PacManGame;
namespace PacManGame
{
    public enum ModeType
    {
        Chase,
        Scatter,
        Fright
    }
    public enum GhostHouseState
    {
        Normal,
        Home,
        Enter,
        Leave,
        Exit
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
        public GhostHouseState HouseState;
        public bool canReverse;
        public PacMan PacMan;
        private Vector2D pendingDirection = Vector2D.Zero;
        private int pendingTileX, pendingTileY;
        public (int X, int Y) ScatterTarget => modeTargetTiles[ModeType.Scatter];
        public int dotCounter;
        public int dotLimit;
        public GhostType ghostType;
        public Ghost Blinky;
        public bool hasAlignedToDoor;
        public Ghost(int TilePosX, int TilePosY, Board board, int ScatterTileX, int ScatterTileY, PacMan pacMan, GhostType ghostType) : base(TilePosX, TilePosY, board)
        {
            PacMan = pacMan;
            modeTargetTiles = new Dictionary<ModeType, (int X, int Y)>
            {
                { ModeType.Scatter, (ScatterTileX, ScatterTileY) },
                { ModeType.Chase, CalculateTargetTileForEachGhost() },
            };
            dotCounter = 0;
            dotLimit = 0;
            this.ghostType = ghostType;
            direction = Vector2D.Left;
            CurrentMode = ModeType.Scatter;
            hasAlignedToDoor = false;
            Initialize();
        }


        // Special function to move the Ghost in the middle of two tiles
        public void pixelAdjustmentX()
        {
            // if (ghostType.Equals(GhostType.Clyde))
            //     PixelPosX = PixelPosX + board.TileWidth / 2;
            // else if (ghostType.Equals(GhostType.Inky))
            //     PixelPosX = PixelPosX - board.TileWidth / 2;
        }
        public override void Initialize()

        {
            base.Initialize();
            //  Further pixel change depending on where the Ghost is placed
            pixelAdjustmentX();
            if (this.isGhostinHouse())
            {
                direction = ghostType.Equals(GhostType.Pinky) ? Vector2D.Down : Vector2D.Up;
                HouseState = GhostHouseState.Home;
            }
            else
            {
                direction = Vector2D.Left;
                HouseState = GhostHouseState.Normal;
            }
        }

        public void CheckHouseStateChange()
        {
            if (dotCounter > dotLimit)
            {

            }
        }
        public void Move()
        {
            (int TileX, int TileY) = ConvertPixelToTile(PixelPosX, PixelPosY);
            // check Speed first
            CheckSpeedForGhost(TileX, TileY);
            if (!CanMoveThisTick())
                return;
            modeTargetTiles[ModeType.Chase] = CalculateTargetTileForEachGhost();

            // Deal with anything related to Home States First
            if (HouseState.Equals(GhostHouseState.Home))
            {
                direction = GhostLookAhead();
                (PixelPosX, PixelPosY) = (PixelPosX + direction.X, PixelPosY + direction.Y);
                if (dotCounter >= dotLimit && (PixelPosX, PixelPosY) == GetStartCords())
                    HouseState = GhostHouseState.Leave;
                return;
            }
            else if (HouseState.Equals(GhostHouseState.Leave))
{
    if (!hasAlignedToDoor)
    {
        Vector2D ghostDirection = ghostType switch
        {
            GhostType.Inky => Vector2D.Right,
            GhostType.Clyde => Vector2D.Left,
            _ => Vector2D.Zero // Blinky/Pinky already aligned in X
        };

        if (HasAlignedToDoor(ghostDirection, 13, 17))
            hasAlignedToDoor = true; // lock it in — never re-check alignment again this trip
    }
    else
    {
        if (HasExitedDoor(13, 14))
        {
            PixelPosX -= board.TileWidth / 2;
            direction = Vector2D.Left;
            pendingDirection = Vector2D.Zero;
            pendingTileX = pendingTileY = int.MinValue;
            hasAlignedToDoor = false; // reset for next time this ghost re-enters the house
            HouseState = GhostHouseState.Normal;
        }
    }
    return;
}


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
        }
        public bool HasAlignedToDoor(Vector2D dir, int MiddleTileX, int MiddleTileY)
        {
            // GET PINKY'S TILE AND POSIITION AS REFERENCE
            (int RefPixelX, int RefPixelY) = ConvertTileToPixel(MiddleTileX,MiddleTileY);
            if ((PixelPosX, PixelPosY) == (RefPixelX + board.TileWidth/2, RefPixelY))
            {
            direction = Vector2D.Up;
            return true;   
            }
            (PixelPosX,PixelPosY) = (PixelPosX+dir.X,PixelPosY + dir.Y);
            return false; 
        }
        public bool HasExitedDoor(int MiddleTileX, int MiddleTileY)
        {
          // GET BLINKY'S TILE AND POSITION AS REFERENCE HERE
            (int RefPixelX, int RefPixelY) = ConvertTileToPixel(MiddleTileX,MiddleTileY);
            if ((PixelPosX, PixelPosY) == (RefPixelX + board.TileWidth/2, RefPixelY))
            {
                return true;
            }
            (PixelPosX,PixelPosY) = (PixelPosX+Vector2D.Up.X,PixelPosY + Vector2D.Up.Y);
                return false;
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
                if (canReverse)
                {
                    canReverse = false;
                    return direction.Reverse();
                }
                // No going back
                if (dir.Equals(direction.Reverse()))
                    continue;

                // REDZONE CANT GO UP IN THESE TILES
                if (board.Grid[tileY, tileX].IsRedZone() && dir.Equals(Vector2D.Up))
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
        public Vector2D GhostLookAhead()
        {
            if (!ghostType.Equals(GhostType.Blinky))
            {
                if (IsValidMove(direction))
                    return direction;
                else
                    return direction.Reverse();
            }
            return Vector2D.Zero;
        }

        public Vector2D Intersection(int tileX, int tileY, List<Vector2D> viableDirections)
        {
            // 
            int low = int.MaxValue;
            Vector2D viableDirection = Vector2D.Zero;
            (int targetTileX, int targetTileY) = modeTargetTiles[CurrentMode];
            foreach (var dir in viableDirections)
            {
                int dist = Utils.EuclideanDistanceBetweenTiles(tileX + dir.X, tileY + dir.Y, targetTileX, targetTileY);
                if (dist < low)
                {
                    low = dist;
                    viableDirection = dir;
                }
            }
            return viableDirection;
        }

        public void UpdateMode(ModeType mode)
        {
            if (mode.Equals(ModeType.Fright))
            {
                // Apply Fright timer starting logic here
            }
            // Reversal logic
            if (CurrentMode.Equals(ModeType.Chase) && (mode.Equals(ModeType.Scatter) || mode.Equals(ModeType.Fright)))
                canReverse = true;
            if (CurrentMode.Equals(ModeType.Scatter) && mode.Equals(ModeType.Chase))
                canReverse = true;
            CurrentMode = mode;
        }

        public (int TileX, int TileY) CalculateTargetTileForEachGhost()
        {

            if (HouseState.Equals(GhostHouseState.Leave))
            {
                return (13, 14);
            }

            (int PacTileX, int PacTileY) = PacMan.GetPacManTile();
            if (ghostType.Equals(GhostType.Blinky))
            {
                return (PacTileX, PacTileY);
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

            else
            {
                (int myTileX, int myTileY) = ConvertPixelToTile(PixelPosX, PixelPosY);
                if (Utils.EuclideanDistanceBetweenTiles(PacTileX, PacTileY, myTileX, myTileY) > 8)
                    return (PacTileX, PacTileY);
                else
                    return modeTargetTiles[ModeType.Scatter];
            }
        }
        public (int X, int Y) GetTargetForMode(ModeType mode)
        {
            if (mode.Equals(ModeType.Fright))
                return ConvertPixelToTile(PixelPosX, PixelPosY);
            return modeTargetTiles[mode];
        }
        public bool isGhostinHouse()
        {
            (int TileX, int TileY) = ConvertPixelToTile(PixelPosX, PixelPosY);
            return board.Grid[TileY, TileX].IsGhostHouse();
        }

        public void SetBlinky(Ghost blinky)
        {
            Blinky = blinky;
        }
    }
}