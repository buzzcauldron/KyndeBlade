using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade.Editor
{
    /// <summary>Helper to create default CombatAction assets via Assets > Create > KyndeBlade.</summary>
    public static class CombatActionCreator
    {
        [UnityEditor.MenuItem("Assets/Create/KyndeBlade/Strike Action")]
        static void CreateStrike()
        {
            var action = ScriptableObject.CreateInstance<CombatAction>();
            action.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Strike,
                ActionName = "Sothfastnesse's Stroke",
                Damage = 15f,
                StaminaCost = 15f,
                KyndeGenerated = 2f,
                BreakDamage = 10f
            };
            CreateAsset(action, "StrikeAction");
        }

        [UnityEditor.MenuItem("Assets/Create/KyndeBlade/Rest Action")]
        static void CreateRest()
        {
            var action = ScriptableObject.CreateInstance<CombatAction>();
            action.ActionData = new CombatActionData
            {
                ActionType = CombatActionType.Rest,
                ActionName = "Kynde's Rest",
                StaminaCost = 0f
            };
            CreateAsset(action, "RestAction");
        }

        static void CreateAsset(ScriptableObject obj, string name)
        {
            string path = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject);
            if (string.IsNullOrEmpty(path)) path = "Assets";
            if (!System.IO.Directory.Exists(path)) path = System.IO.Path.GetDirectoryName(path);
            path = System.IO.Path.Combine(path, name + ".asset");
            UnityEditor.AssetDatabase.CreateAsset(obj, path);
            UnityEditor.AssetDatabase.SaveAssets();
        }
    }
}
