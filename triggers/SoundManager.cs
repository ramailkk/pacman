using Raylib_cs;

namespace PacManGame
{
    public enum SfxType
    {
        EatDot_0, EatDot_1,
        Death_0, Death_1,
        EatGhost,
        EatFruit,
        ExtraLife,
        Start,
    }
    public enum MusicType
    {
        Siren0,
        Siren1,
        Siren2,
        Siren3,
        Siren4,
        Fright,
        Eyes
    }
    public static class SoundManager
    {
        static readonly Dictionary<SfxType, Sound> sounds = new();
        static readonly Dictionary<MusicType, Music> music = new();
        static bool DotToggle;
        static Sound? currentSound;
        static SfxType? currentSoundType;
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
            music[MusicType.Siren0] = Raylib.LoadMusicStream("assets/sounds/siren0_full.wav");
            music[MusicType.Siren1] = Raylib.LoadMusicStream("assets/sounds/siren1.wav");
            music[MusicType.Siren2] = Raylib.LoadMusicStream("assets/sounds/siren2_full.wav");
            music[MusicType.Siren3] = Raylib.LoadMusicStream("assets/sounds/siren3_full.wav");
            music[MusicType.Siren4] = Raylib.LoadMusicStream("assets/sounds/siren4_full.wav");
            music[MusicType.Fright] = Raylib.LoadMusicStream("assets/sounds/fright_full.wav");
            music[MusicType.Eyes] = Raylib.LoadMusicStream("assets/sounds/eyes_full.wav");

        }

        public static void UpdateSiren()
        {
            if (isSirenPlaying && currentSiren.HasValue)
                Raylib.UpdateMusicStream(currentSiren.Value);
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
            {
                // Let the extra-life jingle finish on its own instead of getting
                // cut off by the next waka/eat sound a frame or two later.
                if (currentSoundType == SfxType.ExtraLife && type != SfxType.ExtraLife)
                    return;
                Raylib.StopSound(currentSound.Value);
            }

            Sound next = sounds[type];
            Raylib.PlaySound(next);
            currentSound = next;
            currentSoundType = type;
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