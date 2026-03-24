using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Manages accessibility settings: high contrast, large text, timing assist.</summary>
    public class AccessibilityManager : MonoBehaviour
    {
        public static AccessibilityManager Instance { get; private set; }

        const string PrefHighContrast = "KyndeBlade_HighContrast";
        const string PrefLargeText = "KyndeBlade_LargeText";
        const string PrefTimingAssist = "KyndeBlade_TimingAssist";
        const string PrefFullscreen = "KyndeBlade_Fullscreen";

        public bool HighContrastMode { get; private set; }
        public bool LargeTextMode { get; private set; }
        public bool TimingAssistMode { get; private set; }

        public float TextScaleMultiplier => LargeTextMode ? 1.4f : 1f;
        public float TimingWindowMultiplier => TimingAssistMode ? 1.5f : 1f;

        public System.Action OnSettingsChanged;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void Load()
        {
            HighContrastMode = PlayerPrefs.GetInt(PrefHighContrast, 0) == 1;
            LargeTextMode = PlayerPrefs.GetInt(PrefLargeText, 0) == 1;
            TimingAssistMode = PlayerPrefs.GetInt(PrefTimingAssist, 0) == 1;
            bool fullscreen = PlayerPrefs.GetInt(PrefFullscreen, Screen.fullScreen ? 1 : 0) == 1;
            if (Screen.fullScreen != fullscreen)
                Screen.fullScreen = fullscreen;
        }

        public void SetHighContrast(bool value)
        {
            HighContrastMode = value;
            PlayerPrefs.SetInt(PrefHighContrast, value ? 1 : 0);
            PlayerPrefs.Save();
            OnSettingsChanged?.Invoke();
        }

        public void SetLargeText(bool value)
        {
            LargeTextMode = value;
            PlayerPrefs.SetInt(PrefLargeText, value ? 1 : 0);
            PlayerPrefs.Save();
            OnSettingsChanged?.Invoke();
        }

        public void SetTimingAssist(bool value)
        {
            TimingAssistMode = value;
            PlayerPrefs.SetInt(PrefTimingAssist, value ? 1 : 0);
            PlayerPrefs.Save();
            OnSettingsChanged?.Invoke();
        }

        public void SetFullscreen(bool value)
        {
            Screen.fullScreen = value;
            PlayerPrefs.SetInt(PrefFullscreen, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void SetResolution(int width, int height)
        {
            Screen.SetResolution(width, height, Screen.fullScreen);
        }

        /// <summary>Returns high-contrast override colors when enabled.</summary>
        public Color GetTextColor() => HighContrastMode ? Color.white : ManuscriptUITheme.InkPrimary;

        public Color GetBackgroundColor() => HighContrastMode
            ? new Color(0.05f, 0.05f, 0.05f, 1f)
            : ManuscriptUITheme.ParchmentLight;
    }
}
