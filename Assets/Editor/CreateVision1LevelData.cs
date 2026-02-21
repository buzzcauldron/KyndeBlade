using UnityEngine;
using UnityEditor;

namespace KyndeBlade.Editor
{
    /// <summary>Creates default Vision I level data (locations, encounters, story beats).</summary>
    public static class CreateVision1LevelData
    {
        public const string DataPath = "Assets/Resources/Data/Vision1";

        [MenuItem("KyndeBlade/Create MVP Level Data (Linear)")]
        public static void CreateMVPLinear()
        {
            CreateLinear();
            CreateVision2LevelData.Create();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("MVP linear level data created. Malvern → Fayre → Tour → Dongeoun → Piers → Seven Sins → Quest Do-Wel → Dongeoun Depths → Years Pass.");
        }

        [MenuItem("KyndeBlade/Create All Level Data (Vision I + II + Green Chapel + Orfeo)")]
        public static void CreateAll()
        {
            Create();
            CreateVision2LevelData.Create();
            CreateGreenChapelLevelData.Create();
            CreateOrfeoOtherworldLevelData.Create();
        }

        [MenuItem("KyndeBlade/Create Vision I Level Data")]
        public static void Create()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Data"))
                AssetDatabase.CreateFolder("Assets/Resources", "Data");
            if (!AssetDatabase.IsValidFolder(DataPath))
                AssetDatabase.CreateFolder("Assets/Resources/Data", "Vision1");

            var wodeWoBabyBeat = CreateStoryBeat("WodeWoBaby", "A soft summer morning on the Malvern Hills. Wille rests beneath the Worcestershire Beacon. \"In a somer seson, whan softe was the sonne...\" Sleep comes. The dream begins.\n\nIn the shade of an ancient oak, Wille finds a tiny thing—moss and leaf, root and bark, no bigger than a fist. A seedling. A child of the forest. It stirs, reaching for warmth. Its breath is faint, like wind through young leaves.", "Narrator", 8f);
            var wodeWoCareBeat = CreateStoryBeat("WodeWoCare", "Thou shelterest it beneath the oak. Thou givest it water from St Anne's well. Thou tendest it as dream-days pass. It grows. Roots deepen. Bark hardens. A soft voice begins to form.", "Narrator", 6f);
            var wodeWoGrownBeat = CreateStoryBeat("WodeWoGrown", "One morning it speaks. \"Wode-Wo,\" it says, its voice like wind through branches. \"I am the keeper of this threshold. I shall walk with thee, dreamer. Fear not.\" The dream begins.", "Narrator", 6f);
            CreateStoryBeat("WodeWoDeath", "Pale hands reach from the gloaming. The fae take him—not gently. They pull him apart, root from branch from breath, as if he were nothing. His voice breaks. \"Dreamer... I tried my best... to...\" Cold fingers close. What remains is scattered. The forest weeps. He does not rise again.", "Narrator", 10f);
            var wodeWoRemainsBeat = CreateStoryBeat("WodeWoRemains", "A soft summer morning on the Malvern Hills. Wille rests beneath the Worcestershire Beacon. In the shade of the ancient oak, Wode-Wo's scattered remains lie where the fae left them. The seedling thou once raised—root and branch and breath—never to rise again. The forest mourns. The dream begins.", "Narrator", 8f);
            var fayreBeat = CreateStoryBeat("FayreFelde", "A fair feeld ful of folke fonde I there bitwene. Wille sees a field full of people from all walks of life, all working.", "Narrator");
            var tourBeat = CreateStoryBeat("TourDongeoun", "A tour on a toft, trieliche ymaked. Wille sees the Tower of Trewthe and the Dungeon representing Hell.", "Narrator");
            var piersBeat = CreateStoryBeat("PiersField", "Piers the Plowman appears in his humble field, guiding Wille toward the path of Do-Wel.", "Narrator");
            var sinsBeat = CreateStoryBeat("SevenSins", "The Seven Deadly Sins gather. Wille must face them before the quest can continue.", "Narrator");

            var tutorialEnc = CreateEncounter("TutorialEncounter", null, null);
            var fayreEnc = CreateEncounter("FayreFeldeEncounter", "False");
            var dongeounEnc = CreateEncounter("DongeounEncounter", "False", "LadyMede", "Wrath");
            var piersEnc = CreateEncounter("PiersFieldEncounter", "False", null);
            var sinsEnc = CreateEncounter("SevenSinsEncounter", "False", "LadyMede", "Wrath");

