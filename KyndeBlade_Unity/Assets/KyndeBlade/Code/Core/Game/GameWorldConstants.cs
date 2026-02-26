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
        /// <summary>Tower on the Toft — first view for demo; confined, lovely, spooky, overlooking the Fair Field.</summary>
        public const string LocationTowerOnToft = "tour";
        public const string LocationFairField = "fayre_felde";
        /// <summary>Default start location for new game (demo: Tower on the Toft).</summary>
        public const string DefaultStartLocationId = "tour";
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

        /// <summary>Default parry (Ward) window in seconds. Human-oriented; GameSettings can scale for difficulty.</summary>
        public const float DefaultParryWindowSeconds = 2f;
        /// <summary>Default dodge (Escapade) window in seconds. Human-oriented; GameSettings can scale for difficulty.</summary>
        public const float DefaultDodgeWindowSeconds = 2f;

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
        public const string DefeatGreenChapelMessage = "The Grene Knyght hath taken thy hede. A forme of thee endeth in Orfeo's Othirworld.";
        public const string DefeatSinOrfeoMessage = "The synne hath rent thee. A forme of thy body endeth in Orfeo's Othirworld.";
        public const string DefeatDeathOfOldAgeMessage = "Deth of elde. Wille's yeris han runne her cours. Grace cometh not. A forme of thee endeth in Orfeo's Othirworld.";
    }
}
