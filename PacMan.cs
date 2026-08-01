namespace PacManGame
{
    public class PacMan : Actor
    {
        public int lives;
        public int multiplier;
        public bool spree;

        public PacMan(int x, int y, int speed, Board board, int lives) : base(x, y, speed, board)
        {
            this.lives = lives;
            this.multiplier = 1;
            this.spree = false;
        }
        public void Move()
        {
            switch (direction)
            {
                case Direction.Up:
                    int Y = ConvertPixelCordinatesToTile(PixelPosY-1, board.TileHeight);
                    Tile NewTileY = board.Grid[ConvertPixelCordinatesToTile(PixelPosX, board.TileWidth), Y];
                    if (NewTileY.IsWalkableForPacMan())
                        PixelPosY = ConvertTileCordinatesToPixel(Y, board.TileHeight);
                    break;

                case Direction.Down:
                    Y = ConvertPixelCordinatesToTile(PixelPosY+1, board.TileHeight);
                    NewTileY = board.Grid[ConvertPixelCordinatesToTile(PixelPosX, board.TileWidth),  Y];
                    if (NewTileY.IsWalkableForPacMan())
                        PixelPosY = ConvertTileCordinatesToPixel(Y, board.TileHeight);
                    break;

                case Direction.Left:
                    int X = ConvertPixelCordinatesToTile(PixelPosX-1, board.TileWidth);
                    Tile NewTileX = board.Grid[X,  ConvertPixelCordinatesToTile(PixelPosY, board.TileHeight)];
                    if (NewTileX.IsWalkableForPacMan())
                        PixelPosX = ConvertTileCordinatesToPixel(X, board.TileWidth);
                    break;

                case Direction.Right:
                    X = ConvertPixelCordinatesToTile(PixelPosX+1, board.TileWidth);
                    NewTileX = board.Grid[X,  ConvertPixelCordinatesToTile(PixelPosY, board.TileHeight)];
                    if (NewTileX.IsWalkableForPacMan())
                        PixelPosX = ConvertTileCordinatesToPixel(X, board.TileWidth);
                    break;
                default:
                    return;
            }
        }
    }
}
