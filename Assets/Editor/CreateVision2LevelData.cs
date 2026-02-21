using UnityEngine;
using UnityEditor;

namespace KyndeBlade.Editor
{
    /// <summary>Creates Vision II level data (Do-Wel, Dongeoun Depths, Hunger boss, Years Pass).</summary>
    public static class CreateVision2LevelData
    {
        const string DataPath = "Assets/Resources/Data/Vision2";

        [MenuItem("KyndeBlade/Create Vision II Level Data")]
        public static void Create()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Data"))
                CreateVision1LevelData.Create();
            if (!AssetDatabase.IsValidFolder(DataPath))
                AssetDatabase.CreateFolder("Assets/Resources/Data", "Vision2");

            var questBeat = CreateStoryBeat("QuestDoWel", "What is Do-Wel? Wille begins seeking Do-Wel, learning that it means doing well in one's work and life. But the quest is difficult when poor.", "Narrator");
            var dongeounBeat = CreateStoryBeat("DongeounDepths", "Descending into the depths of poverty and despair. The spiritual quest seems impossible when you're struggling to survive.", "Narrator");
            var yearsBeat = CreateStoryBeat("YearsPass", "Years pass. The party ages. The quest remains unresolved. Work continues. Poverty persists.", "Narrator");
            var fieldOfGraceBeat = CreateStoryBeat("FieldOfGrace", "Thou hast reached the field. Now thou waitest for Grace. You wait for Grace.", "Narrator");

            var questEnc = CreateEncounter("QuestDoWelEncounter", "False", "LadyMede");
            var dongeounEnc = CreateEncounterWithBoss("DongeounDepthsEncounter", "Hunger");
            var yearsEnc = CreateEncounter("YearsPassEncounter", "False", "Wrath");

            var quest = CreateLocation("quest_do_wel", "Quest for Do-Wel", "The search for Do-Wel begins.", 1, 8, "Passus VIII-IX", questBeat, questEnc, "dongeoun_depths");
            var dongeoun = CreateLocation("dongeoun_depths", "Dongeoun Depths", "The depths of poverty. Hunger awaits.", 1, 10, "Passus X-XI", dongeounBeat, dongeounEnc, "years_pass");
            var years = CreateLocation("years_pass", "Years Pass", "Time passes. The party ages.", 1, 12, "Passus XII-XIII", yearsBeat, yearsEnc);
            var fieldOfGrace = CreateLocation("field_of_grace", "Field of Grace", "Best outcome: wait for Grace. She does not come.", 1, 14, "Passus XIV", fieldOfGraceBeat, null);
            fieldOfGrace.IsWaitingForGrace = true;
            UnityEditor.EditorUtility.SetDirty(fieldOfGrace);

            quest.NextLocationIds.Clear();
            quest.NextLocationIds.Add("dongeoun_depths");
            dongeoun.NextLocationIds.Clear();
            dongeoun.NextLocationIds.Add("years_pass");
            years.NextLocationIds.Clear();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Vision II level data created at " + DataPath);
        }

        static StoryBeat CreateStoryBeat(string id, string text, string speaker)
        {
            var beat = ScriptableObject.CreateInstance<StoryBeat>();
            beat.BeatId = id;
            beat.Text = text;
            beat.SpeakerName = speaker;
            beat.DisplayDuration = 5f;
            beat.WaitForInput = true;
            beat.VisionIndex = 1;
            AssetDatabase.CreateAsset(beat, $"{DataPath}/{id}.asset");
            return beat;
        }

        static EncounterConfig CreateEncounter(string id, params string[] enemyTypes)
        {
            var enc = ScriptableObject.CreateInstance<EncounterConfig>();
            enc.Enemies.Clear();
            var offset = new Vector3(4f, 0f, 0f);
            float spacing = 1.5f;
            for (int i = 0; enemyTypes != null && i < enemyTypes.Length; i++)
            {
                var t = enemyTypes[i];
                if (string.IsNullOrEmpty(t)) continue;
                enc.Enemies.Add(new EncounterConfig.EnemySpawnEntry
                {
                    CharacterTypeName = t,
                    Position = offset + new Vector3(0f, (i - enemyTypes.Length * 0.5f) * spacing, 0f)
                });
            }
            AssetDatabase.CreateAsset(enc, $"{DataPath}/{id}.asset");
            return enc;
        }

        static EncounterConfig CreateEncounterWithBoss(string id, string bossType)
        {
            var enc = ScriptableObject.CreateInstance<EncounterConfig>();
            enc.Enemies.Clear();
            enc.BossCharacterType = bossType;
            enc.BossPosition = new Vector3(4f, 0f, 0f);
            AssetDatabase.CreateAsset(enc, $"{DataPath}/{id}.asset");
            return enc;
        }

        static LocationNode CreateLocation(string locId, string displayName, string desc, int vision, int passus, string passusTitle, StoryBeat beat, EncounterConfig enc, params string[] nextIds)
        {
            var loc = ScriptableObject.CreateInstance<LocationNode>();
            loc.LocationId = locId;
            loc.DisplayName = displayName;
            loc.Description = desc;
            loc.VisionIndex = vision;
            loc.PassusIndex = passus;
            loc.PassusTitle = passusTitle;
            loc.StoryBeatOnArrival = beat;
            loc.Encounter = enc;
            loc.NextLocationIds.Clear();
            foreach (var id in nextIds)
                loc.NextLocationIds.Add(id);
            AssetDatabase.CreateAsset(loc, $"{DataPath}/Loc_{locId}.asset");
            return loc;
        }
    }
}
