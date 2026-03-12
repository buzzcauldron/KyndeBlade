using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Lust AI: sustain with Siren's Kiss when low, AoE when many players, default to Allure.</summary>
    public class LustBossAI : BaseBossAI
    {
        protected override CombatAction ChooseSpecificAction(MedievalCharacter target)
        {
            if (TurnManager == null) return null;
            var affordable = GetAffordableActions();
            if (affordable.Count == 0) return null;

            float hpRatio = GetHpRatio();
            int aliveCount = GetAlivePlayerCount();

            if (hpRatio < 0.6f)
            {
                foreach (var a in affordable)
                    if (a.ActionData.ActionName == "Siren's Kiss") return a;
            }

            if (aliveCount >= 3)
            {
                foreach (var a in affordable)
                    if (a.ActionData.ActionName == "Passion's Embrace") return a;
            }

            foreach (var a in affordable)
                if (a.ActionData.ActionName == "Allure") return a;

            foreach (var a in affordable)
                if (a.ActionData.ActionType == CombatActionType.Strike) return a;

            foreach (var a in affordable)
                if (a.ActionData.ActionType == CombatActionType.Rest) return a;

            return affordable[0];
        }
    }
}
