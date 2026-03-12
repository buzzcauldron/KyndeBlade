#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using KyndeBlade;

namespace KyndeBlade.Editor
{
    /// <summary>Creates/ensures Vision II locations and wires dialogue trees. Run via KyndeBlade > Create Vision II Level Data.</summary>
    public static class CreateVision2LevelData
    {
        const string Vision2Path = "Assets/Resources/Data/Vision2";
        const string TreePath = "Assets/Resources/Data/DialogueTrees";

        /// <summary>Called from CreateVision1LevelData when creating MVP or full level data.</summary>
        public static void Create()
        {
            EnsureDirectory();
            EnsureQuestDoWelExists();
            EnsureDongeounDepthsExists();
            EnsureYearsPassExists();
            EnsureFieldOfGraceExists();
            LinkVision2Chain();
            WireDialogueTrees();
            WireHazards();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("KyndeBlade/Create Vision II Level Data")]
        static void CreateMVPLevelData()
        {
            Create();
        }

        static void EnsureDirectory()
        {
            if (!System.IO.Directory.Exists(Vision2Path))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    AssetDatabase.CreateFolder("Assets", "Resources");
                if (!AssetDatabase.IsValidFolder("Assets/Resources/Data"))
                    AssetDatabase.CreateFolder("Assets/Resources", "Data");
                if (!AssetDatabase.IsValidFolder(Vision2Path))
                    AssetDatabase.CreateFolder("Assets/Resources/Data", "Vision2");
            }
        }

        static void EnsureQuestDoWelExists()
        {
            var loc = AssetDatabase.LoadAssetAtPath<LocationNode>($"{Vision2Path}/Loc_quest_do_wel.asset");
            if (loc != null) return;

            var beat = CreateStoryBeat("QuestDoWel",
                "What is Do-Wel? Wille begins seeking Do-Wel, learning that it means doing well in one's work and life. But the quest is difficult when poor.",
                "Narrator", 1, 8);
            var enc = CreateEncounter("QuestDoWelEncounter", "False", "LadyMede");

            loc = ScriptableObject.CreateInstance<LocationNode>();
            loc.LocationId = "quest_do_wel";
            loc.DisplayName = "Quest for Do-Wel";
            loc.Description = "Dream: The search for Do-Wel, Do-Bet, and Do-Best.";
            loc.VisionIndex = 1;
            loc.PassusIndex = 8;
            loc.PassusTitle = "Passus VIII-IX";
            loc.StoryBeatOnArrival = beat;
            loc.Encounter = enc;
            AssetDatabase.CreateAsset(loc, $"{Vision2Path}/Loc_quest_do_wel.asset");
        }

        static void EnsureDongeounDepthsExists()
        {
            var loc = AssetDatabase.LoadAssetAtPath<LocationNode>($"{Vision2Path}/Loc_dongeoun_depths.asset");
            if (loc != null) return;

            var beat = CreateStoryBeat("DongeounDepths",
                "Deeper into the dream. Scripture teacheth and Fortune tempteth. Elde creepeth ever closer. Imaginatif awaiteth those patient enough to listen.",
                "Narrator", 1, 10);
            var enc = CreateEncounter("DongeounDepthsEncounter", "Hunger");

            loc = ScriptableObject.CreateInstance<LocationNode>();
            loc.LocationId = "dongeoun_depths";
            loc.DisplayName = "Dongeoun Depths";
            loc.Description = "Dream: The deeper passages where Scripture and Fortune dwell.";
            loc.VisionIndex = 1;
            loc.PassusIndex = 10;
            loc.PassusTitle = "Passus X-XI";
            loc.StoryBeatOnArrival = beat;
            loc.Encounter = enc;
            AssetDatabase.CreateAsset(loc, $"{Vision2Path}/Loc_dongeoun_depths.asset");
        }

        static void EnsureYearsPassExists()
        {
            var loc = AssetDatabase.LoadAssetAtPath<LocationNode>($"{Vision2Path}/Loc_years_pass.asset");
            if (loc != null) return;

            var beat = CreateStoryBeat("YearsPass",
                "Years pass in the dreaming. Patience and Conscience feast while the Doctor of Divinity gorges. The search for Piers continueth.",
                "Narrator", 1, 12);
            var enc = CreateEncounter("YearsPassEncounter", "False", "Wrath");

            loc = ScriptableObject.CreateInstance<LocationNode>();
            loc.LocationId = "years_pass";
            loc.DisplayName = "Years Pass";
            loc.Description = "Dream: Time grinds onward. Patience and Conscience seek Piers.";
            loc.VisionIndex = 1;
            loc.PassusIndex = 12;
            loc.PassusTitle = "Passus XII-XIII";
            loc.StoryBeatOnArrival = beat;
            loc.Encounter = enc;
            AssetDatabase.CreateAsset(loc, $"{Vision2Path}/Loc_years_pass.asset");
        }

        static void EnsureFieldOfGraceExists()
        {
            var loc = AssetDatabase.LoadAssetAtPath<LocationNode>($"{Vision2Path}/Loc_field_of_grace.asset");
            if (loc != null) return;

            var beat = CreateOrLoadFieldOfGraceBeat();
            loc = ScriptableObject.CreateInstance<LocationNode>();
            loc.LocationId = "field_of_grace";
            loc.DisplayName = "Field of Grace";
            loc.Description = "The field where one waits for Grace. She does not come.";
            loc.VisionIndex = 1;
            loc.PassusIndex = 13;
            loc.PassusTitle = "Passus XIV";
            loc.NextLocationIds = new System.Collections.Generic.List<string>();
            loc.Encounter = null;
            loc.StoryBeatOnArrival = beat;
            loc.IsWaitingForGrace = true;
            AssetDatabase.CreateAsset(loc, $"{Vision2Path}/Loc_field_of_grace.asset");
        }

