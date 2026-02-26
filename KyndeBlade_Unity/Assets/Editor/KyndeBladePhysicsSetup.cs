#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace KyndeBlade
{
    /// <summary>Creates physics materials and optional layers for environments with rigidbodies. Run via KyndeBlade > Setup Physics and Layers.</summary>
    public static class KyndeBladePhysicsSetup
    {
        const string PhysicsFolder = "Assets/KyndeBlade/Physics";
        const string DefaultMaterialName = "Default";
        const string EnvironmentMaterialName = "Environment";
        const string SlipperyMaterialName = "Slippery";

        static readonly int[] LayerSlots = { 6, 7, 8, 9 };
        static readonly string[] LayerNames = { "Environment", "Player", "Enemy", "Trigger" };

        [MenuItem("KyndeBlade/Setup Physics and Layers")]
        public static void SetupPhysicsAndLayers()
        {
            EnsurePhysicsFolder();
            CreatePhysicsMaterials();
            EnsureLayers();
            AssignDefaultPhysicsMaterial();
            Debug.Log("KyndeBlade: Physics materials and layers set up. See Assets/KyndeBlade/Physics and Docs/PHYSICS_SETUP.md.");
        }

        [MenuItem("KyndeBlade/Create Physics Materials Only")]
        public static void CreatePhysicsMaterialsOnly()
        {
            EnsurePhysicsFolder();
            CreatePhysicsMaterials();
            Debug.Log("KyndeBlade: Physics materials created in " + PhysicsFolder);
        }

        static void EnsurePhysicsFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/KyndeBlade"))
                AssetDatabase.CreateFolder("Assets", "KyndeBlade");
            if (!AssetDatabase.IsValidFolder(PhysicsFolder))
                AssetDatabase.CreateFolder("Assets/KyndeBlade", "Physics");
        }

        static void CreatePhysicsMaterials()
        {
            CreateOrUpdateMaterial(Path.Combine(PhysicsFolder, DefaultMaterialName + ".physicMaterial"), 0.6f, 0.6f, 0f);
            CreateOrUpdateMaterial(Path.Combine(PhysicsFolder, EnvironmentMaterialName + ".physicMaterial"), 0.8f, 0.8f, 0f);
            CreateOrUpdateMaterial(Path.Combine(PhysicsFolder, SlipperyMaterialName + ".physicMaterial"), 0.05f, 0.05f, 0.1f);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        static void CreateOrUpdateMaterial(string assetPath, float dynamicFriction, float staticFriction, float bounciness)
        {
            var mat = AssetDatabase.LoadAssetAtPath<PhysicMaterial>(assetPath);
            if (mat == null)
            {
                mat = new PhysicMaterial(Path.GetFileNameWithoutExtension(assetPath))
                {
                    dynamicFriction = dynamicFriction,
                    staticFriction = staticFriction,
                    bounciness = bounciness,
                    frictionCombine = PhysicMaterialCombine.Average,
                    bounceCombine = PhysicMaterialCombine.Average
                };
                AssetDatabase.CreateAsset(mat, assetPath);
            }
            else
            {
                mat.dynamicFriction = dynamicFriction;
                mat.staticFriction = staticFriction;
                mat.bounciness = bounciness;
                EditorUtility.SetDirty(mat);
            }
        }

        static void EnsureLayers()
        {
            var tagManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
            if (tagManager == null || tagManager.Length == 0) return;
            var so = new SerializedObject(tagManager[0]);
            var layersProp = so.FindProperty("layers");
            if (layersProp == null) return;
            for (int i = 0; i < LayerSlots.Length && i < LayerNames.Length; i++)
            {
                int slot = LayerSlots[i];
                if (slot < 0 || slot >= layersProp.arraySize) continue;
                var el = layersProp.GetArrayElementAtIndex(slot);
                if (el != null && string.IsNullOrEmpty(el.stringValue))
                {
                    el.stringValue = LayerNames[i];
                }
            }
            so.ApplyModifiedProperties();
        }

        static void AssignDefaultPhysicsMaterial()
        {
            var defaultMat = AssetDatabase.LoadAssetAtPath<PhysicMaterial>(Path.Combine(PhysicsFolder, DefaultMaterialName + ".physicMaterial"));
            if (defaultMat == null) return;
            var physicsManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/DynamicsManager.asset");
            if (physicsManager == null || physicsManager.Length == 0) return;
            var so = new SerializedObject(physicsManager[0]);
            var defaultMaterialProp = so.FindProperty("m_DefaultMaterial");
            if (defaultMaterialProp != null)
            {
                defaultMaterialProp.objectReferenceValue = defaultMat;
                so.ApplyModifiedProperties();
            }
        }
    }
}
#endif
