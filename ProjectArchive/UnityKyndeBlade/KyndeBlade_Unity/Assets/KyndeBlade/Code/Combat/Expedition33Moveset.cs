using System.Collections.Generic;
using UnityEngine;
using KyndeBlade;

namespace KyndeBlade.Combat
{
    /// <summary>
    /// Expedition 33–inspired moveset per docs/EXPEDITION_33_COMBAT_DESIGN.md.
    /// Melee generates Kynde, ranged/skills consume Kynde.
    /// </summary>
    public static class Expedition33Moveset
    {
        // ─── Melee (generates Kynde) ─────────────────────────────────────────
        public static CombatAction MeleeStrike()
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike,
                ActionName = "Melee Strike",
                Damage = 15f,
                StaminaCost = 15f,
                KyndeGenerated = 2f,
                BreakDamage = 10f,
                ElementType = KyndeElementType.None
            };
            return a;
        }

        public static CombatAction HeavyStrike()
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike,
                ActionName = "Heavy Strike",
                Damage = 25f,
                StaminaCost = 25f,
                KyndeGenerated = 3f,
                BreakDamage = 15f,
                ElementType = KyndeElementType.None
            };
            return a;
        }

        public static CombatAction BreakStrike()
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike,
                ActionName = "Break Strike",
                Damage = 15f,
                StaminaCost = 20f,
                KyndeGenerated = 2f,
                BreakDamage = 25f,
                ElementType = KyndeElementType.None
            };
            return a;
        }

        // ─── Ranged (consumes Kynde) ────────────────────────────────────────────
        public static CombatAction RangedStrike()
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.RangedStrike,
                ActionName = "Ranged Strike",
                Damage = 20f,
                StaminaCost = 10f,
                KyndeCost = 4f,
                BreakDamage = 5f,
                ElementType = KyndeElementType.None
            };
            return a;
        }

        public static CombatAction PiercingShot()
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.RangedStrike,
                ActionName = "Piercing Shot",
                Damage = 30f,
                StaminaCost = 15f,
                KyndeCost = 6f,
                BreakDamage = 10f,
                ElementType = KyndeElementType.None
            };
            return a;
        }

        // ─── Elemental skills (consume Kynde) ────────────────────────────────────
        public static CombatAction FlammeBolt()
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike,
                ActionName = "Flamme Bolt",
                Damage = 22f,
                StaminaCost = 0f,
                KyndeCost = 4f,
                BreakDamage = 5f,
                ElementType = KyndeElementType.Flamme
            };
            return a;
        }

        public static CombatAction FrostStrike()
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike,
                ActionName = "Frost Strike",
                Damage = 18f,
                StaminaCost = 10f,
                KyndeCost = 3f,
                BreakDamage = 15f,
                ElementType = KyndeElementType.Frost
            };
            return a;
        }

        public static CombatAction ThunderStrike()
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike,
                ActionName = "Thunder Strike",
                Damage = 25f,
                StaminaCost = 15f,
                KyndeCost = 5f,
                BreakDamage = 10f,
                ElementType = KyndeElementType.Thunder
            };
            return a;
        }

        public static CombatAction TrewtheSmite()
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike,
                ActionName = "Trewthe Smite",
                Damage = 20f,
                StaminaCost = 12f,
                KyndeCost = 4f,
                BreakDamage = 12f,
                ElementType = KyndeElementType.Trewthe
            };
            return a;
        }

        // ─── Support (consumes Kynde) ───────────────────────────────────────────
        public static CombatAction KyndeHeal()
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Heal,
                ActionName = "Kynde Heal",
                Damage = 25f,
                StaminaCost = 0f,
                KyndeCost = 5f,
                ElementType = KyndeElementType.Kynde
            };
            return a;
        }

        // ─── Defense (real-time window) ─────────────────────────────────────────
        public static CombatAction Escapade()
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Escapade,
                ActionName = "Escapade",
                StaminaCost = 20f,
                SuccessWindow = GameWorldConstants.DefaultDodgeWindowSeconds,
                ElementType = KyndeElementType.None
            };
            return a;
        }

        public static CombatAction Ward()
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Ward,
                ActionName = "Ward",
                StaminaCost = 25f,
                SuccessWindow = GameWorldConstants.DefaultParryWindowSeconds,
                ElementType = KyndeElementType.None
            };
            return a;
        }

        // ─── Counter (after parry) ─────────────────────────────────────────────
        public static CombatAction Counter()
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Counter,
                ActionName = "Counter",
                Damage = 20f,
                StaminaCost = 0f,
                KyndeCost = 0f,
                BreakDamage = 15f,
                ElementType = KyndeElementType.None
            };
            return a;
        }

        // ─── Rest ──────────────────────────────────────────────────────────────
        public static CombatAction Rest()
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Rest,
                ActionName = "Rest",
                StaminaCost = 0f,
                ElementType = KyndeElementType.None
            };
            return a;
        }

        // ─── Presets ───────────────────────────────────────────────────────────
        /// <summary>Basic set: melee, ranged, defense, rest.</summary>
        public static List<CombatAction> BasicSet()
        {
            return new List<CombatAction>
            {
                MeleeStrike(),
                RangedStrike(),
                Escapade(),
                Ward(),
                Rest()
            };
        }

        /// <summary>Full Expedition 33 moveset.</summary>
        public static List<CombatAction> FullSet()
        {
            return new List<CombatAction>
            {
                MeleeStrike(),
                HeavyStrike(),
                BreakStrike(),
                RangedStrike(),
                PiercingShot(),
                FlammeBolt(),
                FrostStrike(),
                ThunderStrike(),
                TrewtheSmite(),
                KyndeHeal(),
                Escapade(),
                Ward(),
                Counter(),
                Rest()
            };
        }

        /// <summary>Knight: melee-focused, break, ward.</summary>
        public static List<CombatAction> KnightSet()
        {
            return new List<CombatAction>
            {
                MeleeStrike(),
                HeavyStrike(),
                BreakStrike(),
                Ward(),
                Rest()
            };
        }

        /// <summary>Mage: elemental, heal, ranged.</summary>
        public static List<CombatAction> MageSet()
        {
            return new List<CombatAction>
            {
                MeleeStrike(),
                FlammeBolt(),
                FrostStrike(),
                ThunderStrike(),
                KyndeHeal(),
                Escapade(),
                Rest()
            };
        }

        /// <summary>Archer: ranged, escapade.</summary>
        public static List<CombatAction> ArcherSet()
        {
            return new List<CombatAction>
            {
                MeleeStrike(),
                RangedStrike(),
                PiercingShot(),
                Escapade(),
                Rest()
            };
        }

        /// <summary>Rogue: strike, break, counter, escapade.</summary>
        public static List<CombatAction> RogueSet()
        {
            return new List<CombatAction>
            {
                MeleeStrike(),
                BreakStrike(),
                TrewtheSmite(),
                Escapade(),
                Counter(),
                Rest()
            };
        }

        /// <summary>Enemy set: melee, break, defense, rest (no heal/ranged).</summary>
        public static List<CombatAction> EnemySet()
        {
            return new List<CombatAction>
            {
                MeleeStrike(),
                HeavyStrike(),
                BreakStrike(),
                Escapade(),
                Ward(),
                Rest()
            };
        }

        /// <summary>Apply Expedition 33 moveset by character class.</summary>
        public static void ApplyToCharacter(MedievalCharacter c, bool isEnemy = false)
        {
            if (c == null) return;
            if (isEnemy)
            {
                c.AvailableActions = EnemySet();
                return;
            }
            switch (c.CharacterClassType)
            {
                case CharacterClass.Knight: c.AvailableActions = KnightSet(); break;
                case CharacterClass.Mage: c.AvailableActions = MageSet(); break;
                case CharacterClass.Archer: c.AvailableActions = ArcherSet(); break;
                case CharacterClass.Rogue: c.AvailableActions = RogueSet(); break;
                default: c.AvailableActions = BasicSet(); break;
            }
        }
    }
}
