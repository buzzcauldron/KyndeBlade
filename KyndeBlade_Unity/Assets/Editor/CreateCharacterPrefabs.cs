using UnityEngine;
using UnityEditor;
using KyndeBlade;
using KyndeBlade.Combat;

namespace KyndeBlade.Editor
{
    /// <summary>
    /// Creates character prefabs with placeholder sprites, components, and stats.
    /// Menu: KyndeBlade > Create All Character Prefabs
    /// </summary>
    public static class CreateCharacterPrefabs
    {
        const string PrefabRoot = "Assets/Resources/Prefabs/Characters";

        [MenuItem("KyndeBlade/Create All Character Prefabs")]
        public static void CreateAll()
        {
            EnsureDirectory(PrefabRoot);

            CreatePartyPrefab("Wille", CharacterClass.Dreamer, true);
            CreatePartyPrefab("Piers", CharacterClass.Knight, true);
            CreatePartyPrefab("Conscience", CharacterClass.Mage, true);

            CreateEnemyPrefab<FalseCharacter>("False", CharacterClass.Rogue);
            CreateEnemyPrefab<LadyMedeCharacter>("Lady Mede", CharacterClass.Mage);
            CreateEnemyPrefab<WrathCharacter>("Wrath", CharacterClass.Knight);
            CreateEnemyPrefab<HungerCharacter>("Hunger", CharacterClass.Knight);
            CreateEnemyPrefab<PrideCharacter>("Pride", CharacterClass.Knight);
            CreateEnemyPrefab<GreenKnightCharacter>("Green Knight", CharacterClass.Knight);
            CreateEnemyPrefab<EldeCharacter>("Elde", CharacterClass.Mage);
            CreateEnemyPrefab<EnvyCharacter>("Envy", CharacterClass.Knight);
            CreateEnemyPrefab<SlothCharacter>("Sloth", CharacterClass.Knight);
            CreateEnemyPrefab<LustCharacter>("Lust", CharacterClass.Knight);

            TryAssignToGameManager();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("KyndeBlade: All character prefabs created at " + PrefabRoot);
        }

        static void CreatePartyPrefab(string charName, CharacterClass cls, bool isPlayer)
        {
            var path = $"{PrefabRoot}/{charName.Replace(" ", "")}.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            var go = new GameObject(charName);
            var mc = go.AddComponent<MedievalCharacter>();
            mc.CharacterClassType = cls;
            mc.CharacterName = charName;

            var col = go.AddComponent<BoxCollider>();
            col.size = new Vector3(0.8f, 1.2f, 0.2f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteFactory.GetSpriteForCharacter(charName, isPlayer);
            sr.color = PlaceholderSpriteFactory.GetColorForCharacter(charName, isPlayer);
            go.transform.localScale = PlaceholderSpriteFactory.GetScaleForCharacter(charName);

            go.AddComponent<SimpleAnimator>();

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        static void CreateEnemyPrefab<T>(string charName, CharacterClass cls) where T : MedievalCharacter
        {
            var path = $"{PrefabRoot}/{charName.Replace(" ", "")}.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            var go = new GameObject(charName);
            go.AddComponent<T>();

            var col = go.GetComponent<Collider>();
            if (col == null)
            {
                var box = go.AddComponent<BoxCollider>();
                box.size = new Vector3(0.8f, 1.2f, 0.2f);
            }

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteFactory.GetSpriteForCharacter(charName, false);
            sr.color = PlaceholderSpriteFactory.GetColorForCharacter(charName, false);
            go.transform.localScale = PlaceholderSpriteFactory.GetScaleForCharacter(charName);

            go.AddComponent<SimpleAnimator>();

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        static void TryAssignToGameManager()
        {
            var gm = Object.FindFirstObjectByType<KyndeBladeGameManager>();
            if (gm == null) return;

            gm.WillePrefab = LoadPrefabComponent<MedievalCharacter>("Wille");
            gm.PiersPrefab = LoadPrefabComponent<MedievalCharacter>("Piers");
            gm.ConsciencePrefab = LoadPrefabComponent<MedievalCharacter>("Conscience");
            gm.FalsePrefab = LoadPrefabComponent<MedievalCharacter>("False");
            gm.LadyMedePrefab = LoadPrefabComponent<MedievalCharacter>("Lady Mede");
            gm.WrathPrefab = LoadPrefabComponent<MedievalCharacter>("Wrath");
            gm.HungerPrefab = LoadPrefabComponent<MedievalCharacter>("Hunger");
            gm.PridePrefab = LoadPrefabComponent<MedievalCharacter>("Pride");
            gm.GreenKnightPrefab = LoadPrefabComponent<MedievalCharacter>("Green Knight");
            gm.EldePrefab = LoadPrefabComponent<MedievalCharacter>("Elde");
            gm.EnvyPrefab = LoadPrefabComponent<MedievalCharacter>("Envy");
            gm.SlothPrefab = LoadPrefabComponent<MedievalCharacter>("Sloth");
            gm.LustPrefab = LoadPrefabComponent<MedievalCharacter>("Lust");

            EditorUtility.SetDirty(gm);
        }

        static T LoadPrefabComponent<T>(string charName) where T : Component
        {
            var path = $"{PrefabRoot}/{charName.Replace(" ", "")}.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            return prefab != null ? prefab.GetComponent<T>() : null;
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
