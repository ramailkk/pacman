namespace PacManGame
{
    public enum TileType
    {
        Empty = 0,
        Wall = 1,
        Dot = 2,
        PowerPellet = 3,
        Fruit = 4,
        GhostHouse = 5,
        DeadSpace = 6
    }


    public class Tile
    {
        public int DimX { get; set; }
        public int DimY { get; set; }
        public TileType Type { get; set; }

        public Tile(int Dim_x, int Dim_y, TileType type)
        {
            DimX = Dim_x;
            DimY = Dim_y;
            Type = type;
        }

        public bool IsWalkable()
        {
            return this.Type != TileType.Wall && this.Type != TileType.DeadSpace;
        }

        public bool IsPellet()
        {
            return this.Type == TileType.Dot || this.Type == TileType.PowerPellet;
        }

        public void RemovePellet()
        {
            this.Type = TileType.Empty;
        }

    }
}