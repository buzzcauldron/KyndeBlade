using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KyndeBlade
{
    /// <summary>
    /// Manages the main menu: New Game, Continue, Settings, Credits.
    /// Lives in the MainMenu scene (build index 0).
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Panels")]
        public GameObject MainPanel;
        public GameObject SettingsPanel;
        public GameObject CreditsPanel;

        [Header("Buttons")]
        public Button NewGameButton;
        public Button ContinueButton;
        public Button SettingsButton;
        public Button CreditsButton;
        public Button BackFromSettingsButton;
        public Button BackFromCreditsButton;

        [Header("Scene")]
        [Tooltip("Name of the gameplay scene to load.")]
        public string GameSceneName = "Main";

        void Start()
        {
            if (MainPanel == null) BuildUI();
            WireButtons();
            ShowMain();
            UpdateContinueAvailability();
        }

        void WireButtons()
        {
            if (NewGameButton != null)   NewGameButton.onClick.AddListener(OnNewGame);
            if (ContinueButton != null)  ContinueButton.onClick.AddListener(OnContinue);
            if (SettingsButton != null)   SettingsButton.onClick.AddListener(OnSettings);
            if (CreditsButton != null)    CreditsButton.onClick.AddListener(OnCredits);
            if (BackFromSettingsButton != null) BackFromSettingsButton.onClick.AddListener(ShowMain);
            if (BackFromCreditsButton != null)  BackFromCreditsButton.onClick.AddListener(ShowMain);
        }

        void UpdateContinueAvailability()
        {
            if (ContinueButton == null) return;
            var save = FindFirstObjectByType<SaveManager>();
            bool hasSave = save != null && save.SlotExists(save.ActiveSlot);
            if (!hasSave)
            {
                string path = System.IO.Path.Combine(Application.persistentDataPath, "saves", "slot_0.json");
                hasSave = System.IO.File.Exists(path);
            }
            ContinueButton.interactable = hasSave;
        }

        void OnNewGame()
        {
            var save = FindFirstObjectByType<SaveManager>();
            if (save != null)
                save.NewGame("malvern");
            SceneManager.LoadScene(GameSceneName);
        }

        void OnContinue()
        {
            SceneManager.LoadScene(GameSceneName);
        }

        void OnSettings()
        {
            if (MainPanel != null) MainPanel.SetActive(false);
            if (SettingsPanel != null) SettingsPanel.SetActive(true);
            if (CreditsPanel != null) CreditsPanel.SetActive(false);
        }

        void OnCredits()
        {
            if (MainPanel != null) MainPanel.SetActive(false);
            if (SettingsPanel != null) SettingsPanel.SetActive(false);
            if (CreditsPanel != null) CreditsPanel.SetActive(true);
        }

        void ShowMain()
        {
            if (MainPanel != null) MainPanel.SetActive(true);
            if (SettingsPanel != null) SettingsPanel.SetActive(false);
            if (CreditsPanel != null) CreditsPanel.SetActive(false);
        }

        /// <summary>Called from gameplay to return to menu.</summary>
        public static void ReturnToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        // --- Auto-build UI if no panels assigned ---

        void BuildUI()
        {
            var canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var scaler = gameObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;
                gameObject.AddComponent<GraphicRaycaster>();
            }

            var bg = CreatePanel(transform, "Background", Vector2.zero, Vector2.one);
            bg.GetComponent<Image>().color = ManuscriptUITheme.ParchmentLight;

            MainPanel = CreatePanel(transform, "MainPanel", new Vector2(0.3f, 0.15f), new Vector2(0.7f, 0.85f));

            var title = CreateText(MainPanel.transform, "Kynde Blade", 48,
                new Vector2(0, 0.75f), new Vector2(1, 0.95f));
            ManuscriptUITheme.ApplyToText(title, emphasis: true);

            var subtitle = CreateText(MainPanel.transform,
                "A 16-bit RPG inspired by Piers Plowman", 18,
                new Vector2(0.1f, 0.65f), new Vector2(0.9f, 0.75f));
            ManuscriptUITheme.ApplyToText(subtitle, lapis: true);

            NewGameButton = CreateMenuButton(MainPanel.transform, "New Game", 0.55f);
            ContinueButton = CreateMenuButton(MainPanel.transform, "Continue", 0.43f);
            SettingsButton = CreateMenuButton(MainPanel.transform, "Settings", 0.31f);
            CreditsButton = CreateMenuButton(MainPanel.transform, "Credits", 0.19f);

            SettingsPanel = CreatePanel(transform, "SettingsPanel", new Vector2(0.2f, 0.1f), new Vector2(0.8f, 0.9f));
            SettingsPanel.GetComponent<Image>().color = ManuscriptUITheme.Parchment;
            var settingsTitle = CreateText(SettingsPanel.transform, "Settings", 36,
                new Vector2(0, 0.85f), new Vector2(1, 0.95f));
            ManuscriptUITheme.ApplyToText(settingsTitle, emphasis: true);
            BackFromSettingsButton = CreateMenuButton(SettingsPanel.transform, "Back", 0.05f);
            var settingsUI = SettingsPanel.AddComponent<SettingsUI>();
            settingsUI.BuildDefaultUI(SettingsPanel.transform);

            CreditsPanel = CreatePanel(transform, "CreditsPanel", new Vector2(0.2f, 0.1f), new Vector2(0.8f, 0.9f));
            CreditsPanel.GetComponent<Image>().color = ManuscriptUITheme.Parchment;
            var creditsTitle = CreateText(CreditsPanel.transform, "Credits", 36,
                new Vector2(0, 0.85f), new Vector2(1, 0.95f));
            ManuscriptUITheme.ApplyToText(creditsTitle, emphasis: true);
            var creditsBody = CreateText(CreditsPanel.transform,
                "Inspired by Clair Obscur: Expedition 33\nNamed after \"Kynde\" from William Langland's Piers Plowman\n\nBuilt in Unity", 18,
                new Vector2(0.1f, 0.2f), new Vector2(0.9f, 0.8f));
            ManuscriptUITheme.ApplyToText(creditsBody);
            BackFromCreditsButton = CreateMenuButton(CreditsPanel.transform, "Back", 0.05f);
        }

        static GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0);
            return go;
        }

        static Text CreateText(Transform parent, string content, int fontSize, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject("Text");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            var t = go.AddComponent<Text>();
            t.text = content;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = fontSize;
            t.alignment = TextAnchor.MiddleCenter;
            ManuscriptUITheme.ApplyToText(t);
            return t;
        }

        static Button CreateMenuButton(Transform parent, string label, float yCenter)
        {
            var go = new GameObject("Btn_" + label.Replace(" ", ""));
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.2f, yCenter - 0.04f);
            rect.anchorMax = new Vector2(0.8f, yCenter + 0.04f);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = ManuscriptUITheme.ParchmentAged;
            var btn = go.AddComponent<Button>();
            ManuscriptUITheme.ApplyToButton(btn);
            var textGo = new GameObject("Text");
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = textRect.offsetMax = Vector2.zero;
            var t = textGo.AddComponent<Text>();
            t.text = label;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = 24;
            t.alignment = TextAnchor.MiddleCenter;
            ManuscriptUITheme.ApplyToText(t);
            return btn;
        }
    }
}
