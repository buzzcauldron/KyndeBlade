using UnityEngine;
using UnityEngine.UI;

namespace KyndeBlade
{
    /// <summary>Manuscript-themed UI colors and styling. See docs/UI_MANUSCRIPT_THEME.md.</summary>
    public static class ManuscriptUITheme
    {
        // Backgrounds (from illuminated manuscript reference)
        public static readonly Color ParchmentLight = Hex(0xF5E6C8);
        public static readonly Color Parchment = Hex(0xE8D4A8);
        public static readonly Color ParchmentAged = Hex(0xD4C090);
        public static readonly Color ParchmentDark = Hex(0xC4A870);

        // Text (Ink) — blackletter black
        public static readonly Color InkPrimary = Hex(0x1A1810);
        public static readonly Color InkSecondary = Hex(0x3D2C1F);
        public static readonly Color InkLight = Hex(0x5C4A3A);

        // Accents (illumination, rubrication, lapis)
        public static readonly Color Gold = Hex(0xD4A84B);
        public static readonly Color GoldDark = Hex(0x8B7355);
        public static readonly Color Rubrication = Hex(0xB8461A);
        public static readonly Color LapisBlue = Hex(0x2E5A8C);

        // Borders
        public static readonly Color BorderSepia = Hex(0x6B5344);
        public static readonly Color BorderBlue = Hex(0x3D6B9E);
        public static readonly Color BorderRed = Hex(0x8B3626);
        public static readonly Color BorderDark = Hex(0x4A3A2E);

        static Color Hex(uint rgb)
        {
            float r = ((rgb >> 16) & 0xFF) / 255f;
            float g = ((rgb >> 8) & 0xFF) / 255f;
            float b = (rgb & 0xFF) / 255f;
            return new Color(r, g, b, 1f);
        }

        /// <summary>Apply manuscript styling to a Text component.</summary>
        /// <param name="emphasis">Use Gold for highlights (e.g. victory, current turn).</param>
        /// <param name="rubrication">Use red for headings/loss (e.g. defeat).</param>
        /// <param name="lapis">Use lapis blue for secondary emphasis (e.g. labels, categories).</param>
        public static void ApplyToText(Text t, bool emphasis = false, bool rubrication = false, bool lapis = false)
        {
            if (t == null) return;
            if (rubrication) t.color = Rubrication;
            else if (lapis) t.color = LapisBlue;
            else t.color = emphasis ? Gold : InkPrimary;
        }

        /// <summary>Apply manuscript styling to a Button (background, border, text).</summary>
        public static void ApplyToButton(Button btn, bool emphasis = false)
        {
            if (btn == null) return;
            var colors = btn.colors;
            colors.normalColor = ParchmentAged;
            colors.highlightedColor = Parchment;
            colors.pressedColor = ParchmentDark;
            colors.disabledColor = new Color(ParchmentAged.r, ParchmentAged.g, ParchmentAged.b, 0.5f);
            btn.colors = colors;

            var img = btn.GetComponent<Image>();
            if (img != null)
            {
                img.color = ParchmentAged;
            }

            var text = btn.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.color = emphasis ? Gold : InkPrimary;
            }
        }

        /// <summary>Apply manuscript background and border to an Image.</summary>
        public static void ApplyToPanel(Image img)
        {
            if (img == null) return;
            img.color = ParchmentLight;
        }

        /// <summary>Apply manuscript colors to an Image used as a border/frame.</summary>
        public static void ApplyToBorder(Image img)
        {
            if (img == null) return;
            img.color = BorderSepia;
        }

        /// <summary>Verdigris (green-blue patina) for good/main characters.</summary>
        public static readonly Color Verdigris = Hex(0x43B3AE);

        /// <summary>Vermillion (red) for enemies/bad characters.</summary>
        public static readonly Color Vermillion = Hex(0xB8461A);

        /// <summary>Health bar fill: verdigris for players, vermillion for enemies. Gold when full.</summary>
        public static Color HealthBarFill(float normalizedHealth, bool isPlayer)
        {
            if (normalizedHealth >= 0.99f) return Gold;
            return isPlayer ? Verdigris : Vermillion;
        }

        /// <summary>Health bar track (background): parchment aged, like manuscript margins.</summary>
        public static readonly Color HealthBarTrack = ParchmentAged;

        /// <summary>Health bar border: sepia frame like miniature illustrations.</summary>
        public static readonly Color HealthBarBorder = BorderSepia;
    }
}
