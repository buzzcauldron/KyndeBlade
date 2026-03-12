using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Elde AI: spreads aging evenly across players. Uses Years' Weight once all players are aged.</summary>
    public class EldeBossAI : BaseBossAI
    {
        /// <summary>Targets the player with the fewest Age stacks to spread aging evenly.</summary>
        protected override MedievalCharacter ChooseTarget()
        {
            if (TurnManager == null) return null;
            MedievalCharacter best = null;
            int lowestTier = int.MaxValue;
            foreach (var p in TurnManager.PlayerCharacters)
            {
                if (p == null || !p.IsAlive()) continue;
                var ageEffect = p.GetStatusEffect(StatusEffectType.Age);
                int tier = ageEffect != null ? ageEffect.Data.StackCount : 0;
                if (tier < lowestTier) { lowestTier = tier; best = p; }
            }
            return best;
        }

        protected override CombatAction ChooseSpecificAction(MedievalCharacter target)
        {
            if (TurnManager == null) return null;
            var affordable = GetAffordableActions();
            if (affordable.Count == 0) return null;

            bool allAged = AllPlayersHaveAge();

            if (allAged)
            {
                foreach (var a in affordable)
                    if (a.ActionData.ActionName == "Years' Weight") return a;
            }

            foreach (var a in affordable)
                if (a.ActionData.ActionName == "Elde's Touch") return a;

            foreach (var a in affordable)
                if (a.ActionData.ActionType == CombatActionType.Strike) return a;

            foreach (var a in affordable)
                if (a.ActionData.ActionType == CombatActionType.Rest) return a;

            return affordable[0];
        }

        bool AllPlayersHaveAge()
        {
            if (TurnManager == null) return false;
            foreach (var p in TurnManager.PlayerCharacters)
            {
                if (p == null || !p.IsAlive()) continue;
                if (!p.HasStatusEffect(StatusEffectType.Age)) return false;
            }
            return true;
        }
    }
}
