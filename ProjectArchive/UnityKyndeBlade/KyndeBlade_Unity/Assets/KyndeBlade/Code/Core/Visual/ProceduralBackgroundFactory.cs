using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Generates procedural location backgrounds and UI textures
    /// in an illuminated manuscript style.</summary>
    public static class ProceduralBackgroundFactory
    {
        public static Sprite CreateLocationBackground(string locationId, int width = 256, int height = 224)
        {
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;

            var (sky, ground) = GetLocationColors(locationId);
            int horizon = height / 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color c;
                    if (y >= horizon)
                    {
                        float t = (y - horizon) / (float)(height - horizon);
                        c = Color.Lerp(sky, Color.Lerp(sky, Color.white, 0.3f), t);
                    }
                    else
                    {
                        float t = y / (float)horizon;
                        c = Color.Lerp(Color.Lerp(ground, ManuscriptUITheme.ParchmentDark, 0.5f), ground, t);
                    }
                    float dither = ((x + y) % 2) * 0.02f;
                    c.r = Mathf.Clamp01(c.r + dither);
                    c.g = Mathf.Clamp01(c.g + dither);
                    c.b = Mathf.Clamp01(c.b + dither);
                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 16f);
        }

        static (Color sky, Color ground) GetLocationColors(string locationId)
        {
            if (string.IsNullOrEmpty(locationId))
                return (new Color(0.6f, 0.7f, 0.85f), new Color(0.4f, 0.5f, 0.3f));

            var id = locationId.ToLowerInvariant();
            return id switch
            {
                "malvern" => (new Color(0.55f, 0.7f, 0.9f), new Color(0.35f, 0.55f, 0.25f)),
                "fayre_felde" => (new Color(0.6f, 0.75f, 0.85f), new Color(0.45f, 0.55f, 0.3f)),
                "tour" => (new Color(0.5f, 0.6f, 0.8f), new Color(0.5f, 0.45f, 0.35f)),
                "dongeoun" => (new Color(0.3f, 0.3f, 0.35f), new Color(0.2f, 0.18f, 0.15f)),
                "piers_field" => (new Color(0.6f, 0.7f, 0.8f), new Color(0.4f, 0.5f, 0.25f)),
                "seven_sins" => (new Color(0.45f, 0.35f, 0.4f), new Color(0.35f, 0.25f, 0.2f)),
                "quest_do_wel" => (new Color(0.55f, 0.6f, 0.75f), new Color(0.4f, 0.45f, 0.3f)),
                "dongeoun_depths" => (new Color(0.2f, 0.2f, 0.25f), new Color(0.15f, 0.12f, 0.1f)),
                "years_pass" => (new Color(0.65f, 0.6f, 0.55f), new Color(0.45f, 0.4f, 0.35f)),
                "field_of_grace" => (new Color(0.75f, 0.8f, 0.9f), new Color(0.5f, 0.6f, 0.4f)),
                "green_chapel" => (new Color(0.3f, 0.5f, 0.35f), new Color(0.2f, 0.35f, 0.2f)),
                "otherworld" => (new Color(0.3f, 0.25f, 0.4f), new Color(0.2f, 0.15f, 0.25f)),
                "boundary_tree" => (new Color(0.35f, 0.3f, 0.45f), new Color(0.25f, 0.2f, 0.15f)),
                _ => (new Color(0.6f, 0.7f, 0.85f), new Color(0.4f, 0.5f, 0.3f))
            };
        }

        public static Sprite CreateMapNodeIcon(string locationId, int size = 32)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;

            var (_, ground) = GetLocationColors(locationId);
            Color border = ManuscriptUITheme.InkPrimary;
            float center = size / 2f;
            float radius = size / 2f - 2f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    if (dist < radius - 1f)
                        tex.SetPixel(x, y, ground);
                    else if (dist < radius + 1f)
                        tex.SetPixel(x, y, border);
                    else
                        tex.SetPixel(x, y, Color.clear);
                }
            }

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 16f);
        }

        public static Sprite CreateParchmentTexture(int width = 128, int height = 128)
        {
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;

            var rng = new System.Random(42);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float noise = (float)rng.NextDouble() * 0.08f - 0.04f;
                    var c = ManuscriptUITheme.ParchmentLight;
                    c.r = Mathf.Clamp01(c.r + noise);
                    c.g = Mathf.Clamp01(c.g + noise);
                    c.b = Mathf.Clamp01(c.b + noise - 0.01f);
                    float edgeFade = Mathf.Min(
                        Mathf.Min((float)x / 8f, (float)(width - x) / 8f),
                        Mathf.Min((float)y / 8f, (float)(height - y) / 8f)
                    );
                    c = Color.Lerp(ManuscriptUITheme.ParchmentAged, c, Mathf.Clamp01(edgeFade));
                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 16f);
        }

        public static Sprite CreateActionButtonIcon(string actionName, int size = 24)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;

            Color bg = ManuscriptUITheme.Parchment;
            Color fg = ManuscriptUITheme.InkPrimary;

            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    tex.SetPixel(x, y, bg);

            if (!string.IsNullOrEmpty(actionName))
            {
                var name = actionName.ToLowerInvariant();
                if (name.Contains("attack") || name.Contains("strike"))
                    DrawCross(tex, fg, size);
                else if (name.Contains("dodge") || name.Contains("escapade"))
                    DrawArrow(tex, fg, size);
                else if (name.Contains("parry") || name.Contains("ward"))
                    DrawShield(tex, fg, size);
                else if (name.Contains("heal") || name.Contains("rest"))
                    DrawPlus(tex, fg, size);
                else
                    DrawDiamond(tex, fg, size);
            }

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 16f);
        }

        static void DrawCross(Texture2D tex, Color c, int size)
        {
            int half = size / 2;
            for (int i = size / 4; i < size * 3 / 4; i++)
            {
                tex.SetPixel(i, half, c);
                tex.SetPixel(i, half - 1, c);
                tex.SetPixel(half, i, c);
                tex.SetPixel(half - 1, i, c);
            }
        }

        static void DrawArrow(Texture2D tex, Color c, int size)
        {
            int mid = size / 2;
            for (int i = 2; i < size - 2; i++)
                tex.SetPixel(i, mid, c);
            for (int i = 0; i < size / 4; i++)
            {
                tex.SetPixel(size - 4 - i, mid + i, c);
                tex.SetPixel(size - 4 - i, mid - i, c);
            }
        }

        static void DrawShield(Texture2D tex, Color c, int size)
        {
            int mid = size / 2;
            for (int y = size / 4; y < size * 3 / 4; y++)
            {
                int w = (int)(mid * (1f - Mathf.Abs(y - mid) / (float)mid) * 0.6f) + 2;
                for (int x = mid - w; x <= mid + w; x++)
                    if (x >= 0 && x < size)
                        tex.SetPixel(x, y, c);
            }
        }

        static void DrawPlus(Texture2D tex, Color c, int size)
        {
            int mid = size / 2;
            int arm = size / 6;
            for (int i = mid - arm; i <= mid + arm; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (mid + j >= 0 && mid + j < size) tex.SetPixel(i, mid + j, c);
                    if (i >= 0 && i < size) tex.SetPixel(mid + j, i, c);
                }
            }
        }

        static void DrawDiamond(Texture2D tex, Color c, int size)
        {
            int mid = size / 2;
            int r = size / 4;
            for (int y = mid - r; y <= mid + r; y++)
                for (int x = mid - r; x <= mid + r; x++)
                    if (Mathf.Abs(x - mid) + Mathf.Abs(y - mid) <= r)
                        tex.SetPixel(x, y, c);
        }
    }
}
