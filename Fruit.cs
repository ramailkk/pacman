namespace PacManGame
{
    public enum FruitType
    {
        Cherries, Strawberry, Peach, Apple, Grapes, Galaxian, Bell, Key
    }
    public class Fruit
    {
        public int PixelPosX;
        public int PixelPosY;
        public int FruitTimer;
        public bool isActive;
        public int PointsTimer;
        public Random random;
        public Board Board;
        public bool hasShown1;
        public bool hasShown2;
        public Fruit(int tilePosX, int tilePosY, Board board)
        {
            Board = board;
            (PixelPosX, PixelPosY) = ConvertTileToPixel(tilePosX, tilePosY);
            PixelPosX += Board.TileWidth / 2;
            isActive = false;
            FruitTimer = 0;
            random = new Random();
        }

        public void Update()
        {
            if (!isActive)
            {
                if (Board.TotalDots - Board.RemainingDots == 70 && !hasShown1)
                {
                    hasShown1 = true;
                    SetActive();
                }
                else if (Board.TotalDots - Board.RemainingDots == 170 && !hasShown2)
                {
                    hasShown2 = true;
                    SetActive();
                }
            }
            else
            {
                if (FruitTimer > 0)
                    FruitTimer--;
                else
                    SetInActive(false);
            }
            if (PointsTimer > 0)
                PointsTimer--;
        }
        public void ResetForNextLevel()
        {
            SetInActive(false);
            FruitTimer = 0;
            hasShown1 = false;
            hasShown2 = false;
        }
        public bool IsActive()
        {
            return isActive;
        }
        public void SetActive()
        {
            PointsTimer = 0;
            isActive = true;
            FruitTimer = (int)((9.0 + (random.NextDouble())) * 60);
        }
        public void SetInActive(bool isEaten)
        {
            isActive = false;
            if (isEaten)
                PointsTimer = 120;
        }
        public (int pixelX, int pixelY) ConvertTileToPixel(int tileX, int tileY)
        {
            return ((tileX * Board.TileWidth) + (Board.TileWidth / 2),
                    (tileY * Board.TileHeight) + (Board.TileHeight / 2));
        }
    }
}