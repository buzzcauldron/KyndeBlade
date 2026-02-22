using UnityEngine;
using UnityEngine.UI;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Lindisfarne-style drop cap for boss names: first letter large and decorative, color by health (BossPalette). Use for boss nameplate or when boss speaks.</summary>
    [RequireComponent(typeof(RectTransform))]
    public class DropCapBossName : MonoBehaviour
    {
        [Header("References")]
        public Text DropCapText;
        public Text RestOfNameText;
        [Tooltip("Optional. Used to color drop cap by health (full = secondary, low = primary).")]
        public BossPalette Palette;
        [Tooltip("Character to show name for; health drives drop cap color.")]
        public MedievalCharacter Character;

        [Header("Drop Cap")]
        [Tooltip("Font size for the first letter (e.g. 48).")]
        public int DropCapFontSize = 48;
        [Tooltip("Font size for the rest of the name (e.g. 24).")]
        public int RestFontSize = 24;

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

        /// <summary>Set boss name and optionally character (for health-based color).</summary>
        public void SetBossName(string bossName, MedievalCharacter character = null)
        {
            Character = character;
            if (Character != null)
                Character.OnHealthChanged += OnHealthChanged;

            if (string.IsNullOrEmpty(bossName))
            {
                if (DropCapText != null) DropCapText.text = "";
                if (RestOfNameText != null) RestOfNameText.text = "";
                return;
            }

            string first = bossName.Substring(0, 1).ToUpperInvariant();
            string rest = bossName.Length > 1 ? bossName.Substring(1) : "";

            if (DropCapText != null)
            {
                DropCapText.text = first;
                DropCapText.fontSize = DropCapFontSize;
            }
            if (RestOfNameText != null)
            {
                RestOfNameText.text = rest;
                RestOfNameText.fontSize = RestFontSize;
            }

            Refresh();
        }

        void Refresh()
        {
            if (DropCapText == null || Palette == null) return;

            float t = 1f;
            string name = Character != null ? Character.CharacterName : "";
            if (Character != null && Character.IsAlive())
            {
                float max = Character.GetMaxHealth();
                t = max > 0f ? Mathf.Clamp01(Character.GetCurrentHealth() / max) : 0f;
            }

            Color primary = Palette.GetPrimaryForBoss(name);
            Color secondary = Palette.GetSecondaryForBoss(name);
            DropCapText.color = Color.Lerp(primary, secondary, t);
        }
    }
}
