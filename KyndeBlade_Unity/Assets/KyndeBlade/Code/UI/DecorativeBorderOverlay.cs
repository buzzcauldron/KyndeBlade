using UnityEngine;
using UnityEngine.UI;

namespace KyndeBlade
{
    /// <summary>Manuscript-style decorative border overlay: context-aware (Green Chapel = vines, Pride = gold filigree). Assign sprites or colors per context; set context from combat or location.</summary>
    public class DecorativeBorderOverlay : MonoBehaviour
    {
        public enum BorderContext
        {
            Default,
            GreenChapel,
            Pride,
            Hunger,
            Elde
        }

        [Header("Display")]
        [Tooltip("Image that draws the border (sprite or tint). Often a full-screen border frame with transparent center.")]
        public Image BorderImage;
        [Tooltip("Optional. Overlay color tint per context (e.g. moss for Green Chapel, gold for Pride).")]
        public bool UseContextTint = true;

        [Header("Context tints (if UseContextTint)")]
        public Color DefaultTint = new Color(0.4f, 0.35f, 0.25f);
        public Color GreenChapelTint = new Color(0.22f, 0.35f, 0.2f);
        public Color PrideTint = new Color(0.5f, 0.4f, 0.2f);
        public Color HungerTint = new Color(0.5f, 0.48f, 0.45f);
        public Color EldeTint = new Color(0.35f, 0.33f, 0.3f);

        [Header("Optional sprites per context (vines, filigree)")]
        public Sprite DefaultSprite;
        public Sprite GreenChapelSprite;
        public Sprite PrideSprite;
        public Sprite HungerSprite;
        public Sprite EldeSprite;

        BorderContext _currentContext = BorderContext.Default;

        void Awake()
        {
            if (BorderImage == null)
                BorderImage = GetComponent<Image>();
        }

        /// <summary>Set border context from location or current boss (e.g. "Green Chapel", "Pride").</summary>
        public void SetContext(BorderContext context)
        {
            if (_currentContext == context) return;
            _currentContext = context;

            if (BorderImage == null) return;

            var (tint, sprite) = GetDisplayForContext(context);
            if (UseContextTint) BorderImage.color = tint;
            if (sprite != null) BorderImage.sprite = sprite;
        }

        (Color tint, Sprite sprite) GetDisplayForContext(BorderContext context)
        {
            switch (context)
            {
                case BorderContext.GreenChapel: return (GreenChapelTint, GreenChapelSprite);
                case BorderContext.Pride: return (PrideTint, PrideSprite);
                case BorderContext.Hunger: return (HungerTint, HungerSprite);
                case BorderContext.Elde: return (EldeTint, EldeSprite);
                default: return (DefaultTint, DefaultSprite);
            }
        }

        /// <summary>Set context by string (e.g. location id or boss name).</summary>
        public void SetContextFromString(string contextName)
        {
            if (string.IsNullOrEmpty(contextName)) { SetContext(BorderContext.Default); return; }
            if (contextName.IndexOf("Green", System.StringComparison.OrdinalIgnoreCase) >= 0) SetContext(BorderContext.GreenChapel);
            else if (contextName.IndexOf("Pride", System.StringComparison.OrdinalIgnoreCase) >= 0) SetContext(BorderContext.Pride);
            else if (contextName.IndexOf("Hunger", System.StringComparison.OrdinalIgnoreCase) >= 0) SetContext(BorderContext.Hunger);
            else if (contextName.IndexOf("Elde", System.StringComparison.OrdinalIgnoreCase) >= 0) SetContext(BorderContext.Elde);
            else SetContext(BorderContext.Default);
        }
    }
}
