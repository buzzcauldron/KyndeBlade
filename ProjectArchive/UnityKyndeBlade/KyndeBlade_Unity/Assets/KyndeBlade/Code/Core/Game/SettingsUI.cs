using UnityEngine;
using UnityEngine.UI;

namespace KyndeBlade
{
    /// <summary>
    /// Exposes GameSettings fields to the player: difficulty, timing, volume.
    /// Can be used from MainMenu or as an in-game overlay.
    /// </summary>
    public class SettingsUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public Dropdown DifficultyDropdown;
        public Slider TimingSlider;
        public Text TimingLabel;
        public Slider VolumeSlider;
        public Text VolumeLabel;
        public Slider MusicSlider;
        public Text MusicLabel;
        public Slider SFXSlider;
        public Text SFXLabel;

        [Header("Data")]
        public GameSettings Settings;

        const string PrefDifficulty = "KyndeBlade_Difficulty";
        const string PrefTiming = "KyndeBlade_TimingMult";
        const string PrefVolume = "KyndeBlade_Volume";

        void Start()
        {
            LoadPrefs();
            BindUI();
        }

        void LoadPrefs()
        {
            int diff = PlayerPrefs.GetInt(PrefDifficulty, 1);
            float timing = PlayerPrefs.GetFloat(PrefTiming, 1f);
            float volume = PlayerPrefs.GetFloat(PrefVolume, 1f);

            if (Settings != null)
            {
                Settings.Difficulty = (DifficultyMode)diff;
                Settings.TimingWindowMultiplier = timing;
            }
            AudioListener.volume = volume;

            if (DifficultyDropdown != null) DifficultyDropdown.value = diff;
            if (TimingSlider != null) TimingSlider.value = timing;
            if (VolumeSlider != null) VolumeSlider.value = volume;

            var vm = VolumeManager.Instance;
            if (vm != null)
            {
                if (MusicSlider != null) MusicSlider.value = vm.MusicVolume;
                if (SFXSlider != null) SFXSlider.value = vm.SFXVolume;
            }
            UpdateLabels();
        }

