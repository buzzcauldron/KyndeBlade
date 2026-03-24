using UnityEngine;

namespace KyndeBlade
{
    /// <summary>
    /// Creates lightweight procedural VFX without requiring the Unity ParticleSystem module.
    /// </summary>
    public static class ManuscriptParticleFactory
    {
        static Sprite _pulseSprite;

        public static GameObject CreateHitImpact(Vector3 position)
        {
            return CreatePulse("HitImpact", position, null, new Color(0.2f, 0.15f, 0.1f, 0.9f), new Color(0.1f, 0.08f, 0.05f, 0f), 0.08f, 0.28f, 0.3f, false);
        }

        public static GameObject CreateBurningEmbers(Transform parent)
        {
            return CreatePulse("BurningEmbers", Vector3.zero, parent, new Color(1f, 0.5f, 0.1f, 0.8f), new Color(1f, 0.2f, 0f, 0f), 0.05f, 0.15f, 0.8f, true);
        }

        public static GameObject CreateFrostCrystals(Transform parent)
        {
            return CreatePulse("FrostCrystals", Vector3.zero, parent, new Color(0.7f, 0.85f, 1f, 0.8f), new Color(0.5f, 0.7f, 1f, 0f), 0.06f, 0.17f, 1.1f, true);
        }

        public static GameObject CreatePoisonDrip(Transform parent)
        {
            return CreatePulse("PoisonDrip", Vector3.zero, parent, new Color(0.3f, 0.7f, 0.2f, 0.8f), new Color(0.2f, 0.5f, 0.1f, 0f), 0.05f, 0.14f, 0.7f, true);
        }

        public static GameObject CreateStunStars(Transform parent)
        {
            return CreatePulse("StunStars", Vector3.zero, parent, new Color(1f, 1f, 0.5f, 0.9f), new Color(1f, 1f, 0.3f, 0f), 0.07f, 0.18f, 0.65f, true);
        }

        public static GameObject CreateBlessedGlow(Transform parent)
        {
            return CreatePulse("BlessedGlow", Vector3.zero, parent, new Color(1f, 0.9f, 0.5f, 0.6f), new Color(ManuscriptUITheme.Gold.r, ManuscriptUITheme.Gold.g, ManuscriptUITheme.Gold.b, 0f), 0.08f, 0.2f, 1f, true);
        }

        public static GameObject CreateBlessingPickup(Vector3 position)
        {
            return CreatePulse("BlessingPickup", position, null, ManuscriptUITheme.Gold, new Color(1f, 1f, 1f, 0f), 0.08f, 0.24f, 1.2f, false);
        }

        public static GameObject CreateDeathDissolve(Transform parent)
        {
            return CreatePulse("DeathDissolve", Vector3.zero, parent, new Color(0.15f, 0.1f, 0.08f, 0.7f), new Color(0.1f, 0.08f, 0.05f, 0f), 0.1f, 0.3f, 1.5f, false);
        }

        static GameObject CreatePulse(
            string name,
            Vector3 worldPosition,
            Transform parent,
            Color startColor,
            Color endColor,
            float startScale,
            float endScale,
            float lifetime,
            bool loop)
        {
            var go = new GameObject(name);
            if (parent != null)
            {
                go.transform.SetParent(parent, false);
                go.transform.localPosition = new Vector3(0f, 0.3f, 0f);
            }
            else
            {
                go.transform.position = worldPosition;
            }

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = GetPulseSprite();
            sr.color = startColor;
            sr.sortingOrder = 8;

            var fx = go.AddComponent<ManuscriptPulseEffect>();
            fx.StartColor = startColor;
            fx.EndColor = endColor;
            fx.StartScale = startScale;
            fx.EndScale = endScale;
            fx.Lifetime = Mathf.Max(0.05f, lifetime);
            fx.Loop = loop;
            return go;
        }

        static Sprite GetPulseSprite()
        {
            if (_pulseSprite != null)
                return _pulseSprite;

            const int size = 16;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            var clear = new Color(0f, 0f, 0f, 0f);
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    tex.SetPixel(x, y, clear);

            int cx = size / 2;
            int cy = size / 2;
            int r = 6;
            int rsq = r * r;
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    int dx = x - cx;
                    int dy = y - cy;
                    if (dx * dx + dy * dy <= rsq)
                        tex.SetPixel(x, y, Color.white);
                }

            tex.Apply();
            _pulseSprite = Sprite.Create(tex, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 16f);
            return _pulseSprite;
        }
    }

    public class ManuscriptPulseEffect : MonoBehaviour
    {
        public Color StartColor = Color.white;
        public Color EndColor = new Color(1f, 1f, 1f, 0f);
        public float StartScale = 0.08f;
        public float EndScale = 0.2f;
        public float Lifetime = 0.8f;
        public bool Loop;

        float _timer;
        SpriteRenderer _renderer;

        void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            transform.localScale = Vector3.one * StartScale;
        }

        void Update()
        {
            _timer += Time.deltaTime;
            var duration = Mathf.Max(0.05f, Lifetime);
            var t = Mathf.Clamp01(_timer / duration);

            if (_renderer != null)
                _renderer.color = Color.Lerp(StartColor, EndColor, t);
            transform.localScale = Vector3.one * Mathf.Lerp(StartScale, EndScale, t);

            if (Loop)
            {
                if (_timer >= duration)
                    _timer = 0f;
            }
            else if (_timer >= duration)
            {
                Destroy(gameObject);
            }
        }
    }
}