            var malvern = CreateLocation("malvern", "Malvern Hills", "Real life: Worcestershire. Wille rests beneath the Beacon; the dream begins here.", 0, 0, "Prologue", null, tutorialEnc, "fayre_felde");
            malvern.IsRealLife = true;
            malvern.RealLifeLocationId = "worcestershire_beacon";
            malvern.StoryBeatSequenceOnArrival.Clear();
            malvern.StoryBeatSequenceOnArrival.Add(wodeWoBabyBeat);
            malvern.StoryBeatSequenceOnArrival.Add(wodeWoCareBeat);
            malvern.StoryBeatSequenceOnArrival.Add(wodeWoGrownBeat);
            malvern.StoryBeatOnArrivalWhenWodeWoDead = wodeWoRemainsBeat;
            UnityEditor.EditorUtility.SetDirty(malvern);
            var fayre = CreateLocation("fayre_felde", "Fayre Felde", "Dream: The Fair Field full of folk.", 0, 1, "Passus I", fayreBeat, fayreEnc, "tour", "dongeoun");
            var tour = CreateLocation("tour", "Tour on Toft", "Dream: The Tower of Truth.", 0, 2, "Passus II", tourBeat, null, "dongeoun");
            var dongeoun = CreateLocation("dongeoun", "Dongeoun", "Dream: The Dungeon in the valley.", 0, 3, "Passus II-III", tourBeat, dongeounEnc, "piers_field");
            var piers = CreateLocation("piers_field", "Piers' Field", "Dream: Where Piers the Plowman works.", 0, 4, "Passus IV-V", piersBeat, piersEnc, "seven_sins");
            var sins = CreateLocation("seven_sins", "Seven Sins", "Dream: The gathering of the deadly sins.", 0, 6, "Passus VI-VII", sinsBeat, sinsEnc);

            malvern.NextLocationIds.Clear();
            malvern.NextLocationIds.Add("fayre_felde");
            fayre.NextLocationIds.Clear();
            fayre.NextLocationIds.Add("tour");
            fayre.NextLocationIds.Add("dongeoun");
            tour.NextLocationIds.Clear();
            tour.NextLocationIds.Add("dongeoun");
            dongeoun.NextLocationIds.Clear();
            dongeoun.NextLocationIds.Add("piers_field");
            piers.NextLocationIds.Clear();
            piers.NextLocationIds.Add("seven_sins");
            sins.NextLocationIds.Clear();
            sins.NextLocationIds.Add("quest_do_wel");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Vision I level data created at " + DataPath);
        }

        [MenuItem("KyndeBlade/Create Vision I Level Data (Linear)")]
        public static void CreateLinear()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Data"))
                AssetDatabase.CreateFolder("Assets/Resources", "Data");
            if (!AssetDatabase.IsValidFolder(DataPath))
                AssetDatabase.CreateFolder("Assets/Resources/Data", "Vision1");

            var wodeWoBabyBeat = CreateStoryBeat("WodeWoBaby", "A soft summer morning on the Malvern Hills. Wille rests beneath the Worcestershire Beacon. \"In a somer seson, whan softe was the sonne...\" Sleep comes. The dream begins.\n\nIn the shade of an ancient oak, Wille finds a tiny thing—moss and leaf, root and bark, no bigger than a fist. A seedling. A child of the forest. It stirs, reaching for warmth. Its breath is faint, like wind through young leaves.", "Narrator", 8f);
            var wodeWoCareBeat = CreateStoryBeat("WodeWoCare", "Thou shelterest it beneath the oak. Thou givest it water from St Anne's well. Thou tendest it as dream-days pass. It grows. Roots deepen. Bark hardens. A soft voice begins to form.", "Narrator", 6f);
            var wodeWoGrownBeat = CreateStoryBeat("WodeWoGrown", "One morning it speaks. \"Wode-Wo,\" it says, its voice like wind through branches. \"I am the keeper of this threshold. I shall walk with thee, dreamer. Fear not.\" The dream begins.", "Narrator", 6f);
            CreateStoryBeat("WodeWoDeath", "Pale hands reach from the gloaming. The fae take him—not gently. They pull him apart, root from branch from breath, as if he were nothing. His voice breaks. \"Dreamer... I tried my best... to...\" Cold fingers close. What remains is scattered. The forest weeps. He does not rise again.", "Narrator", 10f);
            var wodeWoRemainsBeat = CreateStoryBeat("WodeWoRemains", "A soft summer morning on the Malvern Hills. Wille rests beneath the Worcestershire Beacon. In the shade of the ancient oak, Wode-Wo's scattered remains lie where the fae left them. The seedling thou once raised—root and branch and breath—never to rise again. The forest mourns. The dream begins.", "Narrator", 8f);
            var fayreBeat = CreateStoryBeat("FayreFelde", "A fair feeld ful of folke fonde I there bitwene.", "Narrator");
            var tourBeat = CreateStoryBeat("TourDongeoun", "A tour on a toft, trieliche ymaked.", "Narrator");
            var piersBeat = CreateStoryBeat("PiersField", "Piers the Plowman appears in his humble field.", "Narrator");
            var sinsBeat = CreateStoryBeat("SevenSins", "The Seven Deadly Sins gather.", "Narrator");

