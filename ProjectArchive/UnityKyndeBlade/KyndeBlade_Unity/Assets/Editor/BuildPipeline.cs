#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace KyndeBlade.Editor
{
    /// <summary>Automated build pipeline for Windows, Linux, and macOS.</summary>
    public static class KyndeBladeBuildPipeline
    {
        const string BuildRoot = "Builds";
        const string ProductName = "KyndeBlade";

        [MenuItem("KyndeBlade/Build/All Platforms")]
        public static void BuildAll()
        {
            GenerateGameData();
            ValidateScenes();
            BuildWindows();
            BuildLinux();
            BuildMacOS();
        }

        [MenuItem("KyndeBlade/Build/Windows x64")]
        public static void BuildWindows()
        {
            GenerateGameData();
            ValidateScenes();
            var path = Path.Combine(BuildRoot, "Windows", $"{ProductName}.exe");
            Build(BuildTarget.StandaloneWindows64, path);
        }

        [MenuItem("KyndeBlade/Build/Linux x64")]
        public static void BuildLinux()
        {
            GenerateGameData();
            ValidateScenes();
            var path = Path.Combine(BuildRoot, "Linux", ProductName);
            Build(BuildTarget.StandaloneLinux64, path);
        }

        [MenuItem("KyndeBlade/Build/macOS")]
        public static void BuildMacOS()
        {
            GenerateGameData();
            ValidateScenes();
            var path = Path.Combine(BuildRoot, "macOS", $"{ProductName}.app");
            Build(BuildTarget.StandaloneOSX, path);
        }

        static void Build(BuildTarget target, string outputPath)
        {
            var scenes = GetScenePaths();
            if (scenes.Length == 0)
            {
                Debug.LogError("[Build] No scenes in build settings.");
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = target,
                options = BuildOptions.None
            };

            Debug.Log($"[Build] Building {target} to {outputPath}...");
            var report = BuildPipeline.BuildPlayer(options);

            if (report.summary.result == BuildResult.Succeeded)
                Debug.Log($"[Build] {target} succeeded in {report.summary.totalTime.TotalSeconds:F1}s. Size: {report.summary.totalSize / (1024 * 1024):F1}MB");
            else
                Debug.LogError($"[Build] {target} failed: {report.summary.result}");
        }

        static string[] GetScenePaths()
        {
            return EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();
        }

        [MenuItem("KyndeBlade/Build/Validate Scenes")]
        public static void ValidateScenes()
        {
            var scenes = EditorBuildSettings.scenes;
            if (scenes.Length == 0)
            {
                Debug.LogError("[Build] No scenes in build settings. Run KyndeBlade > Setup Project first.");
                return;
            }

            bool hasMainMenu = false;
            bool hasMain = false;
            for (int i = 0; i < scenes.Length; i++)
            {
                var s = scenes[i];
                if (!s.enabled) Debug.LogWarning($"[Build] Scene at index {i} is disabled: {s.path}");
                if (!File.Exists(s.path)) Debug.LogError($"[Build] Scene file missing: {s.path}");
                if (s.path.Contains("MainMenu")) { hasMainMenu = true; if (i != 0) Debug.LogWarning("[Build] MainMenu should be at index 0."); }
                if (s.path.EndsWith("Main.unity") && !s.path.Contains("MainMenu")) { hasMain = true; if (i != 1) Debug.LogWarning("[Build] Main scene should be at index 1."); }
            }

            if (!hasMainMenu) Debug.LogWarning("[Build] MainMenu scene not found. Run KyndeBlade > Create Main Menu Scene.");
            if (!hasMain) Debug.LogWarning("[Build] Main scene not found. Run KyndeBlade > Create Main Scene.");
            Debug.Log($"[Build] Validation complete. {scenes.Length} scenes, MainMenu={hasMainMenu}, Main={hasMain}.");
        }

        static void GenerateGameData()
        {
            Debug.Log("[Build] Pre-build: generating game data...");
            CreateAllGameData.GenerateAll();
        }

        [MenuItem("KyndeBlade/Build/Set Version")]
        public static void SetVersion()
        {
            string version = $"0.3.0-{System.DateTime.UtcNow:yyyyMMdd}";
            PlayerSettings.bundleVersion = version;
            Debug.Log($"[Build] Version set to {version}");
        }
    }
}
#endif
