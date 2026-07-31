package PacMan;
public class PacMan extends Actor {

    int lives;
    int multipler;
    boolean spree;

    public PacMan(int x, int y, int speed, Board board, int lives) {
        super(x, y, speed, board);
        this.lives = lives;
        this.multipler = 1;
        this.spree = false;
    }

    public void move(){
        // Figure out movement on a pixel level instead of array level
    }

    public void eatDot(){
        if (board.board[x][y] == 0){
                board.incrementScore("dot");
                board.decrementDot();
        }
    }

}