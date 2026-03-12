using UnityEngine;

namespace KyndeBlade
{
    /// <summary>
    /// Generates distinct placeholder sprites for characters using Texture2D pixel drawing.
    /// Each character gets a unique silhouette and color from manuscript palettes.
    /// Replace with real art by assigning sprites in CharacterVisualConfig.
    /// </summary>
    public static class PlaceholderSpriteFactory
    {
        const int SpriteSize = 64;
        const float PixelsPerUnit = 64f;

        static readonly Color Outline = ManuscriptUITheme.InkPrimary;

        public static Sprite GetSpriteForCharacter(string characterKey, bool isPlayer)
        {
            var key = (characterKey ?? "").ToLowerInvariant();
            switch (key)
            {
                case "wille":       return CreateDreamerSprite();
                case "piers":       return CreatePlowmanSprite();
                case "conscience":  return CreateMageSprite();
                case "false":       return CreateRogueSprite();
                case "lady mede":   return CreateCrownedSprite(new Color(0.65f, 0.5f, 0.2f));
                case "wrath":       return CreateBerserkerSprite();
                case "hunger":      return CreateGauntSprite();
                case "pride":       return CreateTowerSprite();
                case "green knight":
                case "greenknight": return CreateKnightSprite(new Color(0.2f, 0.55f, 0.25f));
                case "elde":        return CreateEldeSprite();
                case "envy":        return CreateDiamond(new Color(0.3f, 0.6f, 0.3f));
                case "sloth":       return CreateBlobSprite(new Color(0.5f, 0.5f, 0.5f));
                case "lust":        return CreateHeartSprite(new Color(0.85f, 0.4f, 0.55f));
                default:
                    return isPlayer ? CreateGenericPlayerSprite() : CreateGenericEnemySprite();
            }
        }

        public static Color GetColorForCharacter(string characterKey, bool isPlayer)
        {
            var key = (characterKey ?? "").ToLowerInvariant();
            switch (key)
            {
                case "wille":       return ManuscriptUITheme.LapisBlue;
                case "piers":       return new Color(0.55f, 0.42f, 0.28f);
                case "conscience":  return ManuscriptUITheme.Verdigris;
                case "false":       return new Color(0.4f, 0.35f, 0.45f);
                case "lady mede":   return ManuscriptUITheme.Gold;
                case "wrath":       return ManuscriptUITheme.Rubrication;
                case "hunger":      return new Color(0.45f, 0.52f, 0.3f);
                case "pride":       return new Color(0.7f, 0.55f, 0.2f);
                case "green knight":
                case "greenknight": return new Color(0.2f, 0.55f, 0.25f);
                case "elde":        return new Color(0.5f, 0.48f, 0.52f);
                case "envy":        return new Color(0.3f, 0.6f, 0.3f);
                case "sloth":       return new Color(0.5f, 0.5f, 0.5f);
                case "lust":        return new Color(0.85f, 0.4f, 0.55f);
                default:
                    return isPlayer ? ManuscriptUITheme.Verdigris : ManuscriptUITheme.Vermillion;
            }
        }

        public static Vector3 GetScaleForCharacter(string characterKey)
        {
            var key = (characterKey ?? "").ToLowerInvariant();
            switch (key)
            {
                case "hunger":      return new Vector3(0.7f, 1.4f, 1f);
                case "pride":       return new Vector3(1.1f, 1.3f, 1f);
                case "green knight":
                case "greenknight": return new Vector3(1.2f, 1.3f, 1f);
                case "elde":        return new Vector3(0.9f, 1.1f, 1f);
                case "wille":       return new Vector3(0.85f, 1.15f, 1f);
                case "piers":       return new Vector3(0.95f, 1.1f, 1f);
                case "conscience":  return new Vector3(0.8f, 1.2f, 1f);
                case "envy":        return new Vector3(0.9f, 1.1f, 1f);
                case "sloth":       return new Vector3(1.3f, 0.8f, 1f);
                case "lust":        return new Vector3(0.85f, 1.15f, 1f);
                default:            return new Vector3(0.85f, 1.15f, 1f);
            }
        }

