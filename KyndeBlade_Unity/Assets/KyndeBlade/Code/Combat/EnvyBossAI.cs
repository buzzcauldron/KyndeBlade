using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Envy AI: targets blessed characters, steals blessings, weakens the party.</summary>
    public class EnvyBossAI : BaseBossAI
    {
        /// <summary>Targets the player with the most active blessings; falls back to lowest HP.</summary>
        protected override MedievalCharacter ChooseTarget()
        {
            if (TurnManager == null) return null;
            MedievalCharacter best = null;
            int mostBlessings = -1;
            float lowestHp = float.MaxValue;
            foreach (var p in TurnManager.PlayerCharacters)
            {
                if (p == null || !p.IsAlive()) continue;
                int count = p.Stats.ActiveBlessings != null ? p.Stats.ActiveBlessings.Count : 0;
                if (count > mostBlessings || (count == mostBlessings && p.GetCurrentHealth() < lowestHp))
                {
                    mostBlessings = count;
                    lowestHp = p.GetCurrentHealth();
                    best = p;
                }
            }
            return best;
        }

        protected override CombatAction ChooseSpecificAction(MedievalCharacter target)
        {
            if (TurnManager == null) return null;
            var affordable = GetAffordableActions();
            if (affordable.Count == 0) return null;

            int aliveCount = GetAlivePlayerCount();
            bool targetHasBlessings = target != null
                && target.Stats.ActiveBlessings != null
                && target.Stats.ActiveBlessings.Count > 0;

            if (targetHasBlessings)
            {
                foreach (var a in affordable)
                    if (a.ActionData.ActionName == "Green-Eyed Theft") return a;
            }

            if (aliveCount >= 2)
            {
                foreach (var a in affordable)
                    if (a.ActionData.ActionName == "Bitter Comparison") return a;
            }

            foreach (var a in affordable)
                if (a.ActionData.ActionName == "Covetous Gaze") return a;

            foreach (var a in affordable)
                if (a.ActionData.ActionName == "Jealous Spite") return a;

            foreach (var a in affordable)
                if (a.ActionData.ActionType == CombatActionType.Strike) return a;

            foreach (var a in affordable)
                if (a.ActionData.ActionType == CombatActionType.Rest) return a;

            return affordable[0];
        }
    }
}
