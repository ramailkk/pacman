using Raylib_cs;

namespace PacManGame
{
    public enum SfxType
    {
        EatDot_0,
        EatDot_1,
        Death_0,
        Death_1,
        EatGhost,
        EatFruit,
        ExtraLife,
        Start,
    }

    public enum MusicType
    {
        FirstSiren_0,
        FirstSiren_1,
        SecondSiren_0,
        SecondSiren_1,
        ThirdSiren_0,
        ThirdSiren_1,
        FourthSiren_0,
        FourthSiren_1
    }

    public static class SoundManager
    {
        static readonly Dictionary<SfxType, Sound> sounds = new();
        static readonly Dictionary<MusicType, Music> music = new();
        static bool DotToggle;
        static Sound? currentSound;
        static Music? currentSiren;
        static bool isSirenPlaying = false;
        static MusicType currentSirenType;

        public static void Initialize()
        {
            Raylib.InitAudioDevice();

            // Load sound effects
            sounds[SfxType.EatDot_0] = Raylib.LoadSound("assets/sounds/eat_dot_0.wav");
            sounds[SfxType.EatDot_1] = Raylib.LoadSound("assets/sounds/eat_dot_1.wav");
            sounds[SfxType.EatGhost] = Raylib.LoadSound("assets/sounds/eat_ghost.wav");
            sounds[SfxType.EatFruit] = Raylib.LoadSound("assets/sounds/eat_fruit.wav");
            sounds[SfxType.ExtraLife] = Raylib.LoadSound("assets/sounds/extend.wav");
            sounds[SfxType.Death_0] = Raylib.LoadSound("assets/sounds/death_0.wav");
            sounds[SfxType.Death_1] = Raylib.LoadSound("assets/sounds/death_1.wav");
            sounds[SfxType.Start] = Raylib.LoadSound("assets/sounds/start.wav");

            // Load music streams (sirens)
            music[MusicType.FirstSiren_0] = Raylib.LoadMusicStream("assets/sounds/siren0.wav");
            music[MusicType.FirstSiren_1] = Raylib.LoadMusicStream("assets/sounds/siren0_firstloop.wav");
            music[MusicType.SecondSiren_0] = Raylib.LoadMusicStream("assets/sounds/siren1.wav");
            music[MusicType.SecondSiren_1] = Raylib.LoadMusicStream("assets/sounds/siren1.firstloop.wav");
            music[MusicType.ThirdSiren_0] = Raylib.LoadMusicStream("assets/sounds/siren2.wav");
            music[MusicType.ThirdSiren_1] = Raylib.LoadMusicStream("assets/sounds/siren2.firstloop.wav");
            music[MusicType.FourthSiren_0] = Raylib.LoadMusicStream("assets/sounds/siren3.wav");  // Fixed from siren0
            music[MusicType.FourthSiren_1] = Raylib.LoadMusicStream("assets/sounds/siren3.firstloop.wav");  // Fixed from siren0

            currentSirenType = MusicType.FirstSiren_0;
            PlaySirenTest();
        }

        // Test method to play FirstSiren_0
        public static void PlaySirenTest()
        {
            // Stop any currently playing siren
            if (isSirenPlaying && currentSiren.HasValue)
            {
                Raylib.StopMusicStream(currentSiren.Value);
                isSirenPlaying = false;
            }

            // Play the first siren
            currentSiren = music[MusicType.FirstSiren_0];
            currentSirenType = MusicType.FirstSiren_0;
            Raylib.PlayMusicStream(currentSiren.Value);
            isSirenPlaying = true;
        }

        public static void UpdateSiren()
        {
            if (isSirenPlaying && currentSiren.HasValue)
            {
                Raylib.UpdateMusicStream(currentSiren.Value);
            }
        }

        public static void PlaySiren(MusicType type)
        {
            if (isSirenPlaying && currentSiren.HasValue && currentSirenType == type)
                return;

            if (isSirenPlaying && currentSiren.HasValue)
            {
                Raylib.StopMusicStream(currentSiren.Value);
                isSirenPlaying = false;
            }

            // Play the new siren
            currentSiren = music[type];
            currentSirenType = type;
            Raylib.PlayMusicStream(currentSiren.Value);
            isSirenPlaying = true;
        }

        public static void StopSiren()
        {
            if (isSirenPlaying && currentSiren.HasValue)
            {
                Raylib.StopMusicStream(currentSiren.Value);
                isSirenPlaying = false;
            }
        }

        public static void Play(SfxType type)
        {
            if (currentSound.HasValue && Raylib.IsSoundPlaying(currentSound.Value))
                Raylib.StopSound(currentSound.Value);

            Sound next = sounds[type];
            Raylib.PlaySound(next);
            currentSound = next;
        }

        public static void PlayWaka()
        {
            Play(DotToggle ? SfxType.EatDot_0 : SfxType.EatDot_1);
            DotToggle = !DotToggle;
        }

        public static void Shutdown()
        {
            // Unload sound effects
            foreach (var s in sounds.Values)
                Raylib.UnloadSound(s);

            // Unload music streams
            foreach (var m in music.Values)
                Raylib.UnloadMusicStream(m);

            Raylib.CloseAudioDevice();
        }
    }
}