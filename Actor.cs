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
    public class Actor{
        public int PixelPosX;
        public int PixelPosY;
        public int speed;
        public Vector2D direction;
        protected Board board;
        public Actor(int TilePosX, int TilePosY, int speed, Board board)
        {
            this.board = board;
            (this.PixelPosX,this.PixelPosY) = ConvertTileToPixel(TilePosX,TilePosY);
            this.speed = speed;
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

        public virtual void ChangeDirection(Vector2D direction){
            this.direction = direction;
        }
        protected virtual bool IsTileWalkable(Tile tile){
            return tile.IsWalkable();
        }
    }
}