        // --- Shape generators ---

        static Sprite CreateDreamerSprite()
        {
            var tex = NewTexture();
            var c = ManuscriptUITheme.LapisBlue;
            DrawCircle(tex, 32, 38, 14, c);
            DrawRect(tex, 22, 8, 42, 28, c);
            DrawCircle(tex, 32, 52, 6, Outline);
            DrawOutlineCircle(tex, 32, 38, 14, Outline);
            Apply(tex);
            return MakeSprite(tex);
        }

        static Sprite CreatePlowmanSprite()
        {
            var tex = NewTexture();
            var c = new Color(0.55f, 0.42f, 0.28f);
            DrawRect(tex, 20, 8, 44, 36, c);
            DrawCircle(tex, 32, 44, 10, c);
            DrawLine(tex, 10, 20, 20, 20, Outline, 2);
            DrawOutlineRect(tex, 20, 8, 44, 36, Outline);
            Apply(tex);
            return MakeSprite(tex);
        }

        static Sprite CreateMageSprite()
        {
            var tex = NewTexture();
            var c = ManuscriptUITheme.Verdigris;
            DrawTriangle(tex, 32, 55, 16, 8, 48, 8, c);
            DrawCircle(tex, 32, 44, 8, c);
            DrawRect(tex, 24, 8, 40, 30, c);
            DrawOutlineCircle(tex, 32, 44, 8, Outline);
            Apply(tex);
            return MakeSprite(tex);
        }

        static Sprite CreateRogueSprite()
        {
            var tex = NewTexture();
            var c = new Color(0.4f, 0.35f, 0.45f);
            DrawDiamond(tex, 32, 30, 16, 24, c);
            DrawCircle(tex, 32, 48, 7, c);
            DrawOutlineCircle(tex, 32, 48, 7, Outline);
            Apply(tex);
            return MakeSprite(tex);
        }

        static Sprite CreateCrownedSprite(Color bodyColor)
        {
            var tex = NewTexture();
            DrawRect(tex, 22, 6, 42, 30, bodyColor);
            DrawCircle(tex, 32, 38, 10, bodyColor);
            DrawRect(tex, 24, 50, 40, 56, ManuscriptUITheme.Gold);
            DrawRect(tex, 28, 56, 30, 60, ManuscriptUITheme.Gold);
            DrawRect(tex, 34, 56, 36, 60, ManuscriptUITheme.Gold);
            DrawOutlineRect(tex, 22, 6, 42, 30, Outline);
            Apply(tex);
            return MakeSprite(tex);
        }

        static Sprite CreateBerserkerSprite()
        {
            var tex = NewTexture();
            var c = ManuscriptUITheme.Rubrication;
            DrawRect(tex, 18, 6, 46, 34, c);
            DrawCircle(tex, 32, 42, 12, c);
            DrawLine(tex, 18, 34, 12, 44, ManuscriptUITheme.Vermillion, 3);
            DrawLine(tex, 46, 34, 52, 44, ManuscriptUITheme.Vermillion, 3);
            DrawOutlineRect(tex, 18, 6, 46, 34, Outline);
            Apply(tex);
            return MakeSprite(tex);
        }

        static Sprite CreateGauntSprite()
        {
            var tex = NewTexture();
            var c = new Color(0.45f, 0.52f, 0.3f);
            DrawRect(tex, 26, 4, 38, 40, c);
            DrawCircle(tex, 32, 46, 8, c);
            DrawOutlineRect(tex, 26, 4, 38, 40, Outline);
            DrawOutlineCircle(tex, 32, 46, 8, Outline);
            Apply(tex);
            return MakeSprite(tex);
        }

        static Sprite CreateTowerSprite()
        {
            var tex = NewTexture();
            var c = new Color(0.7f, 0.55f, 0.2f);
            DrawTriangle(tex, 32, 58, 14, 8, 50, 8, c);
            DrawRect(tex, 22, 8, 42, 40, c);
            DrawOutlineRect(tex, 22, 8, 42, 40, Outline);
            Apply(tex);
            return MakeSprite(tex);
        }

