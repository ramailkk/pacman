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
        GameOver,
        LevelEnd
    }

    public static class TimerManager
    {
        private static Dictionary<TimerType, int> timers = new();
        private static Dictionary<TimerType, int> maxValues = new();
        private static Dictionary<TimerType, bool> paused = new();
        private static HashSet<TimerType> autoPause = new();
        private static readonly Dictionary<TimerType, int> DefaultDurations = new()
        {
            { TimerType.GameStart, 240 },
            { TimerType.StartTimer, 120 },
            { TimerType.PacManDeath, 60 },
            { TimerType.GhostEaten, 60 },
            { TimerType.PacManAnim, 15 },
            { TimerType.GhostAnim, 3 },
            { TimerType.PelletAnim, 60 },
            { TimerType.LevelEnd , 120},
            { TimerType.LevelStart, 260 },
            { TimerType.GameOver, 300 },
        };

        public static void Initialize()
        {
            foreach (var kvp in DefaultDurations)
            {
                SetTimer(kvp.Key, kvp.Value);
            }
            // Initially pause these timers until needed
            Pause(TimerType.GhostEaten);
            Pause(TimerType.PacManDeath);
            Pause(TimerType.LevelStart);
            Pause(TimerType.GameOver);
            Pause(TimerType.LevelEnd);

            // Animation timers should loop, not auto-pause
            autoPause.Remove(TimerType.PacManAnim);
            autoPause.Remove(TimerType.GhostAnim);
            autoPause.Remove(TimerType.PelletAnim);

        }

        public static void SetTimer(TimerType id, int frames)
        {
            timers[id] = frames;
            maxValues[id] = frames;
            paused[id] = false;
            autoPause.Add(id);
        }

        public static void Update()
        {
            foreach (var id in timers.Keys.ToList())
            {
                if (paused[id]) continue;

                if (timers[id] > 0)
                {
                    timers[id]--;
                    if (timers[id] == 0 && autoPause.Contains(id))
                        paused[id] = true;
                }
            }

        }
        public static bool IsRunning(TimerType id) => timers[id] > 0;
        public static bool IsDone(TimerType id) => timers[id] <= 0;
        public static int GetValue(TimerType id) => timers[id];

        public static void ResetTimer(TimerType id)
        {
            if (maxValues.TryGetValue(id, out int max))
                timers[id] = max;
        }

        public static void ResetTimer(TimerType id, int newValue)
        {
            timers[id] = newValue;
            maxValues[id] = newValue;
        }

        public static void Pause(TimerType id) => paused[id] = true;
        public static void Resume(TimerType id) => paused[id] = false;
        public static bool IsPaused(TimerType id) => paused[id];

        public static void PauseAndReset(TimerType id)
        {
            Pause(id);
            ResetTimer(id);
        }

        public static void ResumeAndReset(TimerType id)
        {
            ResetTimer(id);
            Resume(id);
        }
    }
}