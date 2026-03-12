#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace KyndeBlade.Editor
{
    /// <summary>Creates CharacterVisualConfig assets for all 14 characters with themed color overrides.</summary>
    public static class CreateCharacterVisualConfigs
    {
        const string ConfigPath = "Assets/Resources/Data/VisualConfigs";

        [MenuItem("KyndeBlade/Create Character Visual Configs")]
        public static void CreateAll()
        {
            EnsureDir(ConfigPath);

            var config = ScriptableObject.CreateInstance<CharacterVisualConfig>();
            config.Entries = new CharacterVisualConfig.Entry[]
            {
                MakeEntry("wille", new Color(0.55f, 0.45f, 0.35f), new Vector3(1f, 1f, 1f), 1f),
                MakeEntry("piers", new Color(0.45f, 0.55f, 0.35f), new Vector3(1.1f, 1.1f, 1f), 0.8f),
                MakeEntry("conscience", new Color(0.7f, 0.7f, 0.8f), new Vector3(0.9f, 1.2f, 1f), 1.2f),
                MakeEntry("false", new Color(0.6f, 0.3f, 0.3f), new Vector3(1f, 1f, 1f), 1.1f),
                MakeEntry("lady_mede", new Color(0.8f, 0.6f, 0.3f), new Vector3(0.9f, 1.1f, 1f), 0.9f),
                MakeEntry("wrath", new Color(0.8f, 0.2f, 0.2f), new Vector3(1.2f, 1.2f, 1f), 1.4f),
                MakeEntry("hunger", new Color(0.5f, 0.4f, 0.3f), new Vector3(1.3f, 0.9f, 1f), 0.7f),
                MakeEntry("pride", new Color(0.7f, 0.5f, 0.7f), new Vector3(1.1f, 1.3f, 1f), 1.0f),
                MakeEntry("green_knight", new Color(0.2f, 0.6f, 0.3f), new Vector3(1.4f, 1.4f, 1f), 0.9f),
                MakeEntry("elde", new Color(0.6f, 0.6f, 0.6f), new Vector3(1f, 1.2f, 1f), 0.6f),
                MakeEntry("envy", new Color(0.3f, 0.5f, 0.3f), new Vector3(1f, 1f, 1f), 1.1f),
                MakeEntry("sloth", new Color(0.5f, 0.5f, 0.4f), new Vector3(1.3f, 0.8f, 1f), 0.5f),
                MakeEntry("lust", new Color(0.7f, 0.3f, 0.4f), new Vector3(0.9f, 1.1f, 1f), 1.3f),
                MakeEntry("generic_enemy", new Color(0.4f, 0.35f, 0.3f), new Vector3(1f, 1f, 1f), 1f),
            };

            string path = $"{ConfigPath}/AllCharacterVisuals.asset";
            var existing = AssetDatabase.LoadAssetAtPath<CharacterVisualConfig>(path);
            if (existing != null)
            {
                existing.Entries = config.Entries;
                EditorUtility.SetDirty(existing);
            }
            else
            {
                AssetDatabase.CreateAsset(config, path);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[KyndeBlade] Created CharacterVisualConfig with {config.Entries.Length} entries at {path}");
        }

        static CharacterVisualConfig.Entry MakeEntry(string key, Color color, Vector3 scale, float animSpeed)
        {
            return new CharacterVisualConfig.Entry
            {
                CharacterKey = key,
                ColorOverride = color,
                ScaleOverride = scale,
                AnimationSpeed = animSpeed
            };
        }

        static void EnsureDir(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
#endif
