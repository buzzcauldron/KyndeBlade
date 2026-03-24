using UnityEngine;
using UnityEngine.UI;

namespace KyndeBlade
{
    /// <summary>Eye indicator for parry/dodge zone: pupil contracts and eyelid narrows as strike approaches.</summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class ParryDodgeZoneIndicator : MonoBehaviour
    {
        [Header("References")]
        public TurnManager TurnManager;
        public ParryDodgeZoneSoundBank SoundBank;

        [Header("Eye Elements (auto-created if null)")]
        public RectTransform EyeRoot;
        public Image ScleraImage;      // White of eye
        public RectTransform PupilRoot;
        public Image PupilImage;
        public RectTransform EyelidTop;
        public RectTransform EyelidBottom;

        [Header("Animation (phased: open → steady → strike imminent)")]
        [Range(0.2f, 1f)]
        public float PupilScaleAtStart = 1f;
        [Range(0.1f, 0.5f)]
        public float PupilScaleAtStrike = 0.25f;
        [Range(0f, 0.5f)]
        public float EyelidCloseAmount = 0.35f;
        [Tooltip("First 15% of window: eye open, pupil at start scale.")]
        public float OpenPhaseEnd = 0.15f;
        [Tooltip("Last 25% of window: strike imminent, pupil contracts, eyelids close.")]
        public float ImminentPhaseStart = 0.75f;

        CanvasGroup _canvasGroup;
        bool _soundPlayed;
        bool _imminentSoundPlayed;
        bool _isVisible;

        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        void Start()
        {
            if (TurnManager == null) TurnManager = GameRuntime.TurnManager ?? UnityEngine.Object.FindFirstObjectByType<TurnManager>();
            if (SoundBank == null) SoundBank = UnityEngine.Object.FindFirstObjectByType<ParryDodgeZoneSoundBank>();
            if (TurnManager != null)
                TurnManager.OnCombatEnded += OnCombatEnded;
            EnsureEyeElements();
            Hide();
        }

        void Update()
        {
            if (TurnManager == null) return;

            if (TurnManager.State != CombatState.RealTimeWindow)
            {
                if (_isVisible) Hide();
                return;
            }

            if (!_isVisible) Show();
            _isVisible = true;

            float duration = TurnManager.RealTimeWindowDuration;
            float remaining = TurnManager.RealTimeWindowRemaining;
            float t = 1f - Mathf.Clamp01(remaining / (duration > 0.001f ? duration : 0.001f));

            UpdateEyeAnimationPhased(t);

            if (!_soundPlayed && SoundBank != null)
            {
                SoundBank.PlayStrikeWarning(TurnManager.CurrentAttackerDuringWindow);
                _soundPlayed = true;
            }
            if (SoundBank != null && remaining <= SoundBank.ImminentThresholdSeconds && !_imminentSoundPlayed)
            {
                SoundBank.PlayImminent();
                _imminentSoundPlayed = true;
            }
        }

        void OnEnable()
        {
            if (TurnManager != null)
            {
                TurnManager.OnCombatEnded += OnCombatEnded;
            }
        }

        void OnDisable()
        {
            if (TurnManager != null)
                TurnManager.OnCombatEnded -= OnCombatEnded;
        }

        void OnDestroy()
        {
            if (TurnManager != null)
                TurnManager.OnCombatEnded -= OnCombatEnded;
        }

        void OnCombatEnded()
        {
            Hide();
        }

        public void Show()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.interactable = false;
            }
            if (EyeRoot != null) EyeRoot.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _soundPlayed = false;
            _imminentSoundPlayed = false;
            _isVisible = false;
            if (_canvasGroup != null) _canvasGroup.alpha = 0f;
            if (EyeRoot != null) EyeRoot.gameObject.SetActive(false);
        }

        /// <summary>Phase-based: open (0–15%) → steady → strike imminent (last 25%). Skill-focused: clear “input now” moment.</summary>
        void UpdateEyeAnimationPhased(float t)
        {
            float pupilScale = PupilScaleAtStart;
            float closeNorm = 0f;
            if (t >= ImminentPhaseStart)
            {
                float imminentT = (t - ImminentPhaseStart) / (1f - ImminentPhaseStart);
                imminentT = Mathf.SmoothStep(0f, 1f, imminentT);
                pupilScale = Mathf.Lerp(PupilScaleAtStart, PupilScaleAtStrike, imminentT);
                closeNorm = imminentT;
            }
            float close = closeNorm * EyelidCloseAmount;

            if (PupilRoot != null)
                PupilRoot.localScale = Vector3.one * pupilScale;
            if (EyelidTop != null)
            {
                EyelidTop.anchorMin = new Vector2(0f, 1f - close);
                EyelidTop.anchorMax = new Vector2(1f, 1f);
                EyelidTop.offsetMin = EyelidTop.offsetMax = Vector2.zero;
            }
            if (EyelidBottom != null)
            {
                EyelidBottom.anchorMin = new Vector2(0f, 0f);
                EyelidBottom.anchorMax = new Vector2(1f, close);
                EyelidBottom.offsetMin = EyelidBottom.offsetMax = Vector2.zero;
            }
        }

        void EnsureEyeElements()
        {
            if (EyeRoot != null) return;

            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null) return;

            var go = new GameObject("ParryDodgeEye");
            go.transform.SetParent(transform, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(120, 60);
            rect.anchoredPosition = Vector2.zero;
            EyeRoot = rect;

            var sclera = new GameObject("Sclera");
            sclera.transform.SetParent(rect, false);
            var scleraRect = sclera.AddComponent<RectTransform>();
            scleraRect.anchorMin = Vector2.zero;
            scleraRect.anchorMax = Vector2.one;
            scleraRect.offsetMin = Vector2.zero;
            scleraRect.offsetMax = Vector2.zero;
            var scleraImg = sclera.AddComponent<Image>();
            scleraImg.color = ManuscriptUITheme.ParchmentLight;
            ScleraImage = scleraImg;

            var pupil = new GameObject("Pupil");
            pupil.transform.SetParent(rect, false);
            var pupilRect = pupil.AddComponent<RectTransform>();
            pupilRect.anchorMin = new Vector2(0.5f, 0.5f);
            pupilRect.anchorMax = new Vector2(0.5f, 0.5f);
            pupilRect.sizeDelta = new Vector2(24, 24);
            pupilRect.anchoredPosition = Vector2.zero;
            PupilRoot = pupilRect;
            var pupilImg = pupil.AddComponent<Image>();
            pupilImg.color = ManuscriptUITheme.InkPrimary;
            PupilImage = pupilImg;

            var lidTop = new GameObject("EyelidTop");
            lidTop.transform.SetParent(rect, false);
            var lidTopRect = lidTop.AddComponent<RectTransform>();
            lidTopRect.anchorMin = new Vector2(0f, 1f);
            lidTopRect.anchorMax = new Vector2(1f, 1f);
            lidTopRect.pivot = new Vector2(0.5f, 1f);
            lidTopRect.sizeDelta = new Vector2(0, 20);
            lidTopRect.anchoredPosition = Vector2.zero;
            var lidTopImg = lidTop.AddComponent<Image>();
            lidTopImg.color = new Color(ManuscriptUITheme.BorderDark.r, ManuscriptUITheme.BorderDark.g, ManuscriptUITheme.BorderDark.b, 0.9f);
            EyelidTop = lidTopRect;

            var lidBot = new GameObject("EyelidBottom");
            lidBot.transform.SetParent(rect, false);
            var lidBotRect = lidBot.AddComponent<RectTransform>();
            lidBotRect.anchorMin = new Vector2(0f, 0f);
            lidBotRect.anchorMax = new Vector2(1f, 0f);
            lidBotRect.pivot = new Vector2(0.5f, 0f);
            lidBotRect.sizeDelta = new Vector2(0, 20);
            lidBotRect.anchoredPosition = Vector2.zero;
            var lidBotImg = lidBot.AddComponent<Image>();
            lidBotImg.color = new Color(ManuscriptUITheme.BorderDark.r, ManuscriptUITheme.BorderDark.g, ManuscriptUITheme.BorderDark.b, 0.9f);
            EyelidBottom = lidBotRect;
        }
    }
}
