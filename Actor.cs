namespace PacManGame
{
    public class Actor
    {
        public int x;
        public int y;
        public int speed;
        public int xDirection;  // -1 FOR LEFT <- and 1 FOR RIGHT ->
        public int yDirection;  // -1 FOR DOWN V  and 1 FOR UP ^
        protected Board board;

        public Actor(int x, int y, int speed, Board board)
        {
            this.x = x;
            this.y = y;
            this.speed = speed;
            this.board = board;
        }

        public void Move()
        {
            int newX = this.x + this.xDirection * this.speed;
            int newY = this.y + this.yDirection * this.speed;
            if (board.IsWalkable(newX, newY))
            {
                this.x = newX;
                this.y = newY;
            }
        }
    }
}
