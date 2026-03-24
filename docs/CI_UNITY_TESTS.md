# Running Unity tests in batch mode (CI)

Use the Unity Editor in **batch mode** with the **Test Framework** to run Edit Mode and Play Mode tests without the GUI.

## Prerequisites

- Unity **6000.x** (match [ProjectVersion.txt](../KyndeBlade_Unity/ProjectSettings/ProjectVersion.txt)).
- Project path: `KyndeBlade_Unity` (this repo).
- On macOS/Linux, the editor binary is typically:
  - macOS: `/Applications/Unity/Hub/Editor/<version>/Unity.app/Contents/MacOS/Unity`
  - Windows: `C:\Program Files\Unity\Hub\Editor\<version>\Editor\Unity.exe`

## Edit Mode tests (assembly: `KyndeBlade.Tests.EditMode`)

```bash
UNITY="/Applications/Unity/Hub/Editor/6000.3.9f1/Unity.app/Contents/MacOS/Unity"
PROJECT="$(pwd)/KyndeBlade_Unity"

"$UNITY" -batchmode -nographics -quit \
  -projectPath "$PROJECT" \
  -runTests \
  -testPlatform editmode \
  -testResults "$PROJECT/TestResults-editmode.xml" \
  -logFile -
```

## Play Mode tests (assembly: `KyndeBlade.Tests.PlayMode`)

Play Mode runs the **PlayMode** test player (headless). First scene in **Build Settings** must be valid (`Main.unity` is enabled).

```bash
"$UNITY" -batchmode -nographics -quit \
  -projectPath "$PROJECT" \
  -runTests \
  -testPlatform playmode \
  -testResults "$PROJECT/TestResults-playmode.xml" \
  -logFile -
```

## Combined / filtered runs

- Run a single test name (exact match):

```bash
-runTests -testFilter "FullyQualifiedName~KyndeBlade.Tests.Phase1LogicTests"
```

- **Assembly definition** names map to Unity test assemblies listed in the Test Runner window.

## CI tips

1. Cache the `Library` folder when possible to shorten import time (Unity version must match).
2. Fail the job if the XML result file reports failures (Unity still exits 0 in some versions; parse `TestResults-*.xml`).
3. Use a **Linux** Unity install or **Windows** agent with the same editor version as the team.
4. `GameCompletenessPlayModeTests.MainScene_Loads_WithKyndeBladeGameManager` loads `Main` — ensure **EditorBuildSettings** includes that scene (already configured).

## References

- [Unity Test Framework — Running tests from the command line](https://docs.unity3d.com/Packages/com.unity.test-framework@latest/manual/reference-command-line.html)
