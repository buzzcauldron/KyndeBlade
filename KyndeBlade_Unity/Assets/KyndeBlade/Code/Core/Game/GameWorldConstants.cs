namespace KyndeBlade
{
    /// <summary>Canonical world and gameplay constants for consistent references and ideal gameplay (Expedition 33–style depth). Use everywhere instead of magic strings/numbers.</summary>
    public static class GameWorldConstants
    {
        // ─── Characters (display names, IDs) ───────────────────────────────────
        public const string PlayerDreamerName = "Wille";
        public const string GreenKnightDisplayName = "The Green Knight";
        public const string PilgrimDisplayName = "The Pilgrim";

        // ─── Location IDs (save, transitions, narrative) ───────────────────────
        public const string LocationMalvern = "malvern";
        public const string LocationGreenChapel = "green_chapel";
        public const string LocationOtherworld = "otherworld";
        public const string LocationFieldOfGrace = "field_of_grace";
        public const string LocationYearsPass = "years_pass";

        // ─── Combat (Expedition 33–aligned) ────────────────────────────────────
        /// <summary>Damage multiplier when target is broken (stun). Expedition 33: 50% more.</summary>
        public const float BrokenDamageMultiplier = 1.5f;
        /// <summary>Seconds the target is stunned when break gauge is depleted.</summary>
        public const float BreakStunDurationSeconds = 2f;
        /// <summary>Default max Kynde (nature/spirit resource). Expedition 33: 10.</summary>
        public const float DefaultMaxKynde = 10f;
        /// <summary>Damage variance: final damage *= Random(1 - Variance, 1 + Variance).</summary>
        public const float DamageVarianceHalfRange = 0.05f;
        /// <summary>Defense mitigation factor (defense * this reduces damage).</summary>
        public const float DefenseMitigationFactor = 0.5f;
        /// <summary>Minimum damage after formula (never 0).</summary>
        public const float MinimumDamage = 1f;

        // ─── Elemental (Expedition 33: weakness 2x, resistance 0.5x). Used by CombatCalculator when affinity is wired. ────────────────────────────────────────────────────────────────
        public const float ElementalWeaknessMultiplier = 2f;
        public const float ElementalResistanceMultiplier = 0.5f;

        // ─── UI (goal, state, defeat messages) ────────────────────────────────
        public const string GoalDefeatAllEnemies = "Defeat all enemies";
        public const string GoalYourTurnSelectAction = "Your turn — select an action";
        public const string GoalEnemyTurnPrepare = "Enemy turn — prepare to dodge or parry";
        public const string GoalVictory = "Victory!";
        public const string GoalDefeat = "Defeat...";
        public const string StateSelectAction = "Select action";
        public const string StateExecuting = "Executing...";
        public const string StateResolving = "Resolving...";
        public const string StateCombatEnded = "Combat ended";
        public const string DefeatGreenChapelMessage = "The Green Knight hath taken thy head. A form of thee ends in Orfeo's Otherworld.";
        public const string DefeatSinOrfeoMessage = "The sin hath rent thee. A form of thy body ends in Orfeo's Otherworld.";
        public const string DefeatDeathOfOldAgeMessage = "Death of old age. Wille's years have run their course. Grace did not come. A form of thee ends in Orfeo's Otherworld.";
    }
}
