namespace PacManGame
{
    public class PacMan : Actor
    {
        public int lives;
        public int multiplier;
        public bool spree;

        public PacMan(int x, int y, int speed, Board board, int lives) : base(x, y, speed, board)
        {
            this.lives = lives;
            this.multiplier = 1;
            this.spree = false;
        }

        public new void Move()
        {
            // Figure out movement on a pixel level instead of array level
        }

        public void EatDot()
        {
            if (board.board[x][y] == 0)
            {
                board.IncrementScore("dot");
                board.DecrementDot();
            }
        }
    }
}