        void BindUI()
        {
            if (DifficultyDropdown != null)
                DifficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
            if (TimingSlider != null)
                TimingSlider.onValueChanged.AddListener(OnTimingChanged);
            if (VolumeSlider != null)
                VolumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            if (MusicSlider != null)
                MusicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            if (SFXSlider != null)
                SFXSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        void OnDifficultyChanged(int value)
        {
            PlayerPrefs.SetInt(PrefDifficulty, value);
            if (Settings != null) Settings.Difficulty = (DifficultyMode)value;
            PlayerPrefs.Save();
        }

        void OnTimingChanged(float value)
        {
            PlayerPrefs.SetFloat(PrefTiming, value);
            if (Settings != null) Settings.TimingWindowMultiplier = value;
            PlayerPrefs.Save();
            UpdateLabels();
        }

        void OnVolumeChanged(float value)
        {
            var vm = VolumeManager.Instance;
            if (vm != null) vm.SetMaster(value);
            else { AudioListener.volume = value; }
            PlayerPrefs.SetFloat(PrefVolume, value);
            PlayerPrefs.Save();
            UpdateLabels();
        }

        void OnMusicVolumeChanged(float value)
        {
            var vm = VolumeManager.Instance;
            if (vm != null) vm.SetMusic(value);
            UpdateLabels();
        }

        void OnSFXVolumeChanged(float value)
        {
            var vm = VolumeManager.Instance;
            if (vm != null) vm.SetSFX(value);
            UpdateLabels();
        }

        void UpdateLabels()
        {
            if (TimingLabel != null)
                TimingLabel.text = $"Timing: {(TimingSlider != null ? TimingSlider.value : 1f):F1}x";
            if (VolumeLabel != null)
                VolumeLabel.text = $"Master: {Mathf.RoundToInt((VolumeSlider != null ? VolumeSlider.value : 1f) * 100)}%";
            if (MusicLabel != null)
                MusicLabel.text = $"Music: {Mathf.RoundToInt((MusicSlider != null ? MusicSlider.value : 1f) * 100)}%";
            if (SFXLabel != null)
                SFXLabel.text = $"SFX: {Mathf.RoundToInt((SFXSlider != null ? SFXSlider.value : 1f) * 100)}%";
        }

        /// <summary>Build default settings UI elements under a parent transform.</summary>
        public void BuildDefaultUI(Transform parent)
        {
            float y = 0.72f;
            var diffLabel = CreateLabel(parent, "Difficulty", new Vector2(0.1f, y), new Vector2(0.35f, y + 0.06f));
            ManuscriptUITheme.ApplyToText(diffLabel);
            DifficultyDropdown = CreateDropdown(parent, new[] { "Easy", "Normal", "Hard", "Expert" },
                new Vector2(0.4f, y), new Vector2(0.9f, y + 0.06f));
            DifficultyDropdown.value = 1;

            y -= 0.12f;
            TimingLabel = CreateLabel(parent, "Timing: 1.0x", new Vector2(0.1f, y), new Vector2(0.35f, y + 0.06f));
            ManuscriptUITheme.ApplyToText(TimingLabel);
            TimingSlider = CreateSlider(parent, 0.5f, 3f, 1f,
                new Vector2(0.4f, y), new Vector2(0.9f, y + 0.06f));

            y -= 0.12f;
            VolumeLabel = CreateLabel(parent, "Master: 100%", new Vector2(0.1f, y), new Vector2(0.35f, y + 0.06f));
            ManuscriptUITheme.ApplyToText(VolumeLabel);
            VolumeSlider = CreateSlider(parent, 0f, 1f, 1f,
                new Vector2(0.4f, y), new Vector2(0.9f, y + 0.06f));

            y -= 0.12f;
            MusicLabel = CreateLabel(parent, "Music: 100%", new Vector2(0.1f, y), new Vector2(0.35f, y + 0.06f));
            ManuscriptUITheme.ApplyToText(MusicLabel);
            MusicSlider = CreateSlider(parent, 0f, 1f, 1f,
                new Vector2(0.4f, y), new Vector2(0.9f, y + 0.06f));

            y -= 0.12f;
            SFXLabel = CreateLabel(parent, "SFX: 100%", new Vector2(0.1f, y), new Vector2(0.35f, y + 0.06f));
            ManuscriptUITheme.ApplyToText(SFXLabel);
            SFXSlider = CreateSlider(parent, 0f, 1f, 1f,
                new Vector2(0.4f, y), new Vector2(0.9f, y + 0.06f));

            y -= 0.1f;
            var displayHeader = CreateLabel(parent, "— Display —", new Vector2(0.1f, y), new Vector2(0.9f, y + 0.05f));
            ManuscriptUITheme.ApplyToText(displayHeader, emphasis: true);

            y -= 0.08f;
            CreateToggle(parent, "Fullscreen", new Vector2(0.1f, y), new Vector2(0.9f, y + 0.05f),
                Screen.fullScreen, v => AccessibilityManager.Instance?.SetFullscreen(v));

            y -= 0.1f;
            var accessHeader = CreateLabel(parent, "— Accessibility —", new Vector2(0.1f, y), new Vector2(0.9f, y + 0.05f));
            ManuscriptUITheme.ApplyToText(accessHeader, emphasis: true);

            y -= 0.08f;
            var am = AccessibilityManager.Instance;
            CreateToggle(parent, "High Contrast", new Vector2(0.1f, y), new Vector2(0.9f, y + 0.05f),
                am != null && am.HighContrastMode, v => AccessibilityManager.Instance?.SetHighContrast(v));

            y -= 0.08f;
            CreateToggle(parent, "Large Text", new Vector2(0.1f, y), new Vector2(0.9f, y + 0.05f),
                am != null && am.LargeTextMode, v => AccessibilityManager.Instance?.SetLargeText(v));

            y -= 0.08f;
            CreateToggle(parent, "Timing Assist (wider windows)", new Vector2(0.1f, y), new Vector2(0.9f, y + 0.05f),
                am != null && am.TimingAssistMode, v => AccessibilityManager.Instance?.SetTimingAssist(v));
        }

        static void CreateToggle(Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax,
            bool initialValue, System.Action<bool> onChange)
        {
            var go = new GameObject("Toggle_" + label.Replace(" ", ""));
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = rect.offsetMax = Vector2.zero;

            var toggle = go.AddComponent<Toggle>();
            toggle.isOn = initialValue;

            var bgGo = new GameObject("Background");
            bgGo.transform.SetParent(go.transform, false);
            var bgRect = bgGo.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0, 0.1f);
            bgRect.anchorMax = new Vector2(0.08f, 0.9f);
            bgRect.offsetMin = bgRect.offsetMax = Vector2.zero;
            var bgImg = bgGo.AddComponent<Image>();
            bgImg.color = ManuscriptUITheme.ParchmentDark;

            var checkGo = new GameObject("Checkmark");
            checkGo.transform.SetParent(bgGo.transform, false);
            var checkRect = checkGo.AddComponent<RectTransform>();
            checkRect.anchorMin = new Vector2(0.1f, 0.1f);
            checkRect.anchorMax = new Vector2(0.9f, 0.9f);
            checkRect.offsetMin = checkRect.offsetMax = Vector2.zero;
            var checkImg = checkGo.AddComponent<Image>();
            checkImg.color = ManuscriptUITheme.Gold;

            toggle.targetGraphic = bgImg;
            toggle.graphic = checkImg;

            var textGo = new GameObject("Label");
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.1f, 0);
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = textRect.offsetMax = Vector2.zero;
            var t = textGo.AddComponent<Text>();
            t.text = label;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = 16;
            t.alignment = TextAnchor.MiddleLeft;
            ManuscriptUITheme.ApplyToText(t);

