using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace KyndeBlade.Editor
{
    /// <summary>
    /// Pre-PlayMode guardrail for known regressions seen in this project.
    /// Keeps checks lightweight and focused on patterns that previously broke compile/runtime.
    /// </summary>
    [InitializeOnLoad]
    public static class KnownRegressionGuard
    {
        const bool BlockPlayModeOnKnownRegressionPatterns = true;
        static KnownRegressionGuard()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingEditMode)
                return;

            var issues = CollectIssues();
            if (issues.Count == 0)
                return;

            Debug.LogError("[KyndeBlade] Known regression guard blocked Play Mode:\n- " + string.Join("\n- ", issues));
            if (BlockPlayModeOnKnownRegressionPatterns)
                EditorApplication.isPlaying = false;
        }

        static List<string> CollectIssues()
        {
            var issues = new List<string>();

            AddIfContains(
                issues,
                "Assets/KyndeBlade/Code/Combat/TurnSequenceController.cs",
                "ActionData.BaseDamage",
                "Use ActionData.Damage (BaseDamage no longer exists).");

            AddIfContains(
                issues,
                "Assets/Editor/PlaytestTools.cs",
                ".Data.Type",
                "Use StatusEffectData.EffectType (Type no longer exists).");

            AddIfContains(
                issues,
                "Assets/KyndeBlade/Code/Core/Game/SafeManagerLookup.cs",
                "where T : Object",
                "Use UnityEngine.Object to avoid Object ambiguity.");

            AddIfContains(
                issues,
                "Assets/KyndeBlade/Code/Combat/Visual/ManuscriptParticleFactory.cs",
                "AddComponent<ParticleSystem>",
                "Avoid ParticleSystem dependency in ManuscriptParticleFactory.");

            AddIfContains(
                issues,
                "Assets/Editor/SpriteImportSettings.cs",
                "UnityEngine.U2D.SpriteAtlasPackingSettings",
                "Use SpriteAtlasPackingSettings from UnityEditor.U2D.");

            AddIfContains(
                issues,
                "Assets/Editor/SpriteImportSettings.cs",
                "UnityEngine.U2D.SpriteAtlasTextureSettings",
                "Use SpriteAtlasTextureSettings from UnityEditor.U2D.");

            ScanCoreAssemblyBoundary(issues);
            return issues;
        }

        static void ScanCoreAssemblyBoundary(List<string> issues)
        {
            var corePath = Path.Combine(Application.dataPath, "KyndeBlade/Code/Core");
            if (!Directory.Exists(corePath))
                return;

            var blockedTokens = new[]
            {
                "TurnManager",
                "KyndeBladeGameManager",
                "WorldMapManager",
                "CombatActionType",
                "MedievalCharacter"
            };

            var files = Directory.GetFiles(corePath, "*.cs", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                var text = File.ReadAllText(files[i]);
                for (int j = 0; j < blockedTokens.Length; j++)
                {
                    if (!text.Contains(blockedTokens[j]))
                        continue;

                    // Allow files that intentionally use reflection/string lookups or mention manager names in comments.
                    if (files[i].EndsWith("DebugOverlay.cs", StringComparison.Ordinal) ||
                        files[i].EndsWith("DialogueTreeExecutor.cs", StringComparison.Ordinal) ||
                        files[i].EndsWith("SafeManagerLookup.cs", StringComparison.Ordinal) ||
                        files[i].EndsWith("PiersAppearanceRandomizer.cs", StringComparison.Ordinal))
                        continue;

                    var rel = ToAssetRelative(files[i]);
                    issues.Add(rel + " contains Core->Combat risky token '" + blockedTokens[j] + "'.");
                    break;
                }
            }
        }

        static void AddIfContains(List<string> issues, string assetRelativePath, string token, string message)
        {
            var abs = Path.Combine(Directory.GetCurrentDirectory(), assetRelativePath);
            if (!File.Exists(abs))
                return;

            var text = File.ReadAllText(abs);
            if (text.Contains(token))
                issues.Add(assetRelativePath + ": " + message);
        }

        static string ToAssetRelative(string absolutePath)
        {
            var normalized = absolutePath.Replace('\\', '/');
            var idx = normalized.IndexOf("/Assets/", StringComparison.Ordinal);
            return idx >= 0 ? normalized.Substring(idx + 1) : normalized;
        }
    }
}
