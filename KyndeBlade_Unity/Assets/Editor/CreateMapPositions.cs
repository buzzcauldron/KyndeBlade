using UnityEngine;
using UnityEditor;
using KyndeBlade;

namespace KyndeBlade.Editor
{
    /// <summary>Creates a default MapLocationPositions asset with geographic layout for all known locations.</summary>
    public static class CreateMapPositions
    {
        [MenuItem("KyndeBlade/Create Map Location Positions")]
        public static void Create()
        {
            var asset = ScriptableObject.CreateInstance<MapLocationPositions>();
            asset.Positions = new[]
            {
                Pos("malvern",              0.15f, 0.50f),
                Pos("fayre_felde",          0.35f, 0.60f),
                Pos("london",              0.55f, 0.70f),
                Pos("holy_church",         0.30f, 0.80f),
                Pos("green_chapel",        0.75f, 0.45f),
                Pos("otherworld",          0.85f, 0.30f),
                Pos("conscience_court",    0.45f, 0.50f),
                Pos("mede_court",          0.50f, 0.35f),
                Pos("reason_court",        0.60f, 0.55f),
                Pos("tower_of_pride",      0.70f, 0.65f),
                Pos("hunger_field",        0.40f, 0.25f),
                Pos("plowmans_half_acre",  0.25f, 0.35f),
                Pos("wrath_hollow",        0.55f, 0.20f),
                Pos("st_annes_well",       0.10f, 0.60f),
                Pos("worcestershire_beacon",0.08f, 0.70f),
                Pos("great_malvern_priory", 0.12f, 0.40f),
            };

            string dir = "Assets/Resources/Data";
            if (!AssetDatabase.IsValidFolder(dir))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    AssetDatabase.CreateFolder("Assets", "Resources");
                AssetDatabase.CreateFolder("Assets/Resources", "Data");
            }

            string path = $"{dir}/MapLocationPositions.asset";
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            Debug.Log("KyndeBlade: MapLocationPositions created at " + path);
        }

        static MapLocationPositions.Entry Pos(string id, float x, float y)
        {
            return new MapLocationPositions.Entry { LocationId = id, Position = new Vector2(x, y) };
        }
    }
}
