using System.Collections.Generic;
using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Pride boss AI: prefers Pride's Fall when multiple targets, The Tower when low health.</summary>
    public class PrideBossAI : BaseBossAI
    {
        protected override CombatAction ChooseSpecificAction(MedievalCharacter target)
        {
            var affordable = GetAffordableActions();
            if (affordable.Count == 0) return null;

            int aliveCount = GetAlivePlayerCount();
            float hpRatio = GetHpRatio();

            foreach (var a in affordable)
            {
                if (a.ActionData.ActionName == "The Tower" && hpRatio < 0.4f) return a;
                if (a.ActionData.ActionName == "Pride's Fall" && aliveCount >= 2) return a;
                if (a.ActionData.ActionName == "Pride's Arrogance" && target != null) return a;
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
