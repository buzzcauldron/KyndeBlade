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

        [Header("Animation")]
        [Range(0.2f, 1f)]
        public float PupilScaleAtStart = 1f;
        [Range(0.1f, 0.5f)]
        public float PupilScaleAtStrike = 0.25f;
        [Range(0f, 0.5f)]
        public float EyelidCloseAmount = 0.35f;

        CanvasGroup _canvasGroup;
        bool _soundPlayed;
        bool _isVisible;

        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        void Start()
        {
            if (TurnManager == null) TurnManager = FindObjectOfType<TurnManager>();
            if (SoundBank == null) SoundBank = FindObjectOfType<ParryDodgeZoneSoundBank>();
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

            UpdateEyeAnimation(t);

            if (!_soundPlayed && SoundBank != null)
            {
                SoundBank.PlayStrikeWarning(TurnManager.CurrentAttackerDuringWindow);
                _soundPlayed = true;
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
            _isVisible = false;
            if (_canvasGroup != null) _canvasGroup.alpha = 0f;
            if (EyeRoot != null) EyeRoot.gameObject.SetActive(false);
        }

        void UpdateEyeAnimation(float t)
        {
            if (PupilRoot != null)
                PupilRoot.localScale = Vector3.one * Mathf.Lerp(PupilScaleAtStart, PupilScaleAtStrike, t);

            float close = Mathf.SmoothStep(0f, 1f, t) * EyelidCloseAmount;
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
            scleraImg.color = new Color(0.95f, 0.92f, 0.88f);
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
            pupilImg.color = Color.black;
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
            lidTopImg.color = new Color(0.3f, 0.25f, 0.2f, 0.9f);
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
            lidBotImg.color = new Color(0.3f, 0.25f, 0.2f, 0.9f);
            EyelidBottom = lidBotRect;
        }
    }
}
