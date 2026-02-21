#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace KyndeBlade
{
    /// <summary>Creates Vision2 level data including field_of_grace. Run via KyndeBlade > Create MVP Level Data (Linear).</summary>
    public static class CreateVision2LevelData
    {
        const string Vision2Path = "Assets/Resources/Data/Vision2";

        [MenuItem("KyndeBlade/Create MVP Level Data (Linear)")]
        static void CreateMVPLevelData()
        {
            EnsureFieldOfGraceExists();
            LinkYearsPassToFieldOfGrace();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        static void EnsureFieldOfGraceExists()
        {
            var loc = AssetDatabase.LoadAssetAtPath<LocationNode>($"{Vision2Path}/Loc_field_of_grace.asset");
            if (loc != null) return;

            var storyBeat = CreateOrLoadStoryBeat();
            loc = ScriptableObject.CreateInstance<LocationNode>();
            loc.LocationId = "field_of_grace";
            loc.DisplayName = "Field of Grace";
            loc.Description = "The field where one waits for Grace. She does not come.";
            loc.VisionIndex = 1;
            loc.PassusIndex = 13;
            loc.NextLocationIds = new System.Collections.Generic.List<string>();
            loc.Encounter = null;
            loc.StoryBeatOnArrival = storyBeat;
            loc.IsWaitingForGrace = true;

            if (!System.IO.Directory.Exists(Vision2Path))
                System.IO.Directory.CreateDirectory(Vision2Path);
            AssetDatabase.CreateAsset(loc, $"{Vision2Path}/Loc_field_of_grace.asset");
        }

        static StoryBeat CreateOrLoadStoryBeat()
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

            if (!System.IO.Directory.Exists(Vision2Path))
                System.IO.Directory.CreateDirectory(Vision2Path);
            AssetDatabase.CreateAsset(beat, $"{Vision2Path}/StoryBeat_FieldOfGrace.asset");
            return beat;
        }

        static void LinkYearsPassToFieldOfGrace()
        {
            var yearsPass = AssetDatabase.LoadAssetAtPath<LocationNode>($"{Vision2Path}/Loc_years_pass.asset");
            if (yearsPass == null) return;
            if (yearsPass.NextLocationIds == null)
                yearsPass.NextLocationIds = new System.Collections.Generic.List<string>();
            if (!yearsPass.NextLocationIds.Contains("field_of_grace"))
            {
                yearsPass.NextLocationIds.Add("field_of_grace");
                EditorUtility.SetDirty(yearsPass);
            }
        }
    }
}
#endif
