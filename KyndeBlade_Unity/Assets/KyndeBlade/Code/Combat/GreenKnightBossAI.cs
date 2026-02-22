using System.Collections.Generic;
using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Green Knight AI: prefers Beheading Blow on single target, Wild Nature's Wrath when multiple, Curse when low.</summary>
    public class GreenKnightBossAI : BaseBossAI
    {
        protected override CombatAction ChooseSpecificAction(MedievalCharacter target)
        {
            var affordable = GetAffordableActions();
            if (affordable.Count == 0) return null;

            int aliveCount = GetAlivePlayerCount();
            float hpRatio = GetHpRatio();

            foreach (var a in affordable)
            {
                if (a.ActionData.ActionName == "The Green Chapel's Curse" && hpRatio < 0.35f) return a;
                if (a.ActionData.ActionName == "Wild Nature's Wrath" && aliveCount >= 2) return a;
                if (a.ActionData.ActionName == "Beheading Blow" && target != null) return a;
            }

            foreach (var a in affordable)
            {
                if (a.ActionData.ActionType == CombatActionType.Strike && target != null) return a;
                if (a.ActionData.ActionType == CombatActionType.Special) return a;
            }

            foreach (var a in affordable)
                if (a.ActionData.ActionType == CombatActionType.Rest) return a;

            return affordable[0];
        }
    }
}
