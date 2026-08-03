namespace PacManGame{
    public enum TileType{
        Empty = 0,
        Wall = 1,
        Dot = 2,
        PowerPellet = 3,
        Fruit = 4,
        GhostHouse = 5,
        DeadSpace = 6,
        Tunnel = 7
    }
    public class Tile{
        public int TileHeight { get; set; }
        public int TileWidth { get; set; }
        public TileType Type { get; set; }

        public Tile(int tileHeight, int tileWidth, TileType type){
            TileHeight = tileHeight;
            TileWidth = tileWidth;
            Type = type;
        }

        public bool IsWalkable(){
            return this.Type != TileType.Wall && this.Type != TileType.DeadSpace;
        }

        public bool IsWalkableForPacMan(){
            return IsWalkable() && this.Type != TileType.GhostHouse;
        }

        public bool HasDot(){
            return this.Type == TileType.Dot;
        }
        public bool HasPowerPellet(){
            return this.Type == TileType.PowerPellet;
        }

        public void RemoveDotOrPellet(){
            this.Type = TileType.Empty;
        }
        

    }
}