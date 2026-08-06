namespace PacManGame
{
    public static class Utils
    {
        public static int ManhattanDistanceBetweenTiles(int TileX, int TileY, int targetTileX, int targetTileY)
        {
            return (int)(Math.Abs(TileX - targetTileX) + Math.Abs(TileY - targetTileY));
        }
        public static int SquaredEuclideanDistanceBetweenTiles(int TileX, int TileY, int targetTileX, int targetTileY)
        {
            int dx = TileX - targetTileX;
            int dy = TileY - targetTileY;
            return dx * dx + dy * dy;
        }
        public static int EuclideanDistanceBetweenTiles(int TileX, int TileY, int targetTileX, int targetTileY)
        {
            int dx = TileX - targetTileX;
            int dy = TileY - targetTileY;
            return (int)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}