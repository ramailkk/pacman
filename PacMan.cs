namespace PacManGame
{
    public class PacMan(int x, int y, int speed, Board board, int lives) : Actor(x, y, speed, board)
    {
        public int LIVES = lives;
        public int MULT = 1;
        public bool SPREE = false;

        protected override bool IsTileWalkable(Tile tile)
        {
            return tile.IsWalkableForPacMan();
        }

        public override void Move()
        {
            base.Move();
            CheckConsumables();
        }
        public void CheckConsumables()
        {
            int tileX = ConvertPixelCordinatesToTile(this.PixelPosX, board.TileWidth);
            int tileY = ConvertPixelCordinatesToTile(this.PixelPosY, board.TileHeight);
            Tile tile = this.board.Grid[tileX, tileY];

            if (tile.HasPowerPellet())
            {
                tile.RemoveDotOrPellet();
                board.UpdatePowerScore();
            }
            else if (tile.HasDot())
            {
                // Check first if the pixel position of PacMan is centered to the Tile
                int CenteredX = ConvertTileCordinatesToPixel(tileX, board.TileWidth);
                int CenteredY = ConvertTileCordinatesToPixel(PixelPosY, board.TileHeight);

                if (CenteredX == this.PixelPosX && CenteredY == this.PixelPosY)
                {
                    tile.RemoveDotOrPellet();
                    board.UpdateDotScore();
                }
            }
        }
    }
}

