using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Tricky AI: alternates Mirror Image and attacks, uses False Promise when low HP.</summary>
    public class FalseBossAI : BaseBossAI
    {
        int _turnCount;

        protected override CombatAction ChooseSpecificAction(MedievalCharacter target)
        {
            if (TurnManager == null) return null;
            var affordable = GetAffordableActions();
            if (affordable.Count == 0) return null;

            float hpRatio = GetHpRatio();
            _turnCount++;

            if (hpRatio < 0.35f)
            {
                foreach (var a in affordable)
                    if (a.ActionData.ActionName == "False Promise") return a;
            }

            if (_turnCount % 3 == 1)
            {
                foreach (var a in affordable)
                    if (a.ActionData.ActionName == "Mirror Image") return a;
            }

            if (target != null && target.GetCurrentKynde() >= 3f)
            {
                foreach (var a in affordable)
                    if (a.ActionData.ActionName == "Betrayer's Kiss") return a;
            }

            foreach (var a in affordable)
                if (a.ActionData.ActionName == "Hollow Strike") return a;

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
