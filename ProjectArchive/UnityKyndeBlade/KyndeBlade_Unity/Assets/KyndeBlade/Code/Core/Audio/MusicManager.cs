using System;
using System.Collections;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Plays theme music. Green Knight theme = omen before inescapable encounter. Orfeo theme = Otherworld.</summary>
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }

        /// <summary>Fired whenever <see cref="PlayTheme"/> is invoked with the resolved theme id (before clip lookup). Tests can subscribe without playing audio.</summary>
        public event Action<string> OnMusicThemeChangeRequested;

        /// <summary>When true, <see cref="PlayTheme"/> updates <see cref="ActiveThemeId"/> and fires <see cref="OnMusicThemeChangeRequested"/> but does not start audio coroutines. Use in automated tests.</summary>
        public static bool SuppressAudioOutputForTests { get; set; }

        /// <summary>Last theme id set by <see cref="PlayTheme"/> (including when suppressed).</summary>
        public string ActiveThemeId => _currentThemeId ?? "";

        [Header("Themes (assign in Inspector)")]
        public AudioClip DefaultTheme;
        public AudioClip GreenKnightTheme;
        public AudioClip OrfeoTheme;

        [Header("Settings")]
        public float CrossfadeDuration = 2f;
        public float OmenThemeDuration = 8f;

        [Header("Orfeo — Haunting")]
        [Tooltip("Pitch when Orfeo theme plays (0.9 = lower, more haunting).")]
        [Range(0.7f, 1f)]
        public float OrfeoPitch = 0.92f;
        [Tooltip("Reverb mix for Orfeo (0–1.1). Higher = more distant, ethereal.")]
        [Range(0f, 1.1f)]
        public float OrfeoReverbMix = 0.6f;
        [Tooltip("Lowpass cutoff for Orfeo (Hz). Lower = muffled, distant.")]
        [Range(100f, 22000f)]
        public float OrfeoLowpassCutoff = 4000f;

        AudioSource _sourceA;
        AudioSource _sourceB;
        AudioLowPassFilter _lowpass;
        AudioReverbFilter _reverb;
        string _currentThemeId;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _sourceA = gameObject.AddComponent<AudioSource>();
            _sourceB = gameObject.AddComponent<AudioSource>();
            _sourceA.loop = true;
            _sourceB.loop = true;
            _sourceA.playOnAwake = false;
            _sourceB.playOnAwake = false;

            _lowpass = gameObject.AddComponent<AudioLowPassFilter>();
            _lowpass.cutoffFrequency = 22000f;
            _lowpass.enabled = false;

            _reverb = gameObject.AddComponent<AudioReverbFilter>();
            _reverb.reverbPreset = AudioReverbPreset.Cave;
            _reverb.enabled = false;

            EnsureProceduralThemes();
        }

        void EnsureProceduralThemes()
        {
            var lib = AudioLibrary.LoadFromResources();
            if (DefaultTheme == null) DefaultTheme = lib != null ? lib.DefaultTheme : null;
            if (GreenKnightTheme == null) GreenKnightTheme = lib != null ? lib.GreenKnightTheme : null;
            if (OrfeoTheme == null) OrfeoTheme = lib != null ? lib.OrfeoTheme : null;
            if (DefaultTheme == null) DefaultTheme = ProceduralAudioFactory.AmbientDrone();
            if (GreenKnightTheme == null) GreenKnightTheme = ProceduralAudioFactory.GreenKnightTheme();
            if (OrfeoTheme == null) OrfeoTheme = ProceduralAudioFactory.OrfeoTheme();
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void PlayTheme(string themeId)
        {
            string id = string.IsNullOrEmpty(themeId) ? "default" : themeId;
            OnMusicThemeChangeRequested?.Invoke(id);

            if (SuppressAudioOutputForTests)
            {
                _currentThemeId = id;
                return;
            }

            var clip = GetClipForTheme(themeId);
            if (clip == null) return;
            _currentThemeId = id;
            StartCoroutine(CrossfadeTo(clip, _currentThemeId));
        }

        /// <summary>Play Green Knight theme as omen, then switch to Orfeo theme (before Otherworld).</summary>
        public void PlayOmenThenOrfeo(Action onOmenComplete)
        {
            if (SuppressAudioOutputForTests)
            {
                OnMusicThemeChangeRequested?.Invoke("green_knight");
                OnMusicThemeChangeRequested?.Invoke("orfeo");
                onOmenComplete?.Invoke();
                return;
            }
            StartCoroutine(OmenThenOrfeoRoutine(onOmenComplete));
        }

        IEnumerator OmenThenOrfeoRoutine(Action onOmenComplete)
        {
            if (GreenKnightTheme != null)
            {
                _sourceA.clip = GreenKnightTheme;
                _sourceA.volume = 1f;
                _sourceA.Play();
                _currentThemeId = "green_knight";
                yield return new WaitForSeconds(OmenThemeDuration);
            }
            onOmenComplete?.Invoke();
        }

        float MusicVol
        {
            get
            {
                var vm = VolumeManager.Instance;
                return vm != null ? vm.EffectiveMusicVolume : 1f;
            }
        }

        IEnumerator CrossfadeTo(AudioClip toClip, string themeId)
        {
            var from = _sourceA.isPlaying ? _sourceA : _sourceB;
            var to = _sourceA.isPlaying ? _sourceB : _sourceA;
            float vol = MusicVol;

            bool isOrfeo = IsOrfeoTheme(themeId);
            to.clip = toClip;
            to.volume = 0f;
            to.pitch = isOrfeo ? OrfeoPitch : 1f;
            to.Play();

            _lowpass.enabled = isOrfeo;
            _lowpass.cutoffFrequency = isOrfeo ? OrfeoLowpassCutoff : 22000f;
            _reverb.enabled = isOrfeo;
            _reverb.reverbLevel = isOrfeo ? Mathf.Lerp(-10000f, 1000f, OrfeoReverbMix) : -10000f;

            from.pitch = 1f;

            float t = 0f;
            while (t < CrossfadeDuration)
            {
                t += Time.deltaTime;
                float x = t / CrossfadeDuration;
                from.volume = Mathf.Lerp(vol, 0f, x);
                to.volume = Mathf.Lerp(0f, vol, x);
                yield return null;
            }
            from.Stop();
            from.volume = vol;
            to.volume = vol;
        }

        bool IsOrfeoTheme(string themeId)
        {
            if (string.IsNullOrEmpty(themeId)) return false;
            var id = themeId.ToLowerInvariant();
            return id == "orfeo" || id == "otherworld";
        }

        AudioClip GetClipForTheme(string themeId)
        {
            if (string.IsNullOrEmpty(themeId)) return DefaultTheme;
            var id = themeId.ToLowerInvariant();
            if (id == "green_knight" || id == "greenknight") return GreenKnightTheme;
            if (id == "orfeo" || id == "otherworld") return OrfeoTheme;
            return DefaultTheme;
        }
    }
}
