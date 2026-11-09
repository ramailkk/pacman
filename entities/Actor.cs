namespace PacManGame
{
    public struct Vector2D(int x, int y)
    {
        public int X = x;
        public int Y = y;
        public static readonly Vector2D Up = new Vector2D(0, -1);
        public static readonly Vector2D Down = new Vector2D(0, 1);
        public static readonly Vector2D Left = new Vector2D(-1, 0);
        public static readonly Vector2D Right = new Vector2D(1, 0);
        public static readonly Vector2D Zero = new Vector2D(0, 0);
        public readonly Vector2D Reverse() => new Vector2D(-X, -Y);
    }
    public class Actor
    {
        public float PixelPosX;
        public float PixelPosY;
        public int speed;
        public Vector2D direction;
        protected Board board;
        public int accumulator;

        private readonly int TilePosX;
        private readonly int TilePosY;
        public Actor(int tilePosX, int tilePosY, Board board)
        {
            this.board = board;
            (TilePosX, TilePosY) = (tilePosX, tilePosY);
            accumulator = 0;
        }

        public virtual void Initialize()
        {
            (PixelPosX, PixelPosY) = ConvertTileToPixel(TilePosX, TilePosY);
            PixelPosX += board.TileWidth / 2f;
            RecordPreviousTile(); // no stale "previous tile" from before a reset
        }

        public int GetStepsThisTick()
        {
            // speed is a percentage: /79 means speed 100 ≈ 1.27 game px per frame
            // ≈ 76 px/s at 60 FPS (arcade-like). FPS itself is still 60.
            accumulator += speed;
            int steps = accumulator / 79;
            accumulator -= steps * 79;
            return steps;
        }

        // Tile this actor was in at the start of the current frame, before anyone moved.
        // Used to catch actors that swap tiles head-on within a single frame.
        public int PrevTileX;
        public int PrevTileY;

        // Call once per frame for every actor BEFORE any of them move.
        public void RecordPreviousTile()
        {
            (PrevTileX, PrevTileY) = ConvertPixelToTile(PixelPosX, PixelPosY);
        }

        public bool IsCollisionWithActor(Actor other)
        {
            (int myTileX, int myTileY) = ConvertPixelToTile(PixelPosX, PixelPosY);
            (int otherTileX, int otherTileY) = ConvertPixelToTile(other.PixelPosX, other.PixelPosY);

            // Same tile right now
            if (myTileX == otherTileX && myTileY == otherTileY)
                return true;

            // Pass-through: we moved into the tile they just left AND they moved into the tile we
            // just left, i.e. we crossed the same tile boundary in opposite directions this frame.
            bool iTookTheirTile = myTileX == other.PrevTileX && myTileY == other.PrevTileY;
            bool theyTookMyTile = otherTileX == PrevTileX && otherTileY == PrevTileY;
            return iTookTheirTile && theyTookMyTile;
        }
        public (int tileX, int tileY) ConvertPixelToTile(float pixelX, float pixelY)
        {
            return ((int)(pixelX / board.TileWidth), (int)(pixelY / board.TileHeight));
        }
        public (float pixelX, float pixelY) ConvertTileToPixel(int tileX, int tileY)
        {
            return ((tileX * board.TileWidth) + (board.TileWidth / 2f), 
                    (tileY * board.TileHeight) + (board.TileHeight / 2f));
        }

        public (float pixelX, float pixelY) CheckForTunnel(float newPixelX, float newPixelY)
        {
            float totalPixelX = board.TileWidth * board.Grid.GetLength(1);
            float totalPixelY = board.TileHeight * board.Grid.GetLength(0);

            float wrappedX = ((newPixelX % totalPixelX) + totalPixelX) % totalPixelX;
            float wrappedY = ((newPixelY % totalPixelY) + totalPixelY) % totalPixelY;

            return (wrappedX, wrappedY);
        }
        public (int tileX, int tileY) CheckForTunnelTile(int newTileX, int newTileY)
        {
            int totalTileX = board.Grid.GetLength(1);
            int totalTileY = board.Grid.GetLength(0);

            int wrappedX = ((newTileX % totalTileX) + totalTileX) % totalTileX;
            int wrappedY = ((newTileY % totalTileY) + totalTileY) % totalTileY;

            return (wrappedX, wrappedY);
        }
        public bool IsValidMove(Vector2D currentDirection)
        {
            if (currentDirection.Equals(Vector2D.Zero))
                return false;

            float newPixelX = PixelPosX + currentDirection.X;
            float newPixelY = PixelPosY + currentDirection.Y;
            (newPixelX, newPixelY) = CheckForTunnel(newPixelX, newPixelY);
            (int tileX, int tileY) = ConvertPixelToTile(newPixelX, newPixelY);
            Tile targetTile = board.Grid[tileY, tileX];
            if (!IsTileWalkable(targetTile))
                return false;
            
            (int StartingTileX, int StartingTileY) = ConvertPixelToTile(PixelPosX, PixelPosY);
            (float CenterX, float CenterY) = ConvertTileToPixel(StartingTileX,StartingTileY);
                if (!IsValidTile(StartingTileX,StartingTileY, currentDirection)){
                    // check if its a turn
                    if (!currentDirection.Equals(direction.Reverse()) && !currentDirection.Equals(direction))
                        return false;
                    if ((PixelPosX, PixelPosY) == (CenterX,CenterY))
                        return false;
                }

            return true;
        }
        public bool IsValidTile(int tileX, int tileY, Vector2D currentDirection)
        {
            (int newTileX, int newTileY) = (tileX + currentDirection.X, tileY + currentDirection.Y);
            (newTileX, newTileY) = CheckForTunnelTile(newTileX, newTileY);
            Tile targetTile = board.Grid[newTileY, newTileX];
            return IsTileWalkable(targetTile);
        }
        public (float pixelPosX, float pixelPosY) GetStartCords()
        {
            (float pixelPosX, float pixelPosY) = ConvertTileToPixel(TilePosX, TilePosY);
            pixelPosX += board.TileWidth / 2f;
            return (pixelPosX, pixelPosY);
        }
        public virtual void ChangeDirection(Vector2D direction)
        {
            this.direction = direction;
        }
        protected virtual bool IsTileWalkable(Tile tile)
        {
            return tile.IsWalkable();
        }
    }
}
