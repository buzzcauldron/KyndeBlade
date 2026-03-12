using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Lethargic AI: opens with Heavy Yawn, targets fastest player, heals when desperate.</summary>
    public class SlothBossAI : BaseBossAI
    {
        bool _hasYawned;

        protected override MedievalCharacter ChooseTarget()
        {
            if (TurnManager == null) return null;
            MedievalCharacter fastest = null;
            float highestSpeed = float.MinValue;
            foreach (var p in TurnManager.PlayerCharacters)
            {
                if (p == null || !p.IsAlive()) continue;
                if (p.Stats.Speed > highestSpeed) { highestSpeed = p.Stats.Speed; fastest = p; }
            }
            return fastest;
        }

        protected override CombatAction ChooseSpecificAction(MedievalCharacter target)
        {
            if (TurnManager == null) return null;
            var affordable = GetAffordableActions();
            if (affordable.Count == 0) return null;

            if (!_hasYawned)
            {
                foreach (var a in affordable)
                    if (a.ActionData.ActionName == "Heavy Yawn") { _hasYawned = true; return a; }
            }

            if (GetHpRatio() < 0.4f)
            {
                foreach (var a in affordable)
                    if (a.ActionData.ActionName == "Apathy") return a;
            }

            if (target != null)
            {
                foreach (var a in affordable)
                    if (a.ActionData.ActionName == "Torpor") return a;
            }

            foreach (var a in affordable)
                if (a.ActionData.ActionName == "Lethargy's Touch") return a;

            foreach (var a in affordable)
                if (a.ActionData.ActionType == CombatActionType.Strike) return a;

            foreach (var a in affordable)
                if (a.ActionData.ActionType == CombatActionType.Rest) return a;

            return affordable[0];
        }
    }
}
