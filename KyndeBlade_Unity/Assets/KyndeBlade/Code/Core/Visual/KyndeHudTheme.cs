using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Optional HUD palette (Phase B). Assign on CombatUI / map UI; when null, <see cref="ManuscriptUITheme"/> static colors are used.</summary>
    [CreateAssetMenu(fileName = "KyndeHudTheme", menuName = "KyndeBlade/UI/HUD Theme")]
    public class KyndeHudTheme : ScriptableObject
    {
        [Header("Text")]
        public Color GoalTextColor = ManuscriptUITheme.InkPrimary;
        public Color StateTextColor = ManuscriptUITheme.InkSecondary;
        public Color AccentTextColor = ManuscriptUITheme.Gold;

        [Header("Buttons / panels")]
        public Color ButtonNormalColor = ManuscriptUITheme.ParchmentAged;
        public Color PanelTint = ManuscriptUITheme.ParchmentLight;
    }
}
