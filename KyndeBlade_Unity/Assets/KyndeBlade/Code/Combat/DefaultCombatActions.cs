using UnityEngine;

namespace KyndeBlade.Combat
{
    /// <summary>Creates default CombatActions at runtime (Beginner: Mechanics with clear cause-effect).</summary>
    public static class DefaultCombatActions
    {
        public static CombatAction CreateStrike()
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike,
                ActionName = "Strike",
                Damage = 15f,
                StaminaCost = 15f,
                KyndeGenerated = 2f,
                BreakDamage = 10f
            };
            return a;
        }

        public static CombatAction CreateRest()
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Rest,
                ActionName = "Rest",
                StaminaCost = 0f
            };
            return a;
        }

        public static CombatAction CreateEscapade()
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Escapade,
                ActionName = "Escapade",
                StaminaCost = 20f,
                SuccessWindow = 1.5f
            };
            return a;
        }

        public static CombatAction CreateWard()
        {
            var a = ScriptableObject.CreateInstance<CombatAction>();
            a.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Ward,
                ActionName = "Ward",
                StaminaCost = 25f,
                SuccessWindow = 1f
            };
            return a;
        }

        public static void AddDefaultsTo(MedievalCharacter c)
        {
            if (c == null || (c.AvailableActions != null && c.AvailableActions.Count > 0)) return;
            c.AvailableActions = new System.Collections.Generic.List<CombatAction>
            {
                CreateStrike(),
                CreateRest(),
                CreateEscapade(),
                CreateWard()
            };
        }
    }
}
