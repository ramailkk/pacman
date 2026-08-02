namespace PacManGame
{
public class PacMan : Actor {
        public int LIVES;
        public int MULT;
        public bool SPREE;
        public Vector2D bufferDirection;

        // Secondary constructor
        public PacMan(int x, int y, int speed, Board board, int lives) : base(x, y, speed, board) {
            this.LIVES = lives;
            this.MULT = 1;
            this.SPREE = false;
            this.bufferDirection = Vector2D.Zero;
            this.direction = Vector2D.Down;
        }
        protected override bool IsTileWalkable(Tile tile){
            return tile.IsWalkableForPacMan();
        }

        public override void Move(){
            DecideDirection();
            // Calculate new pixel position based on direction
            int newPixelX = PixelPosX + (direction.X);
            int newPixelY = PixelPosY + (direction.Y);
            // Convert to tile coordinates
            int tileX = ConvertPixelCordinatesToTile(newPixelX, board.TileWidth);
            int tileY = ConvertPixelCordinatesToTile(newPixelY, board.TileHeight);

            if (IsValidMove(direction)){
                if (direction.X == 0){
                    PixelPosX = ConvertTileCordinatesToPixel(tileX,board.TileWidth);
                    PixelPosY = newPixelY;
                }
                else{
                    PixelPosX = newPixelX;
                    PixelPosY = ConvertTileCordinatesToPixel(tileY,board.TileHeight);
                }
                CheckConsumables();
            }
        }
        public bool IsValidMove(Vector2D currentDirection)
        {
            if (currentDirection.Equals(Vector2D.Zero))
                return false;
            int newPixelX = PixelPosX + (currentDirection.X);
            int newPixelY = PixelPosY + (currentDirection.Y);

            // Convert to tile coordinates
            int tileX = ConvertPixelCordinatesToTile(newPixelX, board.TileWidth);
            int tileY = ConvertPixelCordinatesToTile(newPixelY, board.TileHeight);

            // Check also Actor outline collisions
            int outlineTileX = ConvertPixelCordinatesToTile(newPixelX + (board.TileWidth / 2 * currentDirection.X), board.TileWidth);
            int outlineTileY = ConvertPixelCordinatesToTile(newPixelY + (board.TileHeight / 2 * currentDirection.Y), board.TileHeight);
            Tile outlineTile = board.Grid[outlineTileY, outlineTileX];

            if (!IsTileWalkable(outlineTile))
                return false;

            Tile targetTile = board.Grid[tileY, tileX];
            return IsTileWalkable(targetTile);
        }

        public override void ChangeDirection(Vector2D direction){
            this.direction = direction;
        }
        public void DecideDirection()
        {
            if (bufferDirection.Equals(Vector2D.Zero)){
                return;
            }
            if (IsValidMove(bufferDirection)){
                ChangeDirection(bufferDirection);
                ChangeBufferDirection(Vector2D.Zero);
            }
        }
        public void ChangeBufferDirection(Vector2D bufferDirection)
        {
            this.bufferDirection = bufferDirection;

        }
        public void CheckConsumables(){
            int tileX = ConvertPixelCordinatesToTile(this.PixelPosX, board.TileWidth);
            int tileY = ConvertPixelCordinatesToTile(this.PixelPosY, board.TileHeight);
            Tile tile = this.board.Grid[tileY, tileX];
            if (tile.HasPowerPellet()){
                tile.RemoveDotOrPellet();
                board.UpdatePowerScore();
            }
            else if (tile.HasDot()){
                tile.RemoveDotOrPellet();
                board.UpdateDotScore();
            }
        }
    }
}
