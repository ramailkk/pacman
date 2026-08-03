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

         public virtual void Move()
        {
            if (direction.Equals(Vector2D.Zero))
                return;

            // Calculate new pixel position based on direction
            int newPixelX = PixelPosX + (direction.X);
            int newPixelY = PixelPosY + (direction.Y);
            (newPixelX, newPixelY) = CheckForTunnel(newPixelX, newPixelY);

            (int tileX, int tileY) = ConvertPixelToTile(newPixelX, newPixelY);

            int outlinePixelX = newPixelX + (board.TileWidth / 2 * direction.X);
            int outlinePixelY = newPixelY + (board.TileHeight / 2 * direction.Y);

            // Wrap outline pixels too (important if offset pushes beyond edges)
            (outlinePixelX, outlinePixelY) = CheckForTunnel(outlinePixelX, outlinePixelY);
            (int outlineTileX, int outlineTileY) = ConvertPixelToTile(outlinePixelX, outlinePixelY);
            Tile outlineTile = board.Grid[outlineTileY, outlineTileX];

            if (!IsTileWalkable(outlineTile))
                return;

            Tile targetTile = board.Grid[tileY, tileX];

            if (IsTileWalkable(targetTile))
            {
               (PixelPosX, PixelPosY) = ConvertTileToPixel(tileX,tileY);
                if (direction.X == 0)
                    PixelPosY = newPixelY;
                else
                    PixelPosX = newPixelX;
            }
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
