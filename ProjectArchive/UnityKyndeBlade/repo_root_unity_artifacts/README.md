# Stray Unity files from repo root

Unity was opened at least once from the **KyndeBlade monorepo root**, which produced `Library/`, `Logs/`, `UserSettings/`, partial `ProjectSettings/`, and IDE solution files next to `README.md`.

Those directories and files were **moved here** so the repo root stays Godot- and docs-first.

**The real Unity project** is:

`../KyndeBlade_Unity/` (sibling of this folder)

You may delete this entire `repo_root_unity_artifacts` directory locally if you do not need the old cache; Unity will regenerate caches when you open the nested project.
