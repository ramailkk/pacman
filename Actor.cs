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
        public Actor(int TilePosX, int TilePosY, int speed, Board board){
            this.PixelPosX = ConvertTileCordinatesToPixel(TilePosX, board.TileWidth);
            this.PixelPosY = ConvertTileCordinatesToPixel(TilePosY, board.TileHeight);
            this.board = board;
            this.speed = speed;
        }

         public virtual void Move()
        {
            if (direction.Equals(Vector2D.Zero))
                return;

            // Calculate new pixel position based on direction
            int newPixelX = PixelPosX + (direction.X);
            int newPixelY = PixelPosY + (direction.Y);

            // Convert to tile coordinates
            int tileX = ConvertPixelCordinatesToTile(newPixelX, board.TileWidth);
            int tileY = ConvertPixelCordinatesToTile(newPixelY, board.TileHeight);
            // Check also Actor outline collisions

            int outlineTileX = ConvertPixelCordinatesToTile(newPixelX-1 + (board.TileWidth / 2 * direction.X), board.TileWidth);
            int outlineTileY = ConvertPixelCordinatesToTile(newPixelY-1 + (board.TileHeight / 2 * direction.Y), board.TileHeight);

            Tile outlineTile = board.Grid[outlineTileY, outlineTileX];
            if (!IsTileWalkable(outlineTile))
            {
                return;
            }

            Tile targetTile = board.Grid[tileY, tileX];


            if (IsTileWalkable(targetTile))
            {
                if (direction.X == 0)
                {
                    PixelPosX = ConvertTileCordinatesToPixel(tileX,board.TileWidth);
                    PixelPosY = newPixelY;
                }
                else
                {
                    PixelPosX = newPixelX;
                    PixelPosY = ConvertTileCordinatesToPixel(tileY,board.TileHeight);
                }
            }
        }


        public bool IsCollisionWithActor(Actor other)
        {
            int myTileX = ConvertPixelCordinatesToTile(this.PixelPosX, board.TileWidth);
            int myTileY = ConvertPixelCordinatesToTile(this.PixelPosY, board.TileHeight);
            int otherTileX = ConvertPixelCordinatesToTile(other.PixelPosX, board.TileWidth);
            int otherTileY = ConvertPixelCordinatesToTile(other.PixelPosY, board.TileHeight);

            return (myTileX == otherTileX) && (myTileY == otherTileY);
        }

        public int ConvertPixelCordinatesToTile(int PixelPos, int Dim){
            return PixelPos / Dim;
        }

        public int ConvertTileCordinatesToPixel(int TilePos, int Dim){
            return (TilePos * Dim) + (Dim/2);
        }
        public virtual void ChangeDirection(Vector2D direction){
            this.direction = direction;
        }
        protected virtual bool IsTileWalkable(Tile tile){
            return tile.IsWalkable();
        }
    }
}
