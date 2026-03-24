using UnityEngine;

namespace KyndeBlade
{
    /// <summary>PlayerPrefs-backed settings (master volume, fullscreen). Keys documented in ARCHITECTURE.md.</summary>
    public static class KyndeBladeSettingsStore
    {
        public const string PrefMasterVolume = "KyndeBlade_Settings_MasterVolume";
        public const string PrefFullscreen = "KyndeBlade_Settings_Fullscreen";

        const float DefaultMasterVolume = 1f;

        /// <summary>Linear master volume 0–1 (applied via AudioListener).</summary>
        public static float MasterVolume
        {
            get => PlayerPrefs.GetFloat(PrefMasterVolume, DefaultMasterVolume);
            set
            {
                PlayerPrefs.SetFloat(PrefMasterVolume, Mathf.Clamp01(value));
                PlayerPrefs.Save();
                ApplyAudioListenerVolume();
            }
        }

        public static bool Fullscreen
        {
            get => PlayerPrefs.GetInt(PrefFullscreen, Screen.fullScreen ? 1 : 0) != 0;
            set
            {
                PlayerPrefs.SetInt(PrefFullscreen, value ? 1 : 0);
                PlayerPrefs.Save();
                Screen.fullScreen = value;
            }
        }

        /// <summary>Load prefs and apply to engine (call on boot).</summary>
        public static void ApplyAllFromStorage()
        {
            ApplyAudioListenerVolume();
            Screen.fullScreen = Fullscreen;
        }

        public static void ApplyAudioListenerVolume()
        {
            AudioListener.volume = MasterVolume;
        }
    }
}