            var tutorialEnc = CreateEncounter("TutorialEncounter", null, null);
            var fayreEnc = CreateEncounter("FayreFeldeEncounter", "False");
            var dongeounEnc = CreateEncounter("DongeounEncounter", "False", "LadyMede", "Wrath");
            var piersEnc = CreateEncounter("PiersFieldEncounter", "False", null);
            var sinsEnc = CreateEncounter("SevenSinsEncounter", "False", "LadyMede", "Wrath");

            var malvern = CreateLocation("malvern", "Malvern Hills", "Real life: Worcestershire. Wille rests beneath the Beacon; the dream begins here.", 0, 0, "Prologue", null, tutorialEnc, "fayre_felde");
            malvern.IsRealLife = true;
            malvern.RealLifeLocationId = "worcestershire_beacon";
            malvern.StoryBeatSequenceOnArrival.Clear();
            malvern.StoryBeatSequenceOnArrival.Add(wodeWoBabyBeat);
            malvern.StoryBeatSequenceOnArrival.Add(wodeWoCareBeat);
            malvern.StoryBeatSequenceOnArrival.Add(wodeWoGrownBeat);
            malvern.StoryBeatOnArrivalWhenWodeWoDead = wodeWoRemainsBeat;
            UnityEditor.EditorUtility.SetDirty(malvern);
            var fayre = CreateLocation("fayre_felde", "Fayre Felde", "Dream: The Fair Field.", 0, 1, "Passus I", fayreBeat, fayreEnc, "tour");
            var tour = CreateLocation("tour", "Tour on Toft", "Dream: The Tower of Truth.", 0, 2, "Passus II", tourBeat, null, "dongeoun");
            var dongeoun = CreateLocation("dongeoun", "Dongeoun", "Dream: The Dungeon in the valley.", 0, 3, "Passus II-III", tourBeat, dongeounEnc, "piers_field");
            var piers = CreateLocation("piers_field", "Piers' Field", "Dream: Where Piers the Plowman works.", 0, 4, "Passus IV-V", piersBeat, piersEnc, "seven_sins");
            var sins = CreateLocation("seven_sins", "Seven Sins", "Dream: The gathering of the deadly sins.", 0, 6, "Passus VI-VII", sinsBeat, sinsEnc);

            malvern.NextLocationIds.Clear();
            malvern.NextLocationIds.Add("fayre_felde");
            fayre.NextLocationIds.Clear();
            fayre.NextLocationIds.Add("tour");
            tour.NextLocationIds.Clear();
            tour.NextLocationIds.Add("dongeoun");
            dongeoun.NextLocationIds.Clear();
            dongeoun.NextLocationIds.Add("piers_field");
            piers.NextLocationIds.Clear();
            piers.NextLocationIds.Add("seven_sins");
            sins.NextLocationIds.Clear();
            sins.NextLocationIds.Add("quest_do_wel");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Vision I linear level data created at " + DataPath);
        }

        static StoryBeat CreateStoryBeat(string id, string text, string speaker, float duration = 5f)
        {
            var beat = ScriptableObject.CreateInstance<StoryBeat>();
            beat.BeatId = id;
            beat.Text = text;
            beat.SpeakerName = speaker;
            beat.DisplayDuration = duration;
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
