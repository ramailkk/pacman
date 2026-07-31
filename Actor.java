package PacMan;


public class Actor {
    public int x;
    public int y;
    public int speed;
    public int x_direction;  //  -1 FOR LEFT <- and 1 FOR RIGHT ->
    public int y_direction;  //  -1 FOR DOWN V  and 1 FOR UP ^
    protected Board board;

    public Actor(int x, int y, int speed, Board board) {
        this.x = x;
        this.y = y;
        this.speed = speed;
        this.board = board;
    }
    
    public void Move(){
        int new_X = this.x + this.x_direction * this.speed;
        int new_Y = this.y + this.y_direction * this.speed;
        if (board.isWalkable(new_X, new_Y)){
            this.x = new_X;
            this.y = new_Y;
        }
    }
}