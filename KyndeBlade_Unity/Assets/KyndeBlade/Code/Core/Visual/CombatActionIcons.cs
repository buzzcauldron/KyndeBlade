using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>
    /// Generates simple procedural icons for combat action types.
    /// Returns a small Sprite suitable for UI buttons.
    /// </summary>
    public static class CombatActionIcons
    {
        const int Size = 32;
        const float PPU = 32f;

        public static Sprite GetIcon(CombatActionType type)
        {
            switch (type)
            {
                case CombatActionType.Strike:       return SwordIcon();
                case CombatActionType.RangedStrike: return ArrowIcon();
                case CombatActionType.Escapade:     return BootIcon();
                case CombatActionType.Ward:         return ShieldIcon();
                case CombatActionType.Counter:      return CounterIcon();
                case CombatActionType.Special:      return StarIcon();
                case CombatActionType.Heal:         return CrossIcon();
                case CombatActionType.Rest:         return RestIcon();
                default:                            return StarIcon();
            }
        }

        static Texture2D NewTex()
        {
            var tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            var clear = new Color(0, 0, 0, 0);
            var px = tex.GetPixels();
            for (int i = 0; i < px.Length; i++) px[i] = clear;
            tex.SetPixels(px);
            return tex;
        }

        static Sprite Finish(Texture2D tex)
        {
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, Size, Size), Vector2.one * 0.5f, PPU);
        }

        static void FillRect(Texture2D t, int x0, int y0, int x1, int y1, Color c)
        {
            for (int y = Mathf.Max(0, y0); y < Mathf.Min(Size, y1); y++)
                for (int x = Mathf.Max(0, x0); x < Mathf.Min(Size, x1); x++)
                    t.SetPixel(x, y, c);
        }

        static void FillCircle(Texture2D t, int cx, int cy, int r, Color c)
        {
            int rr = r * r;
            for (int y = cy - r; y <= cy + r; y++)
                for (int x = cx - r; x <= cx + r; x++)
                    if (x >= 0 && x < Size && y >= 0 && y < Size && (x - cx) * (x - cx) + (y - cy) * (y - cy) <= rr)
                        t.SetPixel(x, y, c);
        }

        static Sprite SwordIcon()
        {
            var t = NewTex();
            var c = ManuscriptUITheme.InkSecondary;
            FillRect(t, 14, 4, 18, 24, c);
            FillRect(t, 10, 22, 22, 26, c);
            FillRect(t, 14, 24, 18, 28, ManuscriptUITheme.GoldDark);
            return Finish(t);
        }

        static Sprite ArrowIcon()
        {
            var t = NewTex();
            var c = ManuscriptUITheme.InkSecondary;
            FillRect(t, 6, 15, 26, 17, c);
            FillRect(t, 24, 12, 28, 20, c);
            FillRect(t, 4, 14, 8, 18, ManuscriptUITheme.Rubrication);
            return Finish(t);
        }

        static Sprite BootIcon()
        {
            var t = NewTex();
            var c = ManuscriptUITheme.InkSecondary;
            FillRect(t, 12, 4, 20, 20, c);
            FillRect(t, 8, 4, 24, 8, c);
            return Finish(t);
        }

        static Sprite ShieldIcon()
        {
            var t = NewTex();
            var c = ManuscriptUITheme.LapisBlue;
            FillCircle(t, 16, 16, 10, c);
            FillRect(t, 14, 4, 18, 26, ManuscriptUITheme.Gold);
            FillRect(t, 6, 14, 26, 18, ManuscriptUITheme.Gold);
            return Finish(t);
        }

        static Sprite CounterIcon()
        {
            var t = NewTex();
            var c = ManuscriptUITheme.Rubrication;
            FillRect(t, 6, 14, 18, 18, c);
            FillRect(t, 18, 10, 22, 22, ManuscriptUITheme.Gold);
            FillRect(t, 22, 14, 28, 18, c);
            return Finish(t);
        }

        static Sprite StarIcon()
        {
            var t = NewTex();
            var c = ManuscriptUITheme.Gold;
            FillRect(t, 14, 4, 18, 28, c);
            FillRect(t, 4, 14, 28, 18, c);
            FillRect(t, 10, 8, 22, 24, c);
            return Finish(t);
        }

        static Sprite CrossIcon()
        {
            var t = NewTex();
            var c = ManuscriptUITheme.Verdigris;
            FillRect(t, 13, 6, 19, 26, c);
            FillRect(t, 7, 16, 25, 22, c);
            return Finish(t);
        }

        static Sprite RestIcon()
        {
            var t = NewTex();
            var c = ManuscriptUITheme.ParchmentDark;
            FillCircle(t, 16, 12, 8, c);
            FillRect(t, 8, 4, 24, 8, ManuscriptUITheme.InkLight);
            return Finish(t);
        }
    }
}
