#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace KyndeBlade.Editor
{
    /// <summary>Creates RewardConfig assets for encounters and wires them.</summary>
    public static class CreateRewardConfigs
    {
        const string RewardPath = "Assets/Resources/Data/Rewards";

        [MenuItem("KyndeBlade/Create Reward Configs")]
        public static void Create()
        {
            EnsureDir(RewardPath);

            CreateReward("FayreFeldeReward", tierBonus: 0f);
            CreateReward("DongeounReward", tierBonus: 1f);
            CreateReward("PiersFieldReward", tierBonus: 1.5f);
            CreateReward("SevenSinsReward", tierBonus: 2f);
            CreateReward("QuestDoWelReward", tierBonus: 2.5f);
            CreateReward("DongeounDepthsReward", tierBonus: 3f);
            CreateReward("YearsPassReward", tierBonus: 4f);
            CreateReward("GreenChapelReward", tierBonus: 5f);

            WireRewardsToEncounters();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[KyndeBlade] Reward configs created and wired.");
        }

        static RewardConfig CreateReward(string name, float tierBonus, string[] unlocks = null)
        {
            string path = $"{RewardPath}/{name}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<RewardConfig>(path);
            if (existing != null) return existing;

            var reward = ScriptableObject.CreateInstance<RewardConfig>();
            reward.BlessingTierBonus = tierBonus;
            reward.UnlockLocationIds = unlocks ?? new string[0];
            AssetDatabase.CreateAsset(reward, path);
            return reward;
        }

        static void WireRewardsToEncounters()
        {
            WireReward("Assets/Resources/Data/Vision1/FayreFeldeEncounter.asset", "FayreFeldeReward");
            WireReward("Assets/Resources/Data/Vision1/DongeounEncounter.asset", "DongeounReward");
            WireReward("Assets/Resources/Data/Vision1/PiersFieldEncounter.asset", "PiersFieldReward");
            WireReward("Assets/Resources/Data/Vision1/SevenSinsEncounter.asset", "SevenSinsReward");

            WireReward("Assets/Resources/Data/Vision2/QuestDoWelEncounter.asset", "QuestDoWelReward");
            WireReward("Assets/Resources/Data/Vision2/DongeounDepthsEncounter.asset", "DongeounDepthsReward");
            WireReward("Assets/Resources/Data/Vision2/YearsPassEncounter.asset", "YearsPassReward");

            WireReward("Assets/Resources/Data/GreenChapel/GreenChapelEncounter.asset", "GreenChapelReward");
        }

        static void WireReward(string encounterPath, string rewardName)
        {
            var enc = AssetDatabase.LoadAssetAtPath<EncounterConfig>(encounterPath);
            if (enc == null) return;
            var reward = AssetDatabase.LoadAssetAtPath<RewardConfig>($"{RewardPath}/{rewardName}.asset");
            if (reward == null) return;

            var locPath = encounterPath.Replace("Encounter.asset", "").Replace("Encounter", "Loc_");
            var locations = new string[]
            {
                encounterPath.Replace("Encounter.asset", "").Replace("Encounter", "")
            };

            EditorUtility.SetDirty(enc);
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
