using UnityEngine;
using UnityEditor;

namespace KyndeBlade.Editor
{
    public static class CreateHazardConfigs
    {
        const string HazardPath = "Assets/Resources/Data/Hazards";

        [MenuItem("KyndeBlade/Create Hazard Configs")]
        public static void Create()
        {
            EnsureDirectory(HazardPath);

            CreateHazard("Exhaustion", PiersHazardType.Exhaustion, 5f, 2);
            CreateHazard("Poverty", PiersHazardType.Poverty, 3f, 1);
            CreateHazard("Labor", PiersHazardType.Labor, 8f, 3);
            CreateHazard("Hunger", PiersHazardType.Hunger, 4f, 2);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[KyndeBlade] Hazard configs created at " + HazardPath);
        }

        public static PiersHazardConfig LoadOrCreate(string name, PiersHazardType type, float strength, int interval)
        {
            EnsureDirectory(HazardPath);
            var path = $"{HazardPath}/{name}Hazard.asset";
            var existing = AssetDatabase.LoadAssetAtPath<PiersHazardConfig>(path);
            if (existing != null) return existing;
            return CreateHazard(name, type, strength, interval);
        }

        static PiersHazardConfig CreateHazard(string name, PiersHazardType type, float strength, int interval)
        {
            var path = $"{HazardPath}/{name}Hazard.asset";
            var existing = AssetDatabase.LoadAssetAtPath<PiersHazardConfig>(path);
            if (existing != null) return existing;

            var config = ScriptableObject.CreateInstance<PiersHazardConfig>();
            config.Type = type;
            config.Strength = strength;
            config.IntervalTurns = interval;
            AssetDatabase.CreateAsset(config, path);
            return config;
        }

        static void EnsureDirectory(string path)
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
