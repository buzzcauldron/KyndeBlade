using UnityEngine;
using System.Collections;

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

            var save = (GetRuntimeProperty("SaveManager") as SaveManager) ?? UnityEngine.Object.FindFirstObjectByType<SaveManager>();
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
            var tm = GetRuntimeProperty("TurnManager");
            var players = tm != null ? GetEnumerableProperty(tm, "PlayerCharacters") : null;
            if (tm != null && players != null)
            {
                sb.AppendLine($"Combat: {GetProperty(tm, "State")}");
                foreach (var c in players)
                {
                    if (c == null) continue;
                    var stats = GetProperty(c, "Stats");
                    sb.Append($"{GetProperty(c, "CharacterName")}: {ToFloat(GetProperty(stats, "CurrentHealth")):F0}/{ToFloat(GetProperty(stats, "MaxHealth")):F0}HP ");
                    sb.Append($"A{ToFloat(GetProperty(stats, "AttackPower")):F0} D{ToFloat(GetProperty(stats, "Defense")):F0} S{ToFloat(GetProperty(stats, "Speed")):F0} ");
                    sb.Append($"B={CountEnumerable(GetEnumerableProperty(stats, "ActiveBlessings"))} ");
                    var effects = GetEnumerableProperty(c, "ActiveStatusEffects");
                    if (effects != null)
                    {
                        foreach (var se in effects)
                        {
                            var data = GetProperty(se, "Data");
                            sb.Append($"[{GetProperty(data, "EffectType")}:{ToFloat(GetProperty(data, "RemainingTime")):F0}] ");
                        }
                    }
                    sb.AppendLine();
                }
            }

            float w = 360, h = 280;
            GUI.Box(new Rect(Screen.width - w - 10, 10, w, h),
                sb.ToString(), _style);
        }

        static object GetRuntimeProperty(string propertyName)
        {
            var gameRuntimeType = FindType("KyndeBlade.GameRuntime");
            if (gameRuntimeType == null) return null;
            var prop = gameRuntimeType.GetProperty(propertyName);
            if (prop == null) return null;
            return prop.GetValue(null);
        }

        static object GetProperty(object target, string propertyName)
        {
            if (target == null) return null;
            var prop = target.GetType().GetProperty(propertyName);
            return prop != null ? prop.GetValue(target) : null;
        }

        static IEnumerable GetEnumerableProperty(object target, string propertyName)
        {
            return GetProperty(target, propertyName) as IEnumerable;
        }

        static int CountEnumerable(IEnumerable e)
        {
            if (e == null) return 0;
            int count = 0;
            foreach (var _ in e) count++;
            return count;
        }

        static float ToFloat(object value)
        {
            if (value == null) return 0f;
            try { return System.Convert.ToSingle(value); }
            catch { return 0f; }
        }

        static System.Type FindType(string fullName)
        {
            foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = asm.GetType(fullName);
                if (type != null) return type;
            }
            return null;
        }
    }
}
