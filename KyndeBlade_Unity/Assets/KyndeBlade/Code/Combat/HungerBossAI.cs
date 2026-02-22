using System.Collections.Generic;
using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Hunger boss AI: prefers Hunger abilities, uses Feast of Want when many players hungry.</summary>
    public class HungerBossAI : BaseBossAI
    {
        HungerCharacter _hunger;

        protected override void Start()
        {
            base.Start();
            _hunger = GetComponent<HungerCharacter>();
        }

        protected override CombatAction ChooseSpecificAction(MedievalCharacter target)
        {
            if (TurnManager == null) return null;
            var affordable = GetAffordableActions();
            if (affordable.Count == 0) return null;

            int hungryCount = 0;
            foreach (var p in TurnManager.PlayerCharacters)
                if (p != null && p.IsAlive() && p.IsHungry()) hungryCount++;

            float hpRatio = GetHpRatio();

            foreach (var a in affordable)
            {
                if (a.ActionData.ActionName == "The Feast of Want" && hungryCount >= 2 && hpRatio < 0.6f) return a;
                if (a.ActionData.ActionName == "The Unending Need" && _hunger != null && _hunger.CurrentPhase >= 2) return a;
                if (a.ActionData.ActionName == "The Empty Belly" && hungryCount < 2) return a;
                if (a.ActionData.ActionName == "Hunger's Grip" && hungryCount == 0) return a;
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
