using PacManGame;

public class LevelTimer
{
    public int ModeTimer;       // frames elapsed in current scatter/chase phase
    public int ModeIndex;       // 0..7, index into the schedule
    public ModeType GlobalMode; // Scatter or Chase (not Fright - that's separate)
    public int FrightTimer;     // frames remaining of frightened mode; 0 = not frightened
    public int GlobalDotCounter;   // used only after a life is lost
    public bool GlobalDotCounterActive;
    public int TicksSinceLastDotEaten; // for the ghost-house release timer
    public int CurrentLevel;
}

