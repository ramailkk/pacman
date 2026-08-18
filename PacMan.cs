using System.ComponentModel.Design;
using Raylib_cs;

namespace PacManGame
{
    public class PacMan : Actor
    {
        public int LIVES;
        public int MULT;
        public int EatenGhostsCounter;
        public Vector2D bufferDirection;
        public List<Ghost> ghosts;
        public LevelTimer timer;
        public Fruit Fruit;
        public int EatenDotCounter;
        public int FreezeFramesRemaining;
        public bool HasDied;
        public bool HasExtraLife;
        public bool isCornerTurn;
        private bool centering;
        private int cornerCenterX;
        private int cornerCenterY;
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
        }

        public void UpdateLoop(bool Frozen)
        {
            Fruit.Update();
            CheckSpeed();
            Move(Frozen);
            // CheckConsumables();
        }
        public void Move(bool Frozen)
        {
            if (Frozen)
                return;
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
                int newPixelX = PixelPosX + direction.X;
                int newPixelY = PixelPosY + direction.Y;
                (newPixelX, newPixelY) = CheckForTunnel(newPixelX, newPixelY);

                if (base.IsValidMove(direction))
                {
                    PixelPosX = newPixelX;
                    PixelPosY = newPixelY;
                    // crip walking my boy here..on the set
                    if (centering)
                    {
                        if (direction.X == 0)
                        {
                            PixelPosX += Math.Sign(cornerCenterX - PixelPosX);
                            if (PixelPosX == cornerCenterX)
                                centering = false;
                        }
                        else
                        {
                            PixelPosY += Math.Sign(cornerCenterY - PixelPosY);
                            if (PixelPosY == cornerCenterY)
                                centering = false;
                        }
                    }
                }
                CheckConsumables();
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
                isCornerTurn = !bufferDirection.Equals(direction)
                                && !bufferDirection.Equals(direction.Reverse());
                if (isCornerTurn)
                {
                    centering = true;
                    (int curTileX, int curTileY) = ConvertPixelToTile(PixelPosX, PixelPosY);
                    (cornerCenterX, cornerCenterY) = ConvertTileToPixel(curTileX, curTileY);
                }
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
                        SoundManager.Play(SfxType.EatGhost);
                        ghost.DeathState = DiedTransitionState.JustDied;
                        ghost.UpdateMode(ModeType.Dead);
                        ghost.UpdateGhostHouseState(GhostHouseState.Enter);
                        ghost.justDied = true;
                        board.Score += 200 * (int)Math.Pow(2, MULT);
                        MULT++; //Reset this back to 1 when Fright is Intiaited 
                        EatenGhostsCounter++;

                        // Moving ghost to the end of the list so it gets rendered at the end
                        ghosts.Remove(ghost);
                        ghosts.Add(ghost);
                    }
                    else if (!ghost.CurrentMode.Equals(ModeType.Fright) && !ghost.CurrentMode.Equals(ModeType.Dead) && !HasDied)
                    {
                        LIVES--;
                        HasDied = true;
                    }
                    break;
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
            EatenDotCounter = 0;
            Fruit.ResetForNextLevel();
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
                EatenGhostsCounter = 0;
                MULT = 1;
                timer.InitiateFrightTimer();
                foreach (var ghost in ghosts)
                {
                    if (!ghost.CurrentMode.Equals(ModeType.Dead))
                        ghost.UpdateMode(ModeType.Fright);
                }
                board.UpdatePowerScore();
            }
            else if (tile.HasDot())
            {
                tile.RemoveDotOrPellet();
                FreezeFramesRemaining = 1;
                EatenDotCounter++;
                board.UpdateDotScore();
                SoundManager.PlayWaka();
            }
            else if (Fruit.IsActive())
            {
                int pixelDistX = Math.Abs(this.PixelPosX - Fruit.PixelPosX);
                if (pixelDistX <= board.TileWidth / 2 && PixelPosY == Fruit.PixelPosY)
                {
                    SoundManager.Play(SfxType.EatFruit);
                    Fruit.SetInActive(true);
                    board.UpdateFruitScore();
                }
            }
            if (board.Score >= 10000 && !HasExtraLife)
            {
                SoundManager.Play(SfxType.ExtraLife);
                LIVES++;
                HasExtraLife = true;
            }
        }
        public bool IsGhostDead()
        {
            foreach (var ghost in ghosts)
            {
                if (ghost.DeathState.Equals(DiedTransitionState.JustDied))
                    return true;
            }
            return false;
        }
        public bool IsGhostRunningToHome()
        {
            foreach (var ghost in ghosts)
            {
                if (ghost.DeathState.Equals(DiedTransitionState.LateDied))
                    return true;
            }
            return false;
        }
        public bool IsGameOver()
        {
            return LIVES == 0;
        }
    }
}