            toggle.onValueChanged.AddListener(v => onChange?.Invoke(v));
        }

        static Text CreateLabel(Transform parent, string text, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject("Label");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            var t = go.AddComponent<Text>();
            t.text = text;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = 18;
            t.alignment = TextAnchor.MiddleLeft;
            return t;
        }

        static Dropdown CreateDropdown(Transform parent, string[] options, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject("Dropdown");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = ManuscriptUITheme.ParchmentAged;

            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var labelRect = labelGo.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(10, 0);
            labelRect.offsetMax = new Vector2(-25, 0);
            var label = labelGo.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 16;
            label.alignment = TextAnchor.MiddleLeft;
            ManuscriptUITheme.ApplyToText(label);

            var dd = go.AddComponent<Dropdown>();
            dd.captionText = label;
            dd.ClearOptions();
            dd.AddOptions(new System.Collections.Generic.List<string>(options));

            return dd;
        }

        static Slider CreateSlider(Transform parent, float min, float max, float value,
            Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject("Slider");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = rect.offsetMax = Vector2.zero;

            var bgGo = new GameObject("Background");
            bgGo.transform.SetParent(go.transform, false);
            var bgRect = bgGo.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0, 0.35f);
            bgRect.anchorMax = new Vector2(1, 0.65f);
            bgRect.offsetMin = bgRect.offsetMax = Vector2.zero;
            var bgImg = bgGo.AddComponent<Image>();
            bgImg.color = ManuscriptUITheme.ParchmentDark;

            var fillArea = new GameObject("FillArea");
            fillArea.transform.SetParent(go.transform, false);
            var fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0.35f);
            fillAreaRect.anchorMax = new Vector2(1, 0.65f);
            fillAreaRect.offsetMin = fillAreaRect.offsetMax = Vector2.zero;

            var fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            var fillRect = fill.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = fillRect.offsetMax = Vector2.zero;
            var fillImg = fill.AddComponent<Image>();
            fillImg.color = ManuscriptUITheme.Gold;

            var handleArea = new GameObject("HandleSlideArea");
            handleArea.transform.SetParent(go.transform, false);
            var handleAreaRect = handleArea.AddComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = handleAreaRect.offsetMax = Vector2.zero;

            var handle = new GameObject("Handle");
            handle.transform.SetParent(handleArea.transform, false);
            var handleRect = handle.AddComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 0);
            var handleImg = handle.AddComponent<Image>();
            handleImg.color = ManuscriptUITheme.InkSecondary;

            var slider = go.AddComponent<Slider>();
            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = value;

            return slider;
        }
    }
}
