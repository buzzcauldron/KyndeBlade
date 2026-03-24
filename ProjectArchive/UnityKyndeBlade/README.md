# Unity KyndeBlade (reference archive)

**Roles and policy:** [`UNITY_REFERENCE_ARCHIVE.md`](UNITY_REFERENCE_ARCHIVE.md).

The **Unity oracle / authoring project** lives here for parity, export-to-Godot, and TDAD workflows:

`ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/`

Open that folder in **Unity Hub** (Unity 6.3 LTS per `ProjectSettings/ProjectVersion.txt` inside the project).

## Repo-root Unity noise (optional)

If Unity was ever opened from the **monorepo root**, generated files may appear under:

`ProjectArchive/UnityKyndeBlade/repo_root_unity_artifacts/`

That folder is **not** the Unity project root; it is only a stash of stray `Library` / `Logs` / `UserSettings` / `ProjectSettings` fragments and any `.sln` / `.csproj` that landed next to `README.md`. Safe to delete locally if you only use the nested `KyndeBlade_Unity` project.

## Snapshot checklist (M6 / cold storage)

When `godot-m6-cutover-archive` is signed off, optionally record:

| Field | Value |
|-------|--------|
| Frozen git SHA | `________________` |
| Unity Editor version | `________________` |
| Godot primary path | `KyndeBlade_Godot/` (repo root) |
| Date | `________________` |

See also [`.tdad/prompts/godot-archive-unity-snapshot.md`](../../.tdad/prompts/godot-archive-unity-snapshot.md).

## Why keep Unity in git?

Historical **spec** for TDAD workflows under `.tdad/workflows/*` and parity until Godot fully replaces behaviour tests.
