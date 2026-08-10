using System.ComponentModel.Design;

namespace PacManGame
{
    public class PacMan : Actor
    {
        public int LIVES;
        public int MULT;
        public Vector2D bufferDirection;
        public List<Ghost> ghosts;
        public LevelTimer timer;
        public Fruit Fruit;
        public int EatenDotCounter;
        public int FreezeFramesRemaining;
        public bool HasDied;
        public bool HasExtraLife;
        public PacMan(int x, int y, Board board, Fruit fruit) : base(x, y, board)
        {
            this.Initialize();
            Fruit = fruit;
            HasExtraLife = false;
            LIVES = 3;
        }

        public override void Initialize()
        {
            base.Initialize();
            MULT = 1;
            bufferDirection = Vector2D.Zero;
            direction = Vector2D.Left;
            FreezeFramesRemaining = 0;
            HasDied = false;
        }

        public void UpdateLoop()
        {
            Fruit.Update();
            CheckSpeed();
            Move();
            CheckConsumables();
        }
        public void Move()
        {
            int moveCount = GetStepsThisTick();
            for (int i = 0; i < moveCount; i++)
            {
            if (FreezeFramesRemaining > 0)
            {
                FreezeFramesRemaining--;
                return;
            }
            DecideDirection();
            // Calculate new pixel position based on direction
            int newPixelX = PixelPosX + (direction.X);
            int newPixelY = PixelPosY + (direction.Y);
            (newPixelX, newPixelY) = CheckForTunnel(newPixelX, newPixelY);
            (int tileX, int tileY) = ConvertPixelToTile(newPixelX, newPixelY);

            if (base.IsValidMove(direction))
            {
                (PixelPosX, PixelPosY) = ConvertTileToPixel(tileX, tileY);
                if (direction.X == 0)
                    PixelPosY = newPixelY;
                else
                    PixelPosX = newPixelX;
                }
            }
        }
        public void CheckFreezeFrames()
        {
            if (FreezeFramesRemaining > 0) FreezeFramesRemaining--;
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
                        ghost.UpdateMode(ModeType.Dead);
                        ghost.UpdateGhostHouseState(GhostHouseState.Enter);
                        board.Score += 200 * MULT;
                        MULT *= 2; //Reset this back to 1 when Fright is Intiaited 
                    }
                    else if (!ghost.CurrentMode.Equals(ModeType.Fright) && !ghost.CurrentMode.Equals(ModeType.Dead))
                    {
                        LIVES--;
                        HasDied = true;
                        Fruit.SetInActive();
                        ResetGame();
                    }
                }
            }
        }

        public void ResetGame()
        {
            this.Initialize();
            foreach (var ghost in ghosts)
            {
                ghost.Initialize();
            }
            timer.Initialize();
        }

        public void ResetForNextLevel()
        {
            HasDied = false;
            EatenDotCounter = 0;
            board.SetupNextLevel();
            timer.CurrentLevel++;
            ResetGame();
        }
        public void CheckSpeed()
        {
            int Level = board.LEVEL;
            if (timer.IsFrightMode())
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
                EatenDotCounter++;
                MULT = 1;
                timer.InitiateFrightTimer();
                foreach (var ghost in ghosts)
                {
                    if (!ghost.CurrentMode.Equals(ModeType.Dead))
                        ghost.UpdateMode(ModeType.Fright);
                }
                board.UpdatePowerScore();
                if (board.RemainingDots == 0)
                    ResetForNextLevel();
            }
            else if (tile.HasDot())
            {
                tile.RemoveDotOrPellet();
                FreezeFramesRemaining = 1;
                EatenDotCounter++;
                board.UpdateDotScore();
                // Reseting for Next Level here
                if (board.RemainingDots == 0)
                    ResetForNextLevel();
            }
            else if (Fruit.IsActive())
            {
                int pixelDistX =  Math.Abs(this.PixelPosX - Fruit.PixelPosX);
                if (pixelDistX <= board.TileWidth / 2 && PixelPosY == Fruit.PixelPosY)
                {
                    Fruit.SetInActive();
                    board.UpdateFruitScore();
                }
            }
            if (board.Score >= 10000 && !HasExtraLife)
            {
                LIVES++;
                HasExtraLife = true;
            }
        }
    }
}
