namespace PacManGame
{

public enum TimerType
{
    // Game state timers
    GameStart,          // Initial game start countdown
    StartTimer,         // "PLAYER ONE" message display
    PacManDeath,        // Pac-Man death animation duration
    GhostEaten,         // Ghost eaten animation/sound duration
    
    // Animation timers
    PacManAnim,         // Pac-Man animation frame switching
    GhostAnim,          // Ghost animation frame switching
    PelletAnim,         // Power pellet blinking animation
    BufferFrames,       // Input buffer frames
    
    // Level/Gameplay timers
    LevelStart,         // Level start delay
    FruitDisplay,       // Fruit display duration
    BonusLife,          // Bonus life awarded animation
    HighScoreFlash,     // High score flashing animation
}

public static class TimerManager
{
    private static Dictionary<TimerType, int> timers = new();
    private static Dictionary<TimerType, int> maxValues = new();
    private static Dictionary<TimerType, bool> paused = new();
    private static HashSet<TimerType> activeTimers = new();
    
    // Default timer durations (in frames at 60 FPS)
    private static readonly Dictionary<TimerType, int> DefaultDurations = new()
    {
        { TimerType.GameStart, 120 },      
        { TimerType.StartTimer, 100 },     
        { TimerType.PacManDeath, 60 },     
        { TimerType.GhostEaten, 60 },      
        { TimerType.PacManAnim, 3 },       
        { TimerType.GhostAnim, 3 },        
        { TimerType.PelletAnim, 60 },    
        { TimerType.LevelStart, 300 },      
    };
    
    // Initialize all timers with default values
    public static void Initialize()
    {
        foreach (var kvp in DefaultDurations)
        {
            SetTimer(kvp.Key, kvp.Value);
        }
        Pause(TimerType.GhostEaten);
        Pause(TimerType.PacManDeath);
        Pause(TimerType.LevelStart);
    }
    
    // Set a timer with specific duration (decrementing)
    public static void SetTimer(TimerType id, int frames)
    {
        timers[id] = frames;
        maxValues[id] = frames;
        paused[id] = false;
        activeTimers.Add(id);
    }
    
    // Set a counter that counts up (incrementing)
    public static void SetCounter(TimerType id, int startValue = 0)
    {
        timers[id] = startValue;
        maxValues[id] = startValue;
        paused[id] = false;
        activeTimers.Add(id);
    }

    public static bool IsPaused(TimerType id)
        {
            return paused[id];
        }
    
    // Update all timers - call once per frame
    public static void Update()
    {
        foreach (var id in activeTimers.ToList())
        {
            if (paused.TryGetValue(id, out bool isPaused) && isPaused)
                continue;
                
            if (!timers.TryGetValue(id, out int value))
                continue;
            
            // Check if this is a counter (starts at 0 and goes up) or timer
            if (maxValues.TryGetValue(id, out int max) && max == 0 && value == 0)
            {
                // Counter mode - increment
                timers[id] = value + 1;
            }
            else if (value > 0)
            {
                // Timer mode - decrement
                timers[id] = value - 1;
            }
        }
    }
    
    // Check if a timer is running (countdown not reached 0, or counter > 0)
    public static bool IsRunning(TimerType id)
    {
        return timers.TryGetValue(id, out int value) && value > 0;
    }
    
    // Check if a countdown timer is done (reached 0)
    public static bool IsDone(TimerType id)
    {
        return timers.TryGetValue(id, out int value) && value <= 0;
    }
    
    // Get current timer value
    public static int GetValue(TimerType id)
    {
        return timers.TryGetValue(id, out int value) ? value : 0;
    }
    
    // Reset a timer to its initial value
    public static void ResetTimer(TimerType id)
    {
        if (maxValues.TryGetValue(id, out int max))
            timers[id] = max;
    }
    
    // Reset with new value
    public static void ResetTimer(TimerType id, int newValue)
    {
        timers[id] = newValue;
        maxValues[id] = newValue;
    }
    
    // Pause/Resume specific timer
    public static void Pause(TimerType id)
    {
        paused[id] = true;
    }
    public static void PauseAndReset(TimerType id)
        {
            Pause(id);
            ResetTimer(id);
        }
    
    public static void Resume(TimerType id)
    {
        paused[id] = false;
    }
    
    // Pause/Resume all timers
    public static void PauseAll()
    {
        var keys = paused.Keys.ToList();
        foreach (var key in keys)
            paused[key] = true;
    }
    
    public static void ResumeAll()
    {
        var keys = paused.Keys.ToList();
        foreach (var key in keys)
            paused[key] = false;
    }
    
    // Remove a timer
    public static void RemoveTimer(TimerType id)
    {
        timers.Remove(id);
        maxValues.Remove(id);
        paused.Remove(id);
        activeTimers.Remove(id);
    }
    
    // Check if timer exists
    public static bool Exists(TimerType id) => timers.ContainsKey(id);
}
}