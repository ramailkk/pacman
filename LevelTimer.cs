using PacManGame;
using Raylib_cs;

public class LevelTimer
{
    public int ModeTimer;       // frames elapsed in current scatter/chase phase

    public int ModeTimerIndex;
    public ModeType GlobalMode; // Scatter or Chase (not Fright - that's separate)
    public int FrightTimer;
    public int FrightTimerIndex;
    public int[] FrightSchedule;
    public int CurrentLevel;

    public int[][] GlobalSchedule;

    public ModeType CurrentGhostMode;
    public LevelTimer(ModeType currentGhostMode)
    {
        ModeTimer = 0;
        ModeTimerIndex = 0;
        GlobalMode = ModeType.Chase;
        GlobalSchedule = new int[][]
        {
            new int[] { 7 * 60, 20 * 60, 7 * 60, 20 * 60, 5 * 60, 20 * 60, 5 * 60 }, //Level 1 
            new int[] { 7 * 60, 20 * 60, 7 * 60, 20 * 60, 5 * 60, 1033 * 60, 1 },    //Level 2-4
            new int[] { 5 * 60, 20 * 60, 5 * 60, 20 * 60, 5 * 60, 1037 * 60, 1 }     //Level 5+
        };
        FrightSchedule = new int[] { 
                6 * 60, 5 * 60, 4 * 60, 3 * 60, 2 * 60, 5 * 60, 2 * 60, 2 * 60,
                1 * 60, 5 * 60, 2 * 60, 1 * 60, 1 * 60, 3 * 60, 1 * 60, 1 * 60
                };

        CurrentGhostMode = currentGhostMode;
    }

    public void UpdateTimer()
    {
        

        if (ModeTimer > GlobalSchedule[GetGlobalLevelIndex()][ModeTimerIndex])
        {
            ModeTimer = 0;
            ModeTimerIndex++;
            GlobalMode = GlobalMode.Equals(ModeType.Chase) ? ModeType.Scatter : ModeType.Chase;
        }
    }
    public int GetGlobalLevelIndex()
    {
        return CurrentLevel == 1 ? 0 : CurrentLevel >= 5 ? 2 : 1;
    }
    public int GetFrightLevelIndex()
    {
        return CurrentLevel >= 21 ? FrightSchedule.Length - 1 : CurrentLevel;
    }

}

