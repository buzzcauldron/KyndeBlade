using UnityEngine;
using UnityEngine.UI;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Manuscript "bleeding ink" health bar: fill smudges/fades as health drops (ghost bar lags behind for ink-soaking-into-parchment look).</summary>
    [RequireComponent(typeof(RectTransform))]
    public class BleedingInkHealthBar : MonoBehaviour
    {
        public MedievalCharacter Character;
        public bool IsPlayer = true;
        public Image TrackImage;
        public Image FillImage;
        [Tooltip("Optional. Secondary fill that lags behind—creates the 'bleeding' smudge.")]
        public Image GhostFillImage;
        public Image BorderImage;
        public Text LabelText;

        [Header("Bleeding feel")]
        [Tooltip("How fast the ghost (smudge) follows the real fill. Lower = more lag, more ink bleed.")]
        [Range(0.02f, 0.5f)]
        public float GhostFollowSpeed = 0.12f;

        float _ghostFillAmount = 1f;

        void OnEnable()
        {
            if (Character != null)
                Character.OnHealthChanged += OnHealthChanged;
        }

        void OnDisable()
        {
            if (Character != null)
                Character.OnHealthChanged -= OnHealthChanged;
        }

        void Update()
        {
            if (GhostFillImage != null)
            {
                _ghostFillAmount = Mathf.Lerp(_ghostFillAmount, _targetFill, GhostFollowSpeed);
                GhostFillImage.type = Image.Type.Filled;
                GhostFillImage.fillMethod = Image.FillMethod.Horizontal;
                GhostFillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
                GhostFillImage.fillAmount = _ghostFillAmount;
                GhostFillImage.color = ManuscriptUITheme.HealthBarTrack;
            }
        }

        float _targetFill = 1f;

        public void SetCharacter(MedievalCharacter c)
        {
            if (Character != null)
                Character.OnHealthChanged -= OnHealthChanged;
            Character = c;
            _ghostFillAmount = 1f;
            _targetFill = 1f;
            if (Character != null)
            {
                Character.OnHealthChanged += OnHealthChanged;
                Refresh();
            }
        }

        void OnHealthChanged(float current, float max)
        {
            Refresh();
        }

        public void Refresh()
        {
            if (Character == null) return;
            float cur = Character.GetCurrentHealth();
            float max = Character.GetMaxHealth();
            float t = max > 0f ? Mathf.Clamp01(cur / max) : 0f;
            _targetFill = t;

            if (FillImage != null)
            {
                FillImage.type = Image.Type.Filled;
                FillImage.fillMethod = Image.FillMethod.Horizontal;
                FillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
                FillImage.fillAmount = t;
                FillImage.color = ManuscriptUITheme.HealthBarFill(t, IsPlayer);
            }
            if (LabelText != null)
                LabelText.text = $"{cur:0}/{max:0}";
        }
    }
}