        static Sprite CreateKnightSprite(Color armor)
        {
            var tex = NewTexture();
            DrawRect(tex, 18, 8, 46, 38, armor);
            DrawCircle(tex, 32, 46, 10, armor);
            DrawRect(tex, 28, 46, 36, 56, armor);
            DrawOutlineRect(tex, 18, 8, 46, 38, Outline);
            DrawOutlineCircle(tex, 32, 46, 10, Outline);
            DrawLine(tex, 46, 24, 56, 18, Outline, 2);
            Apply(tex);
            return MakeSprite(tex);
        }

        static Sprite CreateEldeSprite()
        {
            var tex = NewTexture();
            var c = new Color(0.5f, 0.48f, 0.52f);
            DrawRect(tex, 24, 6, 40, 34, c);
            DrawCircle(tex, 32, 42, 9, c);
            DrawLine(tex, 24, 34, 18, 40, c, 2);
            DrawLine(tex, 40, 34, 46, 40, c, 2);
            DrawOutlineRect(tex, 24, 6, 40, 34, Outline);
            DrawOutlineCircle(tex, 32, 42, 9, Outline);
            Apply(tex);
            return MakeSprite(tex);
        }

        static Sprite CreateGenericPlayerSprite()
        {
            var tex = NewTexture();
            var c = ManuscriptUITheme.Verdigris;
            DrawCircle(tex, 32, 32, 16, c);
            DrawOutlineCircle(tex, 32, 32, 16, Outline);
            Apply(tex);
            return MakeSprite(tex);
        }

        static Sprite CreateGenericEnemySprite()
        {
            var tex = NewTexture();
            var c = ManuscriptUITheme.Vermillion;
            DrawDiamond(tex, 32, 32, 18, 24, c);
            Apply(tex);
            return MakeSprite(tex);
        }

        static Sprite CreateDiamond(Color c)
        {
            var tex = NewTexture();
            DrawDiamond(tex, 32, 32, 16, 22, c);
            DrawOutlineCircle(tex, 32, 32, 8, Outline);
            Apply(tex);
            return MakeSprite(tex);
        }

        static Sprite CreateBlobSprite(Color c)
        {
            var tex = NewTexture();
            DrawCircle(tex, 32, 24, 18, c);
            DrawCircle(tex, 28, 38, 10, c);
            DrawCircle(tex, 36, 38, 10, c);
            DrawOutlineCircle(tex, 32, 24, 18, Outline);
            Apply(tex);
            return MakeSprite(tex);
        }

        static Sprite CreateHeartSprite(Color c)
        {
            var tex = NewTexture();
            DrawCircle(tex, 24, 40, 10, c);
            DrawCircle(tex, 40, 40, 10, c);
            DrawTriangle(tex, 32, 14, 14, 38, 50, 38, c);
            Apply(tex);
            return MakeSprite(tex);
        }

        // --- Drawing primitives ---

