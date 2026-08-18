using PacManGame;

public class LevelTimer
{
    public int ModeTimer;  // frames elapsed in current scatter/chase phase
    public int ModeTimerIndex;
    public ModeType GlobalMode; // Scatter or Chase (not Fright - that's separate)
    public int FrightTimer;
    public int[] FrightSchedule;
    public bool isBlue;
    public int CurrentLevel;
    public int[][] GlobalSchedule;
    const int FlashHalfCycleFrames = 14;

    public List<Ghost> Ghosts;
    public LevelTimer(List<Ghost> ghosts)
    {
        isBlue = true;
        Ghosts = ghosts;
        Initialize();
        GlobalSchedule =
        [
            [7 * 60, 20 * 60, 7 * 60, 20 * 60, 5 * 60, 20 * 60, 5 * 60], //Level 1 
            [7 * 60, 20 * 60, 7 * 60, 20 * 60, 5 * 60, 1033 * 60, 1],    //Level 2-4
            [5 * 60, 20 * 60, 5 * 60, 20 * 60, 5 * 60, 1037 * 60, 1]     //Level 5+
        ];
        FrightSchedule = new int[] { 6 * 60, 5 * 60, 4 * 60, 3 * 60, 2 * 60, 5 * 60, 2 * 60, 2 * 60, 1 * 60, 5 * 60, 2 * 60, 1 * 60, 1 * 60, 3 * 60, 1 * 60, 1 * 60 };
    }

    public void Initialize()
    {
        ModeTimer = 0;
        ModeTimerIndex = 0;
        GlobalMode = ModeType.Scatter;
        FrightTimer = 0;
        UpdateAllGhostMode(GlobalMode);
    }



    public void UpdateTimer(bool isFrozen)
    {
        if (isFrozen)
            return;
        if (FrightTimer > 0)
        {
            int flashCount = LevelSpecs.GetEntry(CurrentLevel, LevelSpecs.FlashCount);
            int flashWindowFrames = flashCount * FlashHalfCycleFrames * 2;

            if (FrightTimer <= flashWindowFrames)
            {
                int framesIntoFlash = flashWindowFrames - FrightTimer;
                isBlue = (framesIntoFlash / FlashHalfCycleFrames) % 2 == 0;
            }
            else
            {
                isBlue = true; // solid blue outside the flash window
            }

            FrightTimer--;
            if (FrightTimer == 0)
                UpdateAllGhostMode(GlobalMode);
            return;
        }

        if (ModeTimerIndex == GlobalSchedule[0].Length)
            return;

        if (ModeTimer > GlobalSchedule[GetGlobalLevelIndex()][ModeTimerIndex])
        {
            ModeTimer = 0;
            ModeTimerIndex++;
            GlobalMode = GlobalMode.Equals(ModeType.Chase) ? ModeType.Scatter : ModeType.Chase;
            UpdateAllGhostMode(GlobalMode);
        }
        ModeTimer++;
    }
    public void InitiateFrightTimer()
    {
        isBlue = true;
        FrightTimer = FrightSchedule[GetFrightLevelIndex()];
    }

    public int GetGlobalLevelIndex()
    {
        return CurrentLevel == 1 ? 0 : CurrentLevel >= 5 ? 2 : 1;
    }
    public int GetFrightLevelIndex()
    {
        return CurrentLevel >= 21 ? FrightSchedule.Length - 1 : CurrentLevel - 1;
    }
    public ModeType GetCurrentMode()
    {
        return FrightTimer == 0 ? GlobalMode : ModeType.Fright;
    }
    public void UpdateAllGhostMode(ModeType Mode)
    {
        foreach (var ghost in Ghosts)
        {
            if (!ghost.CurrentMode.Equals(ModeType.Dead))
                ghost.UpdateMode(Mode);
        }
    }
    public bool IsFrightMode()
    {
        return FrightTimer > 0;
    }
    public void SetCurrentLevel(int Level)
    {
        this.CurrentLevel = Level;
    }
}

