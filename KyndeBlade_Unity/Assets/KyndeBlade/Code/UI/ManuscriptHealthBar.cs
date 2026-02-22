using UnityEngine;
using UnityEngine.UI;

namespace KyndeBlade
{
    /// <summary>Manuscript-style health bar: parchment track, rubrication/gold fill, sepia border.</summary>
    [RequireComponent(typeof(RectTransform))]
    public class ManuscriptHealthBar : MonoBehaviour
    {
        public MedievalCharacter Character;
        public bool IsPlayer = true;
        public Image TrackImage;
        public Image FillImage;
        public Image BorderImage;
        public Text LabelText;

        const float BarWidth = 80f;
        const float BarHeight = 10f;
        const float BorderThickness = 2f;

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

        public void SetCharacter(MedievalCharacter c)
        {
            if (Character != null)
                Character.OnHealthChanged -= OnHealthChanged;
            Character = c;
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

        /// <summary>Build manuscript-style bar UI. Call SetCharacter after.</summary>
        /// <param name="isPlayer">True for verdigris (good), false for vermillion (enemies).</param>
        public static ManuscriptHealthBar Create(Transform parent, float width = BarWidth, float height = BarHeight, bool isPlayer = true)
        {
            var go = new GameObject("ManuscriptHealthBar");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            var le = go.AddComponent<LayoutElement>();
            le.preferredWidth = width + BorderThickness * 2f;
            le.preferredHeight = height + BorderThickness * 2f;
            le.flexibleWidth = 0f;

            var bar = go.AddComponent<ManuscriptHealthBar>();
            bar.IsPlayer = isPlayer;

            var border = new GameObject("Border");
            border.transform.SetParent(go.transform, false);
            var borderRect = border.AddComponent<RectTransform>();
            borderRect.anchorMin = Vector2.zero;
            borderRect.anchorMax = Vector2.one;
            borderRect.offsetMin = borderRect.offsetMax = Vector2.zero;
            var borderImg = border.AddComponent<Image>();
            borderImg.sprite = UnityEngine.Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);
            borderImg.color = ManuscriptUITheme.HealthBarBorder;
            bar.BorderImage = borderImg;

            var track = new GameObject("Track");
            track.transform.SetParent(go.transform, false);
            var trackRect = track.AddComponent<RectTransform>();
            trackRect.anchorMin = Vector2.zero;
            trackRect.anchorMax = Vector2.one;
            trackRect.offsetMin = new Vector2(BorderThickness, BorderThickness);
            trackRect.offsetMax = new Vector2(-BorderThickness, -BorderThickness);
            var trackImg = track.AddComponent<Image>();
            trackImg.sprite = UnityEngine.Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);
            trackImg.color = ManuscriptUITheme.HealthBarTrack;
            bar.TrackImage = trackImg;

            var fill = new GameObject("Fill");
            fill.transform.SetParent(track.transform, false);
            var fillRect = fill.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = fillRect.offsetMax = Vector2.zero;
            var fillImg = fill.AddComponent<Image>();
            fillImg.sprite = UnityEngine.Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);
            fillImg.color = isPlayer ? ManuscriptUITheme.Verdigris : ManuscriptUITheme.Vermillion;
            fillImg.type = Image.Type.Filled;
            fillImg.fillMethod = Image.FillMethod.Horizontal;
            fillImg.fillOrigin = (int)Image.OriginHorizontal.Left;
            bar.FillImage = fillImg;

            var label = new GameObject("Label");
            label.transform.SetParent(track.transform, false);
            var labelRect = label.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = labelRect.offsetMax = Vector2.zero;
            var labelTxt = label.AddComponent<Text>();
            labelTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            labelTxt.fontSize = 9;
            labelTxt.alignment = TextAnchor.MiddleCenter;
            ManuscriptUITheme.ApplyToText(labelTxt);
            bar.LabelText = labelTxt;

            return bar;
        }
    }
}
