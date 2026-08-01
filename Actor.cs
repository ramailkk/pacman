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
            this.PixelPosX = ConvertTileCordinatesToPixel(TilePosX, board.TileHeight)+ (board.TileHeight / 2); ;
            this.PixelPosY = ConvertTileCordinatesToPixel(TilePosY, board.TileWidth)+ (board.TileWidth / 2); ;
            this.board = board;
            this.speed = speed;
        }

        public void MoveActor()
        {
            switch (this.direction)
            {
                case Direction.Up:
                    Tile NewTileX = board.Grid[ConvertPixelCordinatesToTile(PixelPosX-1, board.TileHeight),  ConvertPixelCordinatesToTile(PixelPosY, board.TileWidth)];
                    if (NewTileX.IsWalkable())
                        PixelPosX--;
                    break;

                case Direction.Down:
                    NewTileX = board.Grid[ConvertPixelCordinatesToTile(PixelPosX+1, board.TileHeight),  ConvertPixelCordinatesToTile(PixelPosY, board.TileWidth)];
                    if (NewTileX.IsWalkable())
                        PixelPosX++;
                    break;

                case Direction.Left:
                    Tile NewTileY = board.Grid[ConvertPixelCordinatesToTile(PixelPosX, board.TileHeight),  ConvertPixelCordinatesToTile(PixelPosY-1, board.TileWidth)];
                    if (NewTileY.IsWalkable())
                        PixelPosY--;
                    break;

                case Direction.Right:
                    NewTileY = board.Grid[ConvertPixelCordinatesToTile(PixelPosX, board.TileHeight),  ConvertPixelCordinatesToTile(PixelPosY+1, board.TileWidth)];
                    if (NewTileY.IsWalkable())
                        PixelPosY++;
                    break;
                default:
                    return;

            }
        }

        public static int ConvertPixelCordinatesToTile(int PixelPos, int Dim)
        {
            return PixelPos / Dim;
        }

        public static int ConvertTileCordinatesToPixel(int TilePos, int Dim)

        {
            return TilePos * Dim;
        }


    }
}
