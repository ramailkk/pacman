using System.ComponentModel.Design;

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
        public LevelTimer timer;
        public int FreezeFramesRemaining;

        public PacMan(int x, int y, Board board, int lives) : base(x, y, board)
        {
            base.Initialize();
            LIVES = lives;
            MULT = 1;
            SPREE = false;
            bufferDirection = Vector2D.Zero;
            direction = Vector2D.Down;
            FreezeFramesRemaining = 0;
        }

        public void UpdateLoop()
        {
            CheckSpeed();
            Move();
            CheckConsumables();
        }
        public void Move()
        {
            if (!this.CanMoveThisTick())
                return;
            if (FreezeFramesRemaining > 0) { 
                FreezeFramesRemaining--;
                    return; 
            }    
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
        public void CheckFreezeFrames()
        {
            if (FreezeFramesRemaining > 0) FreezeFramesRemaining--;
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
        public void SetTimer(LevelTimer timer)
        {
            this.timer = timer;
        }
        public void CheckGhostCollisions()
        {
            foreach (var ghost in ghosts)
            {
                if (this.IsCollisionWithActor(ghost))
                {
                    if (ghost.CurrentMode.Equals(ModeType.Fright))
                    {
                        // ghost.UpdateMode(ModeType.Dead);
                        board.Score += 200 * MULT;
                        MULT *= 2; //Reset this back to 1 when Fright is Intiaited 
                    }
                    // else if (!ghost.CurrentMode.Equals(ModeType.Dead))
                    // {
                    //     LIVES--;
                    //     // Apply some logic about restarting the game
                    // }
                }
            }
        }


        public void CheckSpeed()
        {
            int Level = board.LEVEL;
            if (timer.isFrightMode())
                speed = LevelSpecs.GetEntry(Level, LevelSpecs.FrightPacManSpeed);
            else
                speed = LevelSpecs.GetEntry(Level, LevelSpecs.PacManSpeed);
        }
        public void CheckConsumables()
        {
            (int tileX, int tileY) = ConvertPixelToTile(this.PixelPosX, this.PixelPosY);
            Tile tile = this.board.Grid[tileY, tileX];

            if (tile.HasPowerPellet())
            {
                tile.RemoveDotOrPellet();
                FreezeFramesRemaining = 3;
                MULT = 1;
                timer.InitiateFrightTimer();
                foreach (var ghost in ghosts)
                    ghost.UpdateMode(ModeType.Fright);
                board.UpdatePowerScore();
            }
            else if (tile.HasDot())
            {
                tile.RemoveDotOrPellet();
                FreezeFramesRemaining = 1;
                board.UpdateDotScore();
            }
        }
    }
}
