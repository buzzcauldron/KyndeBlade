using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Aggressive AI: uses Burning Rage when HP is high, Wrath's Storm vs multiple targets.</summary>
    public class WrathBossAI : BaseBossAI
    {
        bool _hasRaged;

        protected override CombatAction ChooseSpecificAction(MedievalCharacter target)
        {
            if (TurnManager == null) return null;
            var affordable = GetAffordableActions();
            if (affordable.Count == 0) return null;

            float hpRatio = GetHpRatio();
            int aliveCount = GetAlivePlayerCount();

            if (!_hasRaged && hpRatio > 0.5f)
            {
                foreach (var a in affordable)
                    if (a.ActionData.ActionName == "Burning Rage") { _hasRaged = true; return a; }
            }

            if (aliveCount >= 2)
            {
                foreach (var a in affordable)
                    if (a.ActionData.ActionName == "Wrath's Storm") return a;
            }

            foreach (var a in affordable)
                if (a.ActionData.ActionName == "Blind Fury") return a;

            foreach (var a in affordable)
                if (a.ActionData.ActionType == CombatActionType.Strike) return a;

            foreach (var a in affordable)
                if (a.ActionData.ActionType == CombatActionType.Rest) return a;

            return affordable[0];
        }
    }
}
