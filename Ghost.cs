namespace PacManGame
{
    public enum ModeType
    {
        Chase,
        Scatter,
        Fright
    }
    public class Ghost : Actor
    {
        public ModeType MODE;

        public Ghost(int TilePosX, int TilePosY, int speed, Board board) : base(TilePosX, TilePosY, speed, board)
        {
            

        }
    }
}