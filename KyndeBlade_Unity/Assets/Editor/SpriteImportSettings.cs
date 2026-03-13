#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.U2D;
using UnityEditor.U2D;

namespace KyndeBlade.Editor
{
    /// <summary>Enforces consistent sprite import settings and creates sprite atlases.</summary>
    public static class SpriteImportSettings
    {
        const int PixelsPerUnit = 16;
        const int MaxTextureSize = 256;

        [MenuItem("KyndeBlade/Apply Sprite Import Settings")]
        public static void ApplyAll()
        {
            var guids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/KyndeBlade/Art/Sprites" });
            int count = 0;
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (ApplyToAsset(path)) count++;
            }
            AssetDatabase.SaveAssets();
            Debug.Log($"[SpriteImportSettings] Applied to {count} sprites.");
        }

        public static bool ApplyToAsset(string assetPath)
        {
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null) return false;

            bool changed = false;
            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                changed = true;
            }
            if (importer.spritePixelsPerUnit != PixelsPerUnit)
            {
                importer.spritePixelsPerUnit = PixelsPerUnit;
                changed = true;
            }
            if (importer.filterMode != FilterMode.Point)
            {
                importer.filterMode = FilterMode.Point;
                changed = true;
            }
            if (importer.textureCompression != TextureImporterCompression.Uncompressed)
            {
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                changed = true;
            }
            if (importer.maxTextureSize != MaxTextureSize)
            {
                importer.maxTextureSize = MaxTextureSize;
                changed = true;
            }
            if (importer.mipmapEnabled)
            {
                importer.mipmapEnabled = false;
                changed = true;
            }

            if (changed)
            {
                importer.SaveAndReimport();
                return true;
            }
            return false;
        }

        [MenuItem("KyndeBlade/Create Sprite Atlases")]
        public static void CreateAtlases()
        {
            EnsureDir("Assets/KyndeBlade/Art/Atlases");

            CreateAtlas("Characters", "Assets/KyndeBlade/Art/Sprites/Characters");
            CreateAtlas("UI", "Assets/KyndeBlade/Art/Sprites/UI");
            CreateAtlas("Map", "Assets/KyndeBlade/Art/Sprites/Map");
            CreateAtlas("Effects", "Assets/KyndeBlade/Art/Sprites/Effects");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[SpriteImportSettings] Sprite atlases created.");
        }

        static void CreateAtlas(string name, string folder)
        {
            string atlasPath = $"Assets/KyndeBlade/Art/Atlases/{name}.spriteatlas";
            if (AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath) != null) return;

            var atlas = new SpriteAtlas();

            var packSettings = new SpriteAtlasPackingSettings
            {
                padding = 2,
                enableRotation = false,
                enableTightPacking = false
            };
            atlas.SetPackingSettings(packSettings);

            var texSettings = new SpriteAtlasTextureSettings
            {
                filterMode = FilterMode.Point,
                generateMipMaps = false,
                sRGB = true,
                readable = false
            };
            atlas.SetTextureSettings(texSettings);

            EnsureDir(folder);
            var folderObj = AssetDatabase.LoadAssetAtPath<Object>(folder);
            if (folderObj != null)
                atlas.Add(new[] { folderObj });

            AssetDatabase.CreateAsset(atlas, atlasPath);
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
