using UnityEngine;
using UnityEditor;

namespace KyndeBlade.Editor
{
    /// <summary>Creates Orfeo's Otherworld - tree encounter (wrong choice) leads to inescapable alternate ending. Sir Orfeo.</summary>
    public static class CreateOrfeoOtherworldLevelData
    {
        const string DataPath = "Assets/Resources/Data/OrfeoOtherworld";

        [MenuItem("KyndeBlade/Create Orfeo Otherworld Level Data")]
        public static void Create()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Data"))
                AssetDatabase.CreateFolder("Assets/Resources", "Data");
            if (!AssetDatabase.IsValidFolder(DataPath))
                AssetDatabase.CreateFolder("Assets/Resources/Data", "OrfeoOtherworld");

            var treeChoice = CreateTreeChoiceBeat();
            var treeEnc = CreateEncounter("BoundaryTreeEncounter", "False");
            var treeLoc = CreateTreeLocation(treeChoice, treeEnc);

            var otherworldBeat = CreateStoryBeat("OtherworldArrival", "The boundary between worlds dissolves. Thou hast followed the music into the Fairy Realm. There is no return.", "Narrator");
            var otherworldEnc = CreateEncounterWithBoss("OtherworldEncounter", "Pride");
            var otherworldLoc = CreateOtherworldLocation(otherworldBeat, otherworldEnc);

            var fayre = AssetDatabase.LoadAssetAtPath<LocationNode>("Assets/Resources/Data/Vision1/Loc_fayre_felde.asset");
            if (fayre != null && !fayre.NextLocationIds.Contains("boundary_tree"))
            {
                fayre.NextLocationIds.Add("boundary_tree");
                EditorUtility.SetDirty(fayre);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Orfeo Otherworld level data created. Boundary Tree reachable from Fayre Felde.");
        }

        static DialogueChoiceBeat CreateTreeChoiceBeat()
        {
            var beat = ScriptableObject.CreateInstance<DialogueChoiceBeat>();
            beat.BeatId = "BoundaryTree";
            beat.Text = "At the boundary between worlds, a tree stands. Music drifts from beyond—melancholic, otherworldly. Dost thou follow?";
            beat.SpeakerName = "Narrator";
            beat.Choices = new DialogueChoiceBeat.Choice[]
            {
                new DialogueChoiceBeat.Choice { Text = "The path is uncertain. I turn back.", IsCorrectChoice = true },
                new DialogueChoiceBeat.Choice { Text = "I follow the music into the twilight.", IsCorrectChoice = false, TransitionToLocationId = "otherworld" }
            };
            AssetDatabase.CreateAsset(beat, $"{DataPath}/BoundaryTreeChoice.asset");
            return beat;
        }

        static StoryBeat CreateStoryBeat(string id, string text, string speaker)
        {
            var beat = ScriptableObject.CreateInstance<StoryBeat>();
            beat.BeatId = id;
            beat.Text = text;
            beat.SpeakerName = speaker;
            beat.DisplayDuration = 6f;
            beat.WaitForInput = true;
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

        static LocationNode CreateTreeLocation(DialogueChoiceBeat choiceBeat, EncounterConfig enc)
        {
            var loc = ScriptableObject.CreateInstance<LocationNode>();
            loc.LocationId = "boundary_tree";
            loc.DisplayName = "The Boundary Tree";
            loc.Description = "A tree at the edge of worlds. Music drifts from beyond.";
            loc.VisionIndex = 1;
            loc.PassusIndex = -1;
            loc.PassusTitle = "Optional - Sir Orfeo";
            loc.PreCombatChoiceBeat = choiceBeat;
            loc.Encounter = enc;
            loc.NextLocationIds.Clear();
            loc.NextLocationIds.Add("fayre_felde");

            var btTree = AssetDatabase.LoadAssetAtPath<DialogueTreeDefinition>(
                "Assets/Resources/Data/DialogueTrees/BoundaryTreeDialogue.asset");
            if (btTree != null) loc.DialogueTreeOnArrival = btTree;

            AssetDatabase.CreateAsset(loc, $"{DataPath}/Loc_boundary_tree.asset");
            return loc;
        }

        static LocationNode CreateOtherworldLocation(StoryBeat beat, EncounterConfig enc)
        {
            var loc = ScriptableObject.CreateInstance<LocationNode>();
            loc.LocationId = "otherworld";
            loc.DisplayName = "Orfeo's Otherworld";
            loc.Description = "The Fairy Realm. Inescapable.";
            loc.VisionIndex = 1;
            loc.PassusIndex = -1;
            loc.PassusTitle = "Alternate Ending";
            loc.StoryBeatOnArrival = beat;
            loc.Encounter = enc;
            loc.MusicThemeOnArrival = "orfeo";
            loc.IsInescapable = true;
            loc.IsAlternateEnding = true;
            loc.NextLocationIds.Clear();

            var owTree = AssetDatabase.LoadAssetAtPath<DialogueTreeDefinition>(
                "Assets/Resources/Data/DialogueTrees/OtherworldDialogue.asset");
            if (owTree != null) loc.DialogueTreeOnArrival = owTree;

            AssetDatabase.CreateAsset(loc, $"{DataPath}/Loc_otherworld.asset");
            return loc;
        }
    }
}
