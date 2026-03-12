#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace KyndeBlade.Editor
{
    /// <summary>Master asset generation script. Calls all sub-generators in dependency order.
    /// Menu: KyndeBlade > Generate All Game Data</summary>
    public static class CreateAllGameData
    {
        [MenuItem("KyndeBlade/Generate All Game Data")]
        public static void GenerateAll()
        {
            Debug.Log("[KyndeBlade] Generating all game data...");

            CreateVision1LevelData.Create();
            CreateVision2LevelData.Create();
            CreateGreenChapelLevelData.Create();
            CreateOrfeoOtherworldLevelData.Create();

            CreatePoemDialogueTrees.GenerateAll();
            CreateBossDialogueScripts.GenerateAll();

            CreateHazardConfigs.Create();

            CreateMapPositions.Create();

            CreateCharacterPrefabs.CreateAll();

            EnsureAudioLibrary();
            EnsureBlessingPool();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[KyndeBlade] All game data generation complete.");
        }

        static void EnsureAudioLibrary()
        {
            const string path = "Assets/Resources/AudioLibrary.asset";
            if (AssetDatabase.LoadAssetAtPath<AudioLibrary>(path) != null) return;

            EnsureResourcesDir();
            var lib = ScriptableObject.CreateInstance<AudioLibrary>();
            AssetDatabase.CreateAsset(lib, path);
            Debug.Log("[KyndeBlade] Created default AudioLibrary at " + path);
        }

        static void EnsureBlessingPool()
        {
            const string path = "Assets/Resources/BlessingPool.asset";
            if (AssetDatabase.LoadAssetAtPath<BlessingPool>(path) != null) return;

            EnsureResourcesDir();
            var pool = ScriptableObject.CreateInstance<BlessingPool>();
            var defaults = BlessingPool.CreateDefaultBlessings();

            string blessingDir = "Assets/Resources/Data/Blessings";
            EnsureDir(blessingDir);

            var baked = new Blessing[defaults.Count];
            for (int i = 0; i < defaults.Count; i++)
            {
                var b = defaults[i];
                var bPath = $"{blessingDir}/{b.BlessingId}.asset";
                var existing = AssetDatabase.LoadAssetAtPath<Blessing>(bPath);
                if (existing != null)
                {
                    baked[i] = existing;
                }
                else
                {
                    AssetDatabase.CreateAsset(b, bPath);
                    baked[i] = b;
                }
            }
            pool.AllBlessings = baked;
            AssetDatabase.CreateAsset(pool, path);
            Debug.Log("[KyndeBlade] Created BlessingPool with " + baked.Length + " blessings at " + path);
        }

        static void EnsureResourcesDir()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
        }

        static void EnsureDir(string path)
        {
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
