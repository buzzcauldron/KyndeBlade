using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Living manuscript border: procedural vine/floral along screen edges. GreenKnight = lush green vines; Hunger = withered thorny. Uses optional texture mask for hand-stamped look.</summary>
    public class DynamicBorder : MonoBehaviour
    {
        public enum VineTheme
        {
            None,
            Lush,   // Green Knight
            Withered // Hunger
        }

        const string StampMaterialName = "BorderStamp";

        [Header("References")]
        [Tooltip("Optional. When set, context is also pushed to DecorativeBorderOverlay.")]
        public DecorativeBorderOverlay DecorativeOverlay;
        [Tooltip("TurnManager to detect current boss (GreenKnight / Hunger).")]
        public TurnManager TurnManager;
        [Tooltip("Root RectTransform for spawned vine/floral elements (e.g. full-screen canvas child).")]
        public RectTransform BorderRoot;

        [Header("Lush (Green Knight)")]
        public Sprite[] LushVineSprites;
        public Color LushTint = new Color(0.22f, 0.4f, 0.2f);
        [Header("Withered (Hunger)")]
        public Sprite[] WitheredVineSprites;
        public Color WitheredTint = new Color(0.35f, 0.28f, 0.22f);

        [Header("Layout")]
        [Tooltip("Width of the border band in pixels (or fraction of edge).")]
        public float BorderBandWidth = 80f;
        [Tooltip("Approximate spacing between vine elements along each edge.")]
        public float VineSpacing = 60f;
        [Tooltip("Random scale min/max for each vine.")]
        public Vector2 VineScaleMinMax = new Vector2(0.7f, 1.3f);
        [Tooltip("Optional texture mask for hand-stamped look (multiply with border).")]
        public Texture2D StampMaskTexture;

        VineTheme _currentTheme = VineTheme.None;
        readonly List<Image> _spawned = new List<Image>();

        void Start()
        {
            if (TurnManager == null) TurnManager = FindObjectOfType<TurnManager>();
            if (TurnManager != null) TurnManager.OnTurnChanged += OnTurnChanged;
            if (BorderRoot != null) RefreshBorder();
        }

        void OnDestroy()
        {
            if (TurnManager != null) TurnManager.OnTurnChanged -= OnTurnChanged;
            ClearSpawned();
        }

        void OnTurnChanged(MedievalCharacter current)
        {
            if (current == null) return;
            var gk = current.GetComponent<GreenKnightBossAI>();
            var hunger = current.GetComponent<HungerBossAI>();
            if (gk != null) SetTheme(VineTheme.Lush);
            else if (hunger != null) SetTheme(VineTheme.Withered);
            else SetTheme(VineTheme.None);
        }

        static DecorativeBorderOverlay.BorderContext ToBorderContext(VineTheme theme)
        {
            switch (theme)
            {
                case VineTheme.Lush: return DecorativeBorderOverlay.BorderContext.GreenChapel;
                case VineTheme.Withered: return DecorativeBorderOverlay.BorderContext.Hunger;
                default: return DecorativeBorderOverlay.BorderContext.Default;
            }
        }

        /// <summary>Set theme explicitly (e.g. from location or combat start).</summary>
        public void SetTheme(VineTheme theme)
        {
            if (_currentTheme == theme) return;
            _currentTheme = theme;

            if (DecorativeOverlay != null)
                DecorativeOverlay.SetContext(ToBorderContext(theme));

            RefreshBorder();
        }

        void RefreshBorder()
        {
            ClearSpawned();
            if (BorderRoot == null || _currentTheme == VineTheme.None) return;

            var (sprites, tint) = GetThemeConfig(_currentTheme);
            if (sprites == null || sprites.Length == 0) return;

            Rect r = BorderRoot.rect;
            PlaceVinesAlongEdge(sprites, tint, VineSpacing, r.width, true, 1f);
            PlaceVinesAlongEdge(sprites, tint, VineSpacing, r.width, true, -1f);
            PlaceVinesAlongEdge(sprites, tint, VineSpacing, r.height, false, -1f);
            PlaceVinesAlongEdge(sprites, tint, VineSpacing, r.height, false, 1f);
        }

        (Sprite[] sprites, Color tint) GetThemeConfig(VineTheme theme)
        {
            switch (theme)
            {
                case VineTheme.Lush: return (LushVineSprites, LushTint);
                case VineTheme.Withered: return (WitheredVineSprites, WitheredTint);
                default: return (null, default);
            }
        }

        void PlaceVinesAlongEdge(Sprite[] sprites, Color tint, float spacing, float edgeLength, bool horizontal, float side)
        {
            int n = Mathf.Max(1, Mathf.FloorToInt(edgeLength / spacing));
            for (int i = 0; i < n; i++)
            {
                float t = (i + 0.5f) / n;
                float jitter = (Mathf.PerlinNoise(i * 0.7f, 0.3f) - 0.5f) * spacing * 0.5f;
                float along = t * edgeLength + jitter;
                Sprite s = sprites[Random.Range(0, sprites.Length)];
                float scale = Mathf.Lerp(VineScaleMinMax.x, VineScaleMinMax.y, Mathf.PerlinNoise(i * 0.2f, 0.1f));
                float rot = (Mathf.PerlinNoise(i * 0.5f, 0.2f) - 0.5f) * 30f;
                if (horizontal) SpawnVine(s, tint, along, edgeLength, side, 0, rot, scale, true);
                else SpawnVine(s, tint, along, edgeLength, 0, side, rot + 90f, scale, false);
            }
        }

        void SpawnVine(Sprite sprite, Color tint, float along, float edgeLength, float sideX, float sideY, float rotation, float scale, bool horizontal)
        {
            GameObject go = new GameObject("Vine");
            go.transform.SetParent(BorderRoot, false);
            var rt = go.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(BorderBandWidth, BorderBandWidth);
            float halfW = BorderRoot.rect.width * 0.5f;
            float halfH = BorderRoot.rect.height * 0.5f;
            float band = BorderBandWidth * 0.5f;
            if (horizontal)
            {
                float x = (along / edgeLength - 0.5f) * BorderRoot.rect.width;
                float y = sideY * (halfH - band);
                rt.anchoredPosition = new Vector2(x, y);
            }
            else
            {
                float x = sideX * (halfW - band);
                float y = (along / edgeLength - 0.5f) * BorderRoot.rect.height;
                rt.anchoredPosition = new Vector2(x, y);
            }
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.localRotation = Quaternion.Euler(0, 0, rotation);
            rt.localScale = Vector3.one * scale;

            var img = go.AddComponent<Image>();
            img.sprite = sprite;
            img.color = tint;
            img.raycastTarget = false;
            if (StampMaskTexture != null)
            {
                var mat = new Material(Shader.Find("UI/Default"));
                mat.name = StampMaterialName;
                mat.SetTexture("_Mask", StampMaskTexture);
                img.material = mat;
            }
            _spawned.Add(img);
        }

        void ClearSpawned()
        {
            bool playing = Application.isPlaying;
            foreach (var img in _spawned)
            {
                if (img == null) continue;
                if (img.material != null && img.material.name == StampMaterialName)
                {
                    if (playing) Destroy(img.material);
                    else DestroyImmediate(img.material);
                }
                if (img.gameObject != null)
                {
                    if (playing) Destroy(img.gameObject);
                    else DestroyImmediate(img.gameObject);
                }
            }
            _spawned.Clear();
        }
    }
}
