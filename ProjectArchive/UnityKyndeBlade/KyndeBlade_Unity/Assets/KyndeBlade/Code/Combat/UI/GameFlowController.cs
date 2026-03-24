using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace KyndeBlade
{
    /// <summary>Main menu, pause, and settings overlay. Defers <see cref="WorldMapManager"/> lazy init until the player continues or starts a new game.</summary>
    [DefaultExecutionOrder(-100)]
    [RequireComponent(typeof(Canvas))]
    public class GameFlowController : MonoBehaviour
    {
        /// <summary>When true (e.g. PlayMode tests), skip the main menu and allow world map init immediately.</summary>
        public static bool SkipMainMenuForAutomatedTests { get; set; }

        [Header("Optional refs (auto-resolve if null)")]
        public SaveManager SaveManager;
        public WorldMapManager WorldMap;
        public Canvas MenuCanvas;

        /// <summary>While true, <see cref="WorldMapManager"/> will not run initial map setup.</summary>
        public bool BlocksWorldInit { get; private set; } = true;

        RectTransform _mainMenuRoot;
        RectTransform _pauseRoot;
        RectTransform _settingsRoot;
        Button _continueButton;
        Scrollbar _volumeScrollbar;
        Toggle _fullscreenToggle;
        bool _isPaused;
        bool _playing;
        static Font _font;

        static Font MenuFont => _font ?? (_font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));

        void Awake()
        {
            if (SkipMainMenuForAutomatedTests)
                BlocksWorldInit = false;
            MenuCanvas = MenuCanvas != null ? MenuCanvas : GetComponent<Canvas>();
            if (MenuCanvas != null) MenuCanvas.sortingOrder = 300;
        }

        void Start()
        {
            ResolveRefs();
            KyndeBladeSettingsStore.ApplyAllFromStorage();
            BuildUi();
            if (SkipMainMenuForAutomatedTests)
            {
                _playing = true;
                if (_mainMenuRoot != null) _mainMenuRoot.gameObject.SetActive(false);
                BlocksWorldInit = false;
                HideOtherCanvasesForMenu(true);
                return;
            }

            BlocksWorldInit = true;
            _playing = false;
            ShowMainMenu();
        }

        void Update()
        {
            if (!_playing || _isPaused) return;
            if (Input.GetKeyDown(KeyCode.Escape))
                OpenPause();
        }

        void ResolveRefs()
        {
            if (SaveManager == null) SaveManager = FindFirstObjectByType<SaveManager>();
            if (WorldMap == null) WorldMap = FindFirstObjectByType<WorldMapManager>();
        }

        void BuildUi()
        {
            if (_mainMenuRoot != null) return;

            var canvasTx = transform as RectTransform;
            if (canvasTx == null) return;

            _mainMenuRoot = CreateFullScreenPanel(canvasTx, "MainMenuRoot");
            var title = CreateText(_mainMenuRoot, "Title", "Kynde Blade", 36, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.72f), new Vector2(0.5f, 0.82f));
            ManuscriptUITheme.ApplyToText(title, emphasis: true);

            _continueButton = CreateMenuButton(_mainMenuRoot, "ContinueButton", "Continue", OnContinueClicked, new Vector2(0.35f, 0.52f), new Vector2(0.65f, 0.58f));
            CreateMenuButton(_mainMenuRoot, "NewGameButton", "New Game", OnNewGameClicked, new Vector2(0.35f, 0.44f), new Vector2(0.65f, 0.5f));
            CreateMenuButton(_mainMenuRoot, "SettingsButton", "Settings", OnOpenSettingsFromMenu, new Vector2(0.35f, 0.36f), new Vector2(0.65f, 0.42f));
            CreateMenuButton(_mainMenuRoot, "QuitButton", "Quit", OnQuitClicked, new Vector2(0.35f, 0.28f), new Vector2(0.65f, 0.34f));

            _pauseRoot = CreateFullScreenPanel(canvasTx, "PauseRoot");
            _pauseRoot.gameObject.SetActive(false);
            var pauseBg = _pauseRoot.GetComponent<Image>();
            if (pauseBg != null) pauseBg.color = new Color(0f, 0f, 0f, 0.55f);
            CreateText(_pauseRoot, "PausedTitle", "Paused", 28, TextAnchor.MiddleCenter, new Vector2(0.3f, 0.58f), new Vector2(0.7f, 0.66f));
            CreateMenuButton(_pauseRoot, "ResumeButton", "Resume", ClosePause, new Vector2(0.35f, 0.46f), new Vector2(0.65f, 0.52f));
            CreateMenuButton(_pauseRoot, "PauseSettingsButton", "Settings", OnOpenSettingsFromPause, new Vector2(0.35f, 0.38f), new Vector2(0.65f, 0.44f));
            CreateMenuButton(_pauseRoot, "MainMenuButton", "Main Menu", OnReturnToMainMenu, new Vector2(0.35f, 0.3f), new Vector2(0.65f, 0.36f));

            _settingsRoot = CreateFullScreenPanel(canvasTx, "SettingsRoot");
            _settingsRoot.gameObject.SetActive(false);
            var setBg = _settingsRoot.GetComponent<Image>();
            if (setBg != null) setBg.color = new Color(0.08f, 0.07f, 0.1f, 0.92f);
            CreateText(_settingsRoot, "SettingsTitle", "Settings", 28, TextAnchor.MiddleCenter, new Vector2(0.25f, 0.72f), new Vector2(0.75f, 0.8f));
            CreateText(_settingsRoot, "VolLabel", "Master volume", 18, TextAnchor.MiddleLeft, new Vector2(0.2f, 0.55f), new Vector2(0.45f, 0.6f));
            _volumeScrollbar = CreateVolumeScrollbar(_settingsRoot, "VolumeScrollbar", KyndeBladeSettingsStore.MasterVolume, v => KyndeBladeSettingsStore.MasterVolume = v, new Vector2(0.2f, 0.48f), new Vector2(0.8f, 0.54f));
            CreateText(_settingsRoot, "FsLabel", "Fullscreen", 18, TextAnchor.MiddleLeft, new Vector2(0.2f, 0.38f), new Vector2(0.45f, 0.43f));
            _fullscreenToggle = CreateToggle(_settingsRoot, "FullscreenToggle", KyndeBladeSettingsStore.Fullscreen, v => KyndeBladeSettingsStore.Fullscreen = v, new Vector2(0.5f, 0.38f), new Vector2(0.6f, 0.44f));
            CreateMenuButton(_settingsRoot, "SettingsCloseButton", "Close", CloseSettings, new Vector2(0.35f, 0.2f), new Vector2(0.65f, 0.28f));
        }

        static RectTransform CreateFullScreenPanel(RectTransform parent, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = new Color(ManuscriptUITheme.ParchmentDark.r, ManuscriptUITheme.ParchmentDark.g, ManuscriptUITheme.ParchmentDark.b, 0.94f);
            return rect;
        }

        static Text CreateText(RectTransform parent, string name, string content, int size, TextAnchor align, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var t = go.AddComponent<Text>();
            t.font = MenuFont;
            t.fontSize = size;
            t.alignment = align;
            t.text = content;
            ManuscriptUITheme.ApplyToText(t);
            return t;
        }

        Button CreateMenuButton(RectTransform parent, string name, string label, UnityEngine.Events.UnityAction onClick, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = ManuscriptUITheme.ParchmentAged;
            var btn = go.AddComponent<Button>();
            ManuscriptUITheme.ApplyToButton(btn);
            var textGo = new GameObject("Text");
            textGo.transform.SetParent(go.transform, false);
            var tr = textGo.AddComponent<RectTransform>();
            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;
            tr.offsetMin = Vector2.zero;
            tr.offsetMax = Vector2.zero;
            var tx = textGo.AddComponent<Text>();
            tx.font = MenuFont;
            tx.fontSize = 20;
            tx.alignment = TextAnchor.MiddleCenter;
            tx.text = label;
            ManuscriptUITheme.ApplyToText(tx);
            btn.onClick.AddListener(onClick);
            return btn;
        }

        static Scrollbar CreateVolumeScrollbar(RectTransform parent, string name, float value, System.Action<float> onChanged, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var sb = go.AddComponent<Scrollbar>();
            sb.direction = Scrollbar.Direction.LeftToRight;
            sb.size = 0.15f;
            sb.numberOfSteps = 0;
            sb.value = Mathf.Clamp01(value);
            var bg = new GameObject("Background").AddComponent<Image>();
            bg.transform.SetParent(go.transform, false);
            var bgR = bg.rectTransform;
            bgR.anchorMin = Vector2.zero;
            bgR.anchorMax = Vector2.one;
            bgR.offsetMin = Vector2.zero;
            bgR.offsetMax = Vector2.zero;
            bg.color = ManuscriptUITheme.ParchmentLight;
            var sliding = new GameObject("Sliding Area");
            sliding.transform.SetParent(go.transform, false);
            var saR = sliding.AddComponent<RectTransform>();
            saR.anchorMin = Vector2.zero;
            saR.anchorMax = Vector2.one;
            saR.offsetMin = new Vector2(8, 4);
            saR.offsetMax = new Vector2(-8, -4);
            var handle = new GameObject("Handle").AddComponent<Image>();
            handle.transform.SetParent(sliding.transform, false);
            var hR = handle.rectTransform;
            hR.anchorMin = Vector2.zero;
            hR.anchorMax = Vector2.one;
            hR.pivot = new Vector2(0.5f, 0.5f);
            hR.sizeDelta = Vector2.zero;
            handle.color = ManuscriptUITheme.LapisBlue;
            sb.handleRect = hR;
            sb.targetGraphic = handle;
            sb.onValueChanged.AddListener(v => onChanged?.Invoke(v));
            return sb;
        }

        static Toggle CreateToggle(RectTransform parent, string name, bool isOn, System.Action<bool> onChanged, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var bg = go.AddComponent<Image>();
            bg.color = ManuscriptUITheme.ParchmentAged;
            var toggle = go.AddComponent<Toggle>();
            toggle.targetGraphic = bg;
            var checkGo = new GameObject("Checkmark");
            checkGo.transform.SetParent(go.transform, false);
            var checkRt = checkGo.AddComponent<RectTransform>();
            checkRt.anchorMin = new Vector2(0.1f, 0.1f);
            checkRt.anchorMax = new Vector2(0.9f, 0.9f);
            checkRt.offsetMin = Vector2.zero;
            checkRt.offsetMax = Vector2.zero;
            var checkImg = checkGo.AddComponent<Image>();
            checkImg.color = ManuscriptUITheme.LapisBlue;
            toggle.graphic = checkImg;
            toggle.isOn = isOn;
            checkGo.SetActive(isOn);
            toggle.onValueChanged.AddListener(v =>
            {
                checkGo.SetActive(v);
                onChanged?.Invoke(v);
            });
            return toggle;
        }

        void ShowMainMenu()
        {
            BuildUi();
            _playing = false;
            _isPaused = false;
            BlocksWorldInit = true;
            Time.timeScale = 1f;
            if (_mainMenuRoot != null) _mainMenuRoot.gameObject.SetActive(true);
            if (_pauseRoot != null) _pauseRoot.gameObject.SetActive(false);
            if (_settingsRoot != null) _settingsRoot.gameObject.SetActive(false);
            HideOtherCanvasesForMenu(false);
            RefreshContinueButton();
        }

        void RefreshContinueButton()
        {
            if (_continueButton == null || SaveManager == null) return;
            _continueButton.interactable = SaveManager.HasSavedGame;
            var cg = _continueButton.GetComponent<CanvasGroup>();
            if (cg == null && !SaveManager.HasSavedGame)
            {
                var c = _continueButton.colors;
                c.disabledColor = new Color(0.6f, 0.6f, 0.6f, 0.5f);
                _continueButton.colors = c;
            }
            _continueButton.gameObject.SetActive(true);
        }

        /// <summary>Hide map/combat while main menu shows; or show map when starting play from tests.</summary>
        void HideOtherCanvasesForMenu(bool mapShouldBeVisible)
        {
            var map = GameObject.Find("MapCanvas");
            var combat = GameObject.Find("CombatCanvas");
            var dialogue = GameObject.Find("DialogueCanvas");
            if (map != null) map.SetActive(mapShouldBeVisible);
            if (combat != null) combat.SetActive(false);
            if (dialogue != null) dialogue.SetActive(mapShouldBeVisible);
        }

        void BeginPlayingFromMenu()
        {
            BlocksWorldInit = false;
            _playing = true;
            if (_mainMenuRoot != null) _mainMenuRoot.gameObject.SetActive(false);
            if (_pauseRoot != null) _pauseRoot.gameObject.SetActive(false);
            if (_settingsRoot != null) _settingsRoot.gameObject.SetActive(false);
            HideOtherCanvasesForMenu(true);
            if (WorldMap != null)
                WorldMap.ResetLazyInitializationFromMenu();
        }

        void OnContinueClicked()
        {
            if (SaveManager == null) return;
            SaveManager.Load();
            BeginPlayingFromMenu();
        }

        void OnNewGameClicked()
        {
            if (SaveManager == null) return;
            SaveManager.NewGame(GameWorldConstants.DefaultStartLocationId);
            BeginPlayingFromMenu();
        }

        void OnOpenSettingsFromMenu()
        {
            if (_settingsRoot != null)
            {
                SyncSettingsUi();
                _settingsRoot.gameObject.SetActive(true);
            }
        }

        void OnOpenSettingsFromPause()
        {
            if (_settingsRoot != null)
            {
                SyncSettingsUi();
                _settingsRoot.gameObject.SetActive(true);
            }
        }

        void SyncSettingsUi()
        {
            if (_volumeScrollbar != null) _volumeScrollbar.SetValueWithoutNotify(KyndeBladeSettingsStore.MasterVolume);
            if (_fullscreenToggle != null)
            {
                _fullscreenToggle.SetIsOnWithoutNotify(KyndeBladeSettingsStore.Fullscreen);
                var check = _fullscreenToggle.transform.Find("Checkmark");
                if (check != null) check.gameObject.SetActive(KyndeBladeSettingsStore.Fullscreen);
            }
        }

        void CloseSettings()
        {
            if (_settingsRoot != null) _settingsRoot.gameObject.SetActive(false);
        }

        void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        void OpenPause()
        {
            if (!_playing || BlocksWorldInit) return;
            _isPaused = true;
            Time.timeScale = 0f;
            if (_pauseRoot != null) _pauseRoot.gameObject.SetActive(true);
        }

        void ClosePause()
        {
            _isPaused = false;
            Time.timeScale = 1f;
            if (_pauseRoot != null) _pauseRoot.gameObject.SetActive(false);
        }

        void OnReturnToMainMenu()
        {
            Time.timeScale = 1f;
            _isPaused = false;
            SkipMainMenuForAutomatedTests = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
