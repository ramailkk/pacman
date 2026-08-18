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
        public int PixelPosX;
        public int PixelPosY;
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
            PixelPosX += (board.TileWidth / 2);
        }

        public int GetStepsThisTick()
        {
            accumulator += speed;
            int steps = accumulator / 100;
            accumulator -= steps * 100;
            return steps;
        }

        public bool IsCollisionWithActor(Actor other)
        {
            (int myTileX, int myTileY) = ConvertPixelToTile(this.PixelPosX, this.PixelPosY);
            (int otherTileX, int otherTileY) = ConvertPixelToTile(other.PixelPosX, other.PixelPosY);
            return (myTileX == otherTileX) && (myTileY == otherTileY);
        }
        public (int tileX, int tileY) ConvertPixelToTile(int pixelX, int pixelY)
        {
            return (pixelX / board.TileWidth, pixelY / board.TileHeight);
        }
        public (int pixelX, int pixelY) ConvertTileToPixel(int tileX, int tileY)
        {
            return ((tileX * board.TileWidth) + (board.TileWidth / 2),
                    (tileY * board.TileHeight) + (board.TileHeight / 2));
        }

        public (int pixelX, int pixelY) CheckForTunnel(int newPixelX, int newPixelY)
        {
            int totalPixelX = board.TileWidth * board.Grid.GetLength(1);
            int totalPixelY = board.TileHeight * board.Grid.GetLength(0);

            int wrappedX = ((newPixelX % totalPixelX) + totalPixelX) % totalPixelX;
            int wrappedY = ((newPixelY % totalPixelY) + totalPixelY) % totalPixelY;

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

            int newPixelX = PixelPosX + currentDirection.X;
            int newPixelY = PixelPosY + currentDirection.Y;
            (newPixelX, newPixelY) = CheckForTunnel(newPixelX, newPixelY);
            
            (int tileX, int tileY) = ConvertPixelToTile(newPixelX, newPixelY);
            Tile targetTile = board.Grid[tileY, tileX];

            if (!IsTileWalkable(targetTile))
                return false;
            
            (int StartingTileX, int StartingTileY) = ConvertPixelToTile(PixelPosX, PixelPosY);
            (int CenterX, int CenterY) = ConvertTileToPixel(StartingTileX,StartingTileY);
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
        public (int pixelPosX, int pixelPosY) GetStartCords()
        {
            (int pixelPosX, int pixelPosY) = ConvertTileToPixel(TilePosX, TilePosY);
            pixelPosX += (board.TileWidth / 2);
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
