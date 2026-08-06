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
        DeadSpace = 6,
        Tunnel = 7,
        RedZone = 8,
        HouseGate = 9
    }
    public class Tile
    {
        public int TileHeight { get; set; }
        public int TileWidth { get; set; }
        public TileType Type { get; set; }

        public Tile(int tileHeight, int tileWidth, TileType type)
        {
            TileHeight = tileHeight;
            TileWidth = tileWidth;
            Type = type;
        }

        public bool IsWalkable()
        {
            return !Type.Equals(TileType.Wall) && !Type.Equals(TileType.DeadSpace);
        }

        public bool IsWalkableForPacMan()
        {
            return IsWalkable() && !Type.Equals(TileType.GhostHouse);
        }
        public bool IsGhostHouse()
        {
            return Type.Equals(TileType.GhostHouse);
        }
        public bool HasDot()
        {
            return Type.Equals(TileType.Dot);
        }
        public bool HasPowerPellet()
        {
            return Type.Equals(TileType.PowerPellet);
        }

        public void RemoveDotOrPellet()
        {
            this.Type = TileType.Empty;
        }
        public bool IsRedZone()
        {
            return Type.Equals(TileType.RedZone);
        }
        public bool IsTunnel()
        {
            return Type.Equals(TileType.Tunnel);
        }
    }
}