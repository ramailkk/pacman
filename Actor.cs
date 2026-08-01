namespace PacManGame
{
     public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
    public class Actor
    {
        public int PixelPosX;
        public int PixelPosY;
        public int speed;
        public Direction direction;
        protected Board board;

        public Actor(int TilePosX, int TilePosY, int speed, Board board)
        {
            this.PixelPosX = ConvertTileCordinatesToPixel(TilePosX, board.TileWidth);
            this.PixelPosY = ConvertTileCordinatesToPixel(TilePosY, board.TileHeight);
            this.board = board;
            this.speed = speed;
        }

        public virtual void Move()
        {
            switch (direction)
            {
                case Direction.Up:
                    int Y = ConvertPixelCordinatesToTile(PixelPosY-1, board.TileHeight);
                    Tile NewTileY = board.Grid[ConvertPixelCordinatesToTile(PixelPosX, board.TileWidth), Y];
                    if (NewTileY.IsWalkable())
                        PixelPosY = ConvertTileCordinatesToPixel(Y, board.TileHeight);
                    break;

                case Direction.Down:
                    Y = ConvertPixelCordinatesToTile(PixelPosY+1, board.TileHeight);
                    NewTileY = board.Grid[ConvertPixelCordinatesToTile(PixelPosX, board.TileWidth),  Y];
                    if (NewTileY.IsWalkable())
                        PixelPosY = ConvertTileCordinatesToPixel(Y, board.TileHeight);
                    break;

                case Direction.Left:
                    int X = ConvertPixelCordinatesToTile(PixelPosX-1, board.TileWidth);
                    Tile NewTileX = board.Grid[X,  ConvertPixelCordinatesToTile(PixelPosY, board.TileHeight)];
                    if (NewTileX.IsWalkable())
                        PixelPosX = ConvertTileCordinatesToPixel(X, board.TileWidth);
                    break;

                case Direction.Right:
                    X = ConvertPixelCordinatesToTile(PixelPosX+1, board.TileWidth);
                    NewTileX = board.Grid[X,  ConvertPixelCordinatesToTile(PixelPosY, board.TileHeight)];
                    if (NewTileX.IsWalkable())
                        PixelPosX = ConvertTileCordinatesToPixel(X, board.TileWidth);
                    break;
                default:
                    return;
            }
        }

        public int ConvertPixelCordinatesToTile(int PixelPos, int Dim)
        {
            return (PixelPos / Dim) + (Dim / 2);
        }

        public int ConvertTileCordinatesToPixel(int TilePos, int Dim)
        {
            return TilePos * Dim;
        }
        public void ChangeDirection(Direction direction)
        {
            this.direction = direction;
        }
    }
}
