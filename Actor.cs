namespace PacManGame
{
    public class Actor
    {
        public Tile currentTile;
        public int TilePosX;
        public int TilePosY;
        public int speed;
        public int xDirection;  // -1 FOR LEFT <- and 1 FOR RIGHT ->
        public int yDirection;  // -1 FOR DOWN V  and 1 FOR UP ^
        protected Board board;
        public int PixelPosX;
        public int PixelPosY;
        public int CenterPosX;
        public int CenterPosY;

        public Actor(int TilePosX, int TilePosY, int speed, Board board)
         {
            this.TilePosX = TilePosX;
            this.TilePosY = TilePosY;
            this.board = board;
            this.speed = speed;
            this.currentTile = board.Grid[TilePosX, TilePosY];
            this.CenterPosX = currentTile.DimX / 2;
            this.CenterPosY = currentTile.DimY / 2;
            this.PixelPosX = ConvertTileCordinatesToPixel(TilePosX, currentTile.DimX)+ CenterPosX;
            this.PixelPosY = ConvertTileCordinatesToPixel(TilePosY, currentTile.DimY)+ CenterPosY;
        }

        public void MoveActorInOn()
        {

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
