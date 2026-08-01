namespace PacManGame
{
    public class Actor(int x, int y, int speed, Board board)
    {
        public int posX = x;
        public int posY = y;
        public int speed = speed;
        public int xDirection;  // -1 FOR LEFT <- and 1 FOR RIGHT ->
        public int yDirection;  // -1 FOR DOWN V  and 1 FOR UP ^
        protected Board board = board;
    }
}
