using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Strategic AI: targets highest-Kynde player, uses Bribe on key targets, Corruption's Embrace for AoE debuff.</summary>
    public class LadyMedeBossAI : BaseBossAI
    {
        bool _hasDebuffed;

        protected override CombatAction ChooseSpecificAction(MedievalCharacter target)
        {
            if (TurnManager == null) return null;
            var affordable = GetAffordableActions();
            if (affordable.Count == 0) return null;

            int aliveCount = GetAlivePlayerCount();
            float hpRatio = GetHpRatio();

            if (!_hasDebuffed && aliveCount >= 2)
            {
                foreach (var a in affordable)
                    if (a.ActionData.ActionName == "Corruption's Embrace") { _hasDebuffed = true; return a; }
            }

            if (target != null && target.GetCurrentStamina() > 60f)
            {
                foreach (var a in affordable)
                    if (a.ActionData.ActionName == "Bribe") return a;
            }

            if (target != null && target.GetCurrentKynde() >= 3f)
            {
                foreach (var a in affordable)
                    if (a.ActionData.ActionName == "Mede's Demand") return a;
            }

            foreach (var a in affordable)
                if (a.ActionData.ActionName == "Golden Touch") return a;

            foreach (var a in affordable)
                if (a.ActionData.ActionType == CombatActionType.Strike) return a;

            foreach (var a in affordable)
                if (a.ActionData.ActionType == CombatActionType.Rest) return a;

            return affordable[0];
        }

        protected override MedievalCharacter ChooseTarget()
        {
            if (TurnManager == null) return null;
            MedievalCharacter best = null;
            float highestKynde = -1f;
            foreach (var p in TurnManager.PlayerCharacters)
            {
                if (p == null || !p.IsAlive()) continue;
                if (p.GetCurrentKynde() > highestKynde) { highestKynde = p.GetCurrentKynde(); best = p; }
            }
            return best ?? base.ChooseTarget();
        }
    }
}
