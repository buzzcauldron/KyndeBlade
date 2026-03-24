#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace KyndeBlade.Editor
{
    public static class CreateBalanceConfig
    {
        const string Path = "Assets/Resources/BalanceConfig.asset";

        [MenuItem("KyndeBlade/Create Balance Config")]
        public static void Create()
        {
            var existing = AssetDatabase.LoadAssetAtPath<BalanceConfig>(Path);
            if (existing != null)
            {
                Debug.Log("[KyndeBlade] BalanceConfig already exists at " + Path);
                Selection.activeObject = existing;
                return;
            }

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            var config = ScriptableObject.CreateInstance<BalanceConfig>();
            AssetDatabase.CreateAsset(config, Path);
            AssetDatabase.SaveAssets();
            Debug.Log("[KyndeBlade] BalanceConfig created at " + Path);
            Selection.activeObject = config;
        }
    }
}
#endif