        static void LinkVision2Chain()
        {
            var qdw = AssetDatabase.LoadAssetAtPath<LocationNode>($"{Vision2Path}/Loc_quest_do_wel.asset");
            var dd = AssetDatabase.LoadAssetAtPath<LocationNode>($"{Vision2Path}/Loc_dongeoun_depths.asset");
            var yp = AssetDatabase.LoadAssetAtPath<LocationNode>($"{Vision2Path}/Loc_years_pass.asset");
            var fog = AssetDatabase.LoadAssetAtPath<LocationNode>($"{Vision2Path}/Loc_field_of_grace.asset");

            if (qdw != null && !qdw.NextLocationIds.Contains("dongeoun_depths"))
            {
                qdw.NextLocationIds.Add("dongeoun_depths");
                EditorUtility.SetDirty(qdw);
            }
            if (dd != null && !dd.NextLocationIds.Contains("years_pass"))
            {
                dd.NextLocationIds.Add("years_pass");
                EditorUtility.SetDirty(dd);
            }
            if (yp != null && !yp.NextLocationIds.Contains("field_of_grace"))
            {
                yp.NextLocationIds.Add("field_of_grace");
                EditorUtility.SetDirty(yp);
            }
        }

        static void WireDialogueTrees()
        {
            WireTree("quest_do_wel", "QuestDoWelDialogue");
            WireTree("dongeoun_depths", "DongeounDepthsDialogue");
            WireTree("years_pass", "YearsPassDialogue");
            WireTree("field_of_grace", "FieldOfGraceDialogue");
        }

        static void WireTree(string locId, string treeName)
        {
            var loc = AssetDatabase.LoadAssetAtPath<LocationNode>($"{Vision2Path}/Loc_{locId}.asset");
            var tree = AssetDatabase.LoadAssetAtPath<DialogueTreeDefinition>($"{TreePath}/{treeName}.asset");
            if (loc != null && tree != null)
            {
                loc.DialogueTreeOnArrival = tree;
                EditorUtility.SetDirty(loc);
            }
        }

        static StoryBeat CreateOrLoadFieldOfGraceBeat()
        {
            var beat = AssetDatabase.LoadAssetAtPath<StoryBeat>($"{Vision2Path}/StoryBeat_FieldOfGrace.asset");
            if (beat != null) return beat;

            beat = ScriptableObject.CreateInstance<StoryBeat>();
            beat.BeatId = "field_of_grace";
            beat.Text = "Thou hast reached the field. Now thou waitest for Grace. She does not come.";
            beat.SpeakerName = "Narrator";
            beat.DisplayDuration = 5f;
            beat.WaitForInput = true;
            beat.VisionIndex = 1;
            beat.PassusIndex = 13;
            AssetDatabase.CreateAsset(beat, $"{Vision2Path}/StoryBeat_FieldOfGrace.asset");
            return beat;
        }

        static StoryBeat CreateStoryBeat(string id, string text, string speaker, int vision, int passus)
        {
            var existing = AssetDatabase.LoadAssetAtPath<StoryBeat>($"{Vision2Path}/{id}.asset");
            if (existing != null) return existing;

            var beat = ScriptableObject.CreateInstance<StoryBeat>();
            beat.BeatId = id;
            beat.Text = text;
            beat.SpeakerName = speaker;
            beat.DisplayDuration = 5f;
            beat.WaitForInput = true;
            beat.VisionIndex = vision;
            beat.PassusIndex = passus;
            AssetDatabase.CreateAsset(beat, $"{Vision2Path}/{id}.asset");
            return beat;
        }

        static void WireHazards()
        {
            var qdwEnc = AssetDatabase.LoadAssetAtPath<EncounterConfig>($"{Vision2Path}/QuestDoWelEncounter.asset");
            if (qdwEnc != null)
            {
                AddHazard(qdwEnc, "Exhaustion", PiersHazardType.Exhaustion, 5f, 2);
                AddHazard(qdwEnc, "Hunger", PiersHazardType.Hunger, 4f, 2);
            }
            var ddEnc = AssetDatabase.LoadAssetAtPath<EncounterConfig>($"{Vision2Path}/DongeounDepthsEncounter.asset");
            if (ddEnc != null)
                AddHazard(ddEnc, "Poverty", PiersHazardType.Poverty, 3f, 1);
        }

        static void AddHazard(EncounterConfig enc, string name, PiersHazardType type, float strength, int interval)
        {
            var hazard = CreateHazardConfigs.LoadOrCreate(name, type, strength, interval);
            if (hazard != null && !enc.Hazards.Contains(hazard))
            {
                enc.Hazards.Add(hazard);
                EditorUtility.SetDirty(enc);
            }
        }

        static EncounterConfig CreateEncounter(string id, params string[] enemyTypes)
        {
            var existing = AssetDatabase.LoadAssetAtPath<EncounterConfig>($"{Vision2Path}/{id}.asset");
            if (existing != null) return existing;

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
            AssetDatabase.CreateAsset(enc, $"{Vision2Path}/{id}.asset");
            return enc;
        }
    }
}
#endif
