using UnityEngine;
using UnityEditor;

namespace KyndeBlade.Editor
{
    /// <summary>Creates optional Green Chapel dungeon with superboss. Sir Gawain - wrong dialogue = Green Knight appears randomly.</summary>
    public static class CreateGreenChapelLevelData
    {
        const string DataPath = "Assets/Resources/Data/GreenChapel";

        [MenuItem("KyndeBlade/Create Green Chapel Level Data")]
        public static void Create()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Data"))
                AssetDatabase.CreateFolder("Assets/Resources", "Data");
            if (!AssetDatabase.IsValidFolder(DataPath))
                AssetDatabase.CreateFolder("Assets/Resources/Data", "GreenChapel");

            var choiceBeat = CreateDialogueChoiceBeat();
            var encounter = CreateEncounterWithBoss("GreenChapelEncounter", "GreenKnight");
            var loc = CreateLocation(choiceBeat, encounter);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Green Chapel level data created at " + DataPath + ". Add 'green_chapel' to a location's NextLocationIds to unlock it.");
        }

        static DialogueChoiceBeat CreateDialogueChoiceBeat()
        {
            var beat = ScriptableObject.CreateInstance<DialogueChoiceBeat>();
            beat.BeatId = "GreenChapelChallenge";
            beat.Text = "I am the Knight of the Green Chapel. I offer thee a game: strike me once, and in a year and a day I shall return the blow. Dost thou accept the covenant?";
            beat.SpeakerName = "The Green Knight";
            beat.Choices = new DialogueChoiceBeat.Choice[]
            {
                new DialogueChoiceBeat.Choice { Text = "I accept. I will meet thee at the Green Chapel.", IsCorrectChoice = true },
                new DialogueChoiceBeat.Choice { Text = "I refuse. I will not play thy game.", IsCorrectChoice = false, AssociatedSin = SinType.Pride },
                new DialogueChoiceBeat.Choice { Text = "I flee. This is no place for me.", IsCorrectChoice = false, AssociatedSin = SinType.Sloth }
            };
            AssetDatabase.CreateAsset(beat, $"{DataPath}/GreenChapelChoice.asset");
            return beat;
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

        static LocationNode CreateLocation(DialogueChoiceBeat choiceBeat, EncounterConfig enc)
        {
            var loc = ScriptableObject.CreateInstance<LocationNode>();
            loc.LocationId = "green_chapel";
            loc.DisplayName = "The Green Chapel";
            loc.Description = "Optional dungeon. The Knight of the Green Chapel awaits. Choose thy words wisely.";
            loc.VisionIndex = 1;
            loc.PassusIndex = -1;
            loc.PassusTitle = "Optional - Sir Gawain";
            loc.PreCombatChoiceBeat = choiceBeat;
            loc.Encounter = enc;
            loc.NextLocationIds.Clear();
            loc.NextLocationIds.Add("fayre_felde");
            AssetDatabase.CreateAsset(loc, $"{DataPath}/Loc_green_chapel.asset");

            var fayre = AssetDatabase.LoadAssetAtPath<LocationNode>("Assets/Resources/Data/Vision1/Loc_fayre_felde.asset");
            if (fayre != null && !fayre.NextLocationIds.Contains("green_chapel"))
            {
                fayre.NextLocationIds.Add("green_chapel");
                EditorUtility.SetDirty(fayre);
            }
            return loc;
        }
    }
}
