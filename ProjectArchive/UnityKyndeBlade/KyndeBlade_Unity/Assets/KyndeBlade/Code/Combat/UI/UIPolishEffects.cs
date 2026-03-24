using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace KyndeBlade
{
    /// <summary>Button hover/press feedback with scale and color shift.</summary>
    public class ButtonFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Scale")]
        public float HoverScale = 1.08f;
        public float PressScale = 0.95f;
        public float ScaleSpeed = 12f;

        [Header("Color")]
        public Color HoverTint = new Color(1f, 0.95f, 0.8f, 1f);

        Vector3 _originalScale;
        float _targetScale = 1f;
        Image _image;
        Color _originalColor;

        void Awake()
        {
            _originalScale = transform.localScale;
            _image = GetComponent<Image>();
            if (_image != null) _originalColor = _image.color;
        }

        void Update()
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _originalScale * _targetScale, Time.unscaledDeltaTime * ScaleSpeed);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _targetScale = HoverScale;
            if (_image != null) _image.color = HoverTint;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _targetScale = 1f;
            if (_image != null) _image.color = _originalColor;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _targetScale = PressScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _targetScale = HoverScale;
        }
    }

    /// <summary>Animates a UI element sliding in from off-screen.</summary>
    public class SlideInAnimation : MonoBehaviour
    {
        public enum Direction { Left, Right, Top, Bottom }

        public Direction SlideFrom = Direction.Left;
        public float Duration = 0.4f;
        public float OffscreenOffset = 300f;

        RectTransform _rect;
        Vector2 _targetPos;
        Vector2 _startPos;
        float _timer;
        bool _animating;

        void OnEnable()
        {
            _rect = GetComponent<RectTransform>();
            if (_rect == null) return;
            _targetPos = _rect.anchoredPosition;
            _startPos = _targetPos + GetOffset();
            _rect.anchoredPosition = _startPos;
            _timer = 0f;
            _animating = true;
        }

        Vector2 GetOffset() => SlideFrom switch
        {
            Direction.Left => new Vector2(-OffscreenOffset, 0),
            Direction.Right => new Vector2(OffscreenOffset, 0),
            Direction.Top => new Vector2(0, OffscreenOffset),
            Direction.Bottom => new Vector2(0, -OffscreenOffset),
            _ => Vector2.zero
        };

        void Update()
        {
            if (!_animating || _rect == null) return;
            _timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(_timer / Duration);
            float ease = 1f - Mathf.Pow(1f - t, 3f);
            _rect.anchoredPosition = Vector2.Lerp(_startPos, _targetPos, ease);
            if (t >= 1f) _animating = false;
        }
    }

    /// <summary>Card reveal animation with scale-up from zero.</summary>
    public class CardRevealAnimation : MonoBehaviour
    {
        public float Duration = 0.3f;
        public float Delay;

        Vector3 _targetScale;
        float _timer;
        bool _animating;

        void OnEnable()
        {
            _targetScale = transform.localScale;
            transform.localScale = Vector3.zero;
            _timer = -Delay;
            _animating = true;
        }

        void Update()
        {
            if (!_animating) return;
            _timer += Time.unscaledDeltaTime;
            if (_timer < 0f) return;
            float t = Mathf.Clamp01(_timer / Duration);
            float ease = 1f - Mathf.Pow(1f - t, 2f);
            transform.localScale = Vector3.Lerp(Vector3.zero, _targetScale, ease);
            if (t >= 1f) _animating = false;
        }
    }

    /// <summary>Turn order indicator that slides and fades.</summary>
    public class TurnOrderIndicator : MonoBehaviour
    {
        public float FadeInDuration = 0.2f;
        public float FadeOutDuration = 0.15f;

        CanvasGroup _group;

        void Awake()
        {
            _group = GetComponent<CanvasGroup>();
            if (_group == null) _group = gameObject.AddComponent<CanvasGroup>();
        }

        public void Show()
        {
            StopAllCoroutines();
            StartCoroutine(FadeRoutine(0f, 1f, FadeInDuration));
        }

        public void Hide()
        {
            StopAllCoroutines();
            StartCoroutine(FadeRoutine(1f, 0f, FadeOutDuration));
        }

        System.Collections.IEnumerator FadeRoutine(float from, float to, float duration)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                _group.alpha = Mathf.Lerp(from, to, t / duration);
                yield return null;
            }
            _group.alpha = to;
        }
    }

    /// <summary>Health bar damage "bleed" effect with configurable lag.</summary>
    public class HealthBarBleed : MonoBehaviour
    {
        [Tooltip("Seconds before the damage bleed catches up to actual health.")]
        public float BleedLag = 0.5f;
        [Tooltip("Speed at which the bleed bar descends.")]
        public float BleedSpeed = 2f;

        Image _bleedFill;
        Image _healthFill;
        float _displayedRatio = 1f;
        float _targetRatio = 1f;
        float _bleedTimer;

        public void Initialize(Image healthFill, Image bleedFill)
        {
            _healthFill = healthFill;
            _bleedFill = bleedFill;
            _displayedRatio = 1f;
            _targetRatio = 1f;
        }

        public void SetHealthRatio(float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            if (ratio < _targetRatio)
                _bleedTimer = BleedLag;
            _targetRatio = ratio;
            if (_healthFill != null) _healthFill.fillAmount = ratio;
        }

        void Update()
        {
            if (_bleedFill == null) return;
            if (_bleedTimer > 0f)
            {
                _bleedTimer -= Time.deltaTime;
                return;
            }
            if (_displayedRatio > _targetRatio)
            {
                _displayedRatio = Mathf.MoveTowards(_displayedRatio, _targetRatio, BleedSpeed * Time.deltaTime);
                _bleedFill.fillAmount = _displayedRatio;
            }
        }
    }
}
