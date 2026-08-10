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
        public Random random;
        public Board Board;
        public Fruit(int tilePosX, int tilePosY, Board board)
        {
            Board = board;
            (PixelPosX, PixelPosY) = ConvertTileToPixel(tilePosX, tilePosY);
            PixelPosX += Board.TileWidth/2;
            isActive = false;
            FruitTimer = 0;
            random = new Random();
        }

        public void Update()
        {
            if (!isActive)
            {
                if (Board.TotalDots - Board.RemainingDots == 70)
                    SetActive();
                else if (Board.TotalDots - Board.RemainingDots == 170)
                    SetActive();
            }
            else
            {
                if (FruitTimer > 0)
                    FruitTimer--;
                else
                    SetInActive();
            }
        }
        public bool IsActive()
        {
            return isActive;
        }
        public void SetActive()
        {
            isActive = true;
            FruitTimer = (int)((9.0 + (random.NextDouble())) * 60);
        }
        public void SetInActive()
        {
            isActive = false;
        }
        public (int pixelX, int pixelY) ConvertTileToPixel(int tileX, int tileY)
        {
            return ((tileX * Board.TileWidth) + (Board.TileWidth / 2),
                    (tileY * Board.TileHeight) + (Board.TileHeight / 2));
        }
    }
}