using System;
using System.Collections.Generic;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Boss "script" for state-driven dialogue: arc by phase (entry / midpoint / desperation / defeat) and barks per action. Keep lines close to the poems (Piers Plowman, Sir Gawain, medieval tradition).</summary>
    [CreateAssetMenu(fileName = "NewBossScript", menuName = "KyndeBlade/Boss Dialogue Script")]
    public class BossDialogueScript : ScriptableObject
    {
        [Header("Dramatic arc (phase triggers)")]
        [Tooltip("Greeting – arrogance & warning. Play on first boss turn.")]
        [TextArea(2, 4)]
        public string EntryLine = "What knight so knokkez at my gate? / I bid thee welcum to this place.";
        [Tooltip("Escalation – frustration & revelation. Trigger when hpRatio < 0.6 (or phase 2).")]
        [TextArea(2, 4)]
        public string MidPointLine = "Thy blade is kene, but thy soule is thyn. / Now se what grace may wynne.";
        [Tooltip("Desperation – truth & climax. Trigger when hpRatio < 0.2 (or phase 3).")]
        [TextArea(2, 4)]
        public string DefeatLine = "The hunte … lasteth ever. / I wende we mete eft in grene.";
        [Tooltip("Boss defeated. Play when character is defeated.")]
        [TextArea(2, 4)]
        public string BossDefeatedLine = "So hatz he lyved that no wye wiste / with horled werk of his hande.";

        [Header("Action barks (match CombatAction.ActionName)")]
        [Tooltip("When the boss uses this action, show this line during telegraph. ActionName must match exactly (e.g. Beheading Blow, The Tower).")]
        public List<ActionLine> ActionBarks = new List<ActionLine>();

        [Serializable]
        public struct ActionLine
        {
            [Tooltip("Must match the action's ActionData.ActionName (e.g. Beheading Blow, Pride's Fall).")]
            public string ActionName;
            [TextArea(1, 3)]
            [Tooltip("Short line from the poem or in that voice. Shown during wind-up.")]
            public string Dialogue;
        }

        /// <summary>Get dialogue for an action by name. Returns null if not found.</summary>
        public string GetBarkForAction(string actionName)
        {
            if (string.IsNullOrEmpty(actionName) || ActionBarks == null) return null;
            foreach (var a in ActionBarks)
                if (string.Equals(a.ActionName, actionName, StringComparison.OrdinalIgnoreCase))
                    return a.Dialogue;
            return null;
        }
    }
}
