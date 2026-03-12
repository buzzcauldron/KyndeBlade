using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Runtime debug overlay showing current location, active blessings,
    /// status effects, and save state. Toggle with F1.</summary>
    public class DebugOverlay : MonoBehaviour
    {
        bool _visible;
        GUIStyle _style;

        void Update()
        {
            if (UnityEngine.InputSystem.Keyboard.current != null &&
                UnityEngine.InputSystem.Keyboard.current.f1Key.wasPressedThisFrame)
                _visible = !_visible;
        }

        void OnGUI()
        {
            if (!_visible) return;

            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.box);
                _style.fontSize = 12;
                _style.alignment = TextAnchor.UpperLeft;
                _style.normal.textColor = Color.white;
                _style.padding = new RectOffset(6, 6, 4, 4);
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<b>DEBUG OVERLAY (F1 to toggle)</b>");
            sb.AppendLine();

            var save = GameRuntime.SaveManager ?? Object.FindFirstObjectByType<SaveManager>();
            if (save?.CurrentProgress != null)
            {
                var p = save.CurrentProgress;
                sb.AppendLine($"Location: {p.CurrentLocationId}");
                sb.AppendLine($"Vision: {p.VisionIndex}  Missteps: {p.EthicalMisstepCount}");
                sb.AppendLine($"Elde Hits: {p.EldeHitsAccrued}  Hunger: {p.HasEverHadHunger}");
                sb.AppendLine($"WodeWo: stg={p.WodeWoArcStage} dead={p.WodeWoDead}");
                sb.AppendLine($"Grace: {p.HasReachedFieldOfGrace}");
            }
            else
            {
                sb.AppendLine("No save data");
            }

            sb.AppendLine();
            var tm = GameRuntime.TurnManager ?? Object.FindFirstObjectByType<TurnManager>();
            if (tm?.PlayerCharacters != null)
            {
                sb.AppendLine($"Combat: {tm.State}");
                foreach (var c in tm.PlayerCharacters)
                {
                    if (c == null) continue;
                    sb.Append($"{c.CharacterName}: {c.Stats.CurrentHealth:F0}/{c.Stats.MaxHealth:F0}HP ");
                    sb.Append($"A{c.Stats.AttackPower:F0} D{c.Stats.Defense:F0} S{c.Stats.Speed:F0} ");
                    sb.Append($"B={c.Stats.ActiveBlessings?.Count ?? 0} ");
                    foreach (var se in c.ActiveStatusEffects)
                        sb.Append($"[{se.Data.Type}:{se.Data.RemainingTime:F0}] ");
                    sb.AppendLine();
                }
            }

            float w = 360, h = 280;
            GUI.Box(new Rect(Screen.width - w - 10, 10, w, h),
                sb.ToString(), _style);
        }
    }
}