        static Texture2D NewTexture()
        {
            var tex = new Texture2D(SpriteSize, SpriteSize, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            var clear = new Color(0, 0, 0, 0);
            var pixels = tex.GetPixels();
            for (int i = 0; i < pixels.Length; i++) pixels[i] = clear;
            tex.SetPixels(pixels);
            return tex;
        }

        static void Apply(Texture2D tex) => tex.Apply();

        static Sprite MakeSprite(Texture2D tex)
        {
            return Sprite.Create(tex, new Rect(0, 0, SpriteSize, SpriteSize), new Vector2(0.5f, 0.5f), PixelsPerUnit);
        }

        static void DrawCircle(Texture2D tex, int cx, int cy, int r, Color c)
        {
            int rSq = r * r;
            for (int y = cy - r; y <= cy + r; y++)
                for (int x = cx - r; x <= cx + r; x++)
                    if (x >= 0 && x < SpriteSize && y >= 0 && y < SpriteSize && (x - cx) * (x - cx) + (y - cy) * (y - cy) <= rSq)
                        tex.SetPixel(x, y, c);
        }

        static void DrawOutlineCircle(Texture2D tex, int cx, int cy, int r, Color c, int thickness = 1)
        {
            int rOutSq = (r + thickness) * (r + thickness);
            int rInSq = (r - thickness) * (r - thickness);
            for (int y = cy - r - thickness; y <= cy + r + thickness; y++)
                for (int x = cx - r - thickness; x <= cx + r + thickness; x++)
                {
                    if (x < 0 || x >= SpriteSize || y < 0 || y >= SpriteSize) continue;
                    int dSq = (x - cx) * (x - cx) + (y - cy) * (y - cy);
                    if (dSq <= rOutSq && dSq >= rInSq) tex.SetPixel(x, y, c);
                }
        }

        static void DrawRect(Texture2D tex, int x0, int y0, int x1, int y1, Color c)
        {
            for (int y = Mathf.Max(0, y0); y < Mathf.Min(SpriteSize, y1); y++)
                for (int x = Mathf.Max(0, x0); x < Mathf.Min(SpriteSize, x1); x++)
                    tex.SetPixel(x, y, c);
        }

        static void DrawOutlineRect(Texture2D tex, int x0, int y0, int x1, int y1, Color c, int thickness = 1)
        {
            DrawRect(tex, x0, y0, x1, y0 + thickness, c);
            DrawRect(tex, x0, y1 - thickness, x1, y1, c);
            DrawRect(tex, x0, y0, x0 + thickness, y1, c);
            DrawRect(tex, x1 - thickness, y0, x1, y1, c);
        }

        static void DrawTriangle(Texture2D tex, int topX, int topY, int blX, int blY, int brX, int brY, Color c)
        {
            int minY = Mathf.Max(0, Mathf.Min(topY, Mathf.Min(blY, brY)));
            int maxY = Mathf.Min(SpriteSize - 1, Mathf.Max(topY, Mathf.Max(blY, brY)));
            for (int y = minY; y <= maxY; y++)
                for (int x = 0; x < SpriteSize; x++)
                    if (PointInTriangle(x, y, topX, topY, blX, blY, brX, brY))
                        tex.SetPixel(x, y, c);
        }

        static bool PointInTriangle(int px, int py, int ax, int ay, int bx, int by, int cx, int cy)
        {
            float d1 = Sign(px, py, ax, ay, bx, by);
            float d2 = Sign(px, py, bx, by, cx, cy);
            float d3 = Sign(px, py, cx, cy, ax, ay);
            bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);
            return !(hasNeg && hasPos);
        }

        static float Sign(int px, int py, int ax, int ay, int bx, int by)
        {
            return (px - bx) * (ay - by) - (ax - bx) * (py - by);
        }

        static void DrawDiamond(Texture2D tex, int cx, int cy, int hw, int hh, Color c)
        {
            for (int y = cy - hh; y <= cy + hh; y++)
                for (int x = cx - hw; x <= cx + hw; x++)
                {
                    if (x < 0 || x >= SpriteSize || y < 0 || y >= SpriteSize) continue;
                    float dx = Mathf.Abs(x - cx) / (float)hw;
                    float dy = Mathf.Abs(y - cy) / (float)hh;
                    if (dx + dy <= 1f) tex.SetPixel(x, y, c);
                }
        }

        static void DrawLine(Texture2D tex, int x0, int y0, int x1, int y1, Color c, int thickness = 1)
        {
            int dx = Mathf.Abs(x1 - x0), dy = Mathf.Abs(y1 - y0);
            int steps = Mathf.Max(dx, dy);
            if (steps == 0) { if (x0 >= 0 && x0 < SpriteSize && y0 >= 0 && y0 < SpriteSize) tex.SetPixel(x0, y0, c); return; }
            for (int i = 0; i <= steps; i++)
            {
                int x = x0 + (x1 - x0) * i / steps;
                int y = y0 + (y1 - y0) * i / steps;
                for (int ty = -thickness / 2; ty <= thickness / 2; ty++)
                    for (int tx = -thickness / 2; tx <= thickness / 2; tx++)
                        if (x + tx >= 0 && x + tx < SpriteSize && y + ty >= 0 && y + ty < SpriteSize)
                            tex.SetPixel(x + tx, y + ty, c);
            }
        }
    }
}
