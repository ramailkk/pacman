namespace PacManGame
{

    public enum State
    {
        Normal,
        Power,
        Dead,
    }
    public class PacMan : Actor
    {
        public int LIVES;
        public int MULT;
        public bool SPREE;
        public Vector2D bufferDirection;
        public List<Ghost> ghosts;
        public PacMan(int x, int y, int speed, Board board, int lives) : base(x, y, speed, board)
        {
            this.LIVES = lives;
            this.MULT = 1;
            this.SPREE = false;
            this.bufferDirection = Vector2D.Zero;
            this.direction = Vector2D.Down;
        }

        public void UpdateLoop()
        {
            Move();
            CheckConsumables();
            CheckGhostCollisions();
        }
        public void Move()
        {
            if (!this.CanMoveThisTick())
                return;
            DecideDirection();
            // Calculate new pixel position based on direction
            int newPixelX = PixelPosX + (direction.X);
            int newPixelY = PixelPosY + (direction.Y);
            (newPixelX, newPixelY) = CheckForTunnel(newPixelX, newPixelY);
            (int tileX, int tileY) = ConvertPixelToTile(newPixelX, newPixelY);

            if (IsValidMove(direction))
            {
                (PixelPosX, PixelPosY) = ConvertTileToPixel(tileX, tileY);
                if (direction.X == 0)
                    PixelPosY = newPixelY;
                else
                    PixelPosX = newPixelX;
            }
        }
        public bool IsValidMove(Vector2D currentDirection)
        {
            if (currentDirection.Equals(Vector2D.Zero))
                return false;

            int newPixelX = PixelPosX + (currentDirection.X);
            int newPixelY = PixelPosY + (currentDirection.Y);
            (newPixelX, newPixelY) = CheckForTunnel(newPixelX, newPixelY);

            (int tileX, int tileY) = ConvertPixelToTile(newPixelX, newPixelY);

            int outlinePixelX = newPixelX + (board.TileWidth / 2 * currentDirection.X);
            int outlinePixelY = newPixelY + (board.TileHeight / 2 * currentDirection.Y);

            // Wrap outline pixels too (important if offset pushes beyond edges)
            (outlinePixelX, outlinePixelY) = CheckForTunnel(outlinePixelX, outlinePixelY);
            (int outlineTileX, int outlineTileY) = ConvertPixelToTile(outlinePixelX, outlinePixelY);

            Tile outlineTile = board.Grid[outlineTileY, outlineTileX];

            if (!IsTileWalkable(outlineTile))
                return false;

            Tile targetTile = board.Grid[tileY, tileX];
            return IsTileWalkable(targetTile);
        }

        public override void ChangeDirection(Vector2D direction)
        {
            this.direction = direction;
        }

        public void DecideDirection()
        {
            if (bufferDirection.Equals(Vector2D.Zero))
                return;
            if (IsValidMove(bufferDirection))
            {
                ChangeDirection(bufferDirection);
                ChangeBufferDirection(Vector2D.Zero);
            }
        }
        public void ChangeBufferDirection(Vector2D bufferDirection)
        {
            this.bufferDirection = bufferDirection;

        }
        protected override bool IsTileWalkable(Tile tile)
        {
            return tile.IsWalkableForPacMan();
        }
        public (int TileX, int TileY) GetPacManTile()
        {
            return ConvertPixelToTile(PixelPosX, PixelPosY);
        }

        public void SetGhosts(List<Ghost> ghosts)
        {
            this.ghosts = ghosts;
        }
        public void CheckGhostCollisions()
        {
            foreach (var ghost in ghosts)
            {
                if (this.IsCollisionWithActor(ghost))
                {
                    if (ghost.CurrentMode.Equals(ModeType.Fright)){
                        ghost.UpdateMode(ModeType.Dead);
                        board.Score = 200 * MULT;
                        MULT *= 2; //Reset this back to 1 when Fright is Intiaited 
                    }
                    else if (!ghost.CurrentMode.Equals(ModeType.Dead))
                    {
                        LIVES--;
                        // Apply some logic about restarting the game
                    }
                }
            }
        }
        public void CheckConsumables()
        {
            (int tileX, int tileY) = ConvertPixelToTile(this.PixelPosX, this.PixelPosY);
            Tile tile = this.board.Grid[tileY, tileX];

            if (tile.HasPowerPellet()){
                tile.RemoveDotOrPellet();
                foreach (var ghost in ghosts)
                    ghost.UpdateMode(ModeType.Fright);
                board.UpdatePowerScore();
            }
            else if (tile.HasDot())
            {
                tile.RemoveDotOrPellet();
                board.UpdateDotScore();
            }
        }
    }
}
