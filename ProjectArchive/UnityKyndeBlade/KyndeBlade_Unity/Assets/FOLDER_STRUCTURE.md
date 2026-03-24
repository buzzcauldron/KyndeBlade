# Unity Folder Structure (KyndeBlade)

This project follows standard Unity folder conventions.

## Layout

```
Assets/
├── Art/                    # Shared art reference (e.g. Art/Reference)
├── Editor/                  # Editor-only scripts (menu tools, inspectors)
├── KyndeBlade/              # Project root (game name)
│   ├── Art/                 # Sprites, animations
│   │   ├── Sprites/
│   │   └── Animations/
│   ├── Code/                # Runtime scripts (assemblies)
│   │   ├── Core/            # KyndeBlade.Core — save, map, narrative, visual, game
│   │   ├── Combat/          # KyndeBlade.Combat — turn, characters, bosses
│   │   ├── UI/              # KyndeBlade.UI — combat UI, borders, manuscript UI
│   │   └── World/           # World / level code
│   ├── Data/                # ScriptableObject assets (optional; some data in Resources)
│   └── Shaders/             # Custom shaders (manuscript, gold leaf, toon, etc.)
├── Resources/               # Runtime-loaded assets (Resources.Load)
│   └── Data/                # Location nodes, encounters, dialogue trees
├── Scenes/                  # All .unity scenes
└── Tests/                   # PlayMode / EditMode tests
    └── PlayMode/
```

## Reserved / special folders

- **Editor** — Scripts here run only in the Unity Editor (e.g. `DialogueTreeGenerator`, level data creation).
- **Resources** — Assets here can be loaded at runtime via `Resources.Load()`. Use sparingly; game data (locations, encounters) lives under `Resources/Data`.
- **Scenes** — All scenes should live here. Build settings reference paths under `Assets/Scenes/`.

## Assemblies (asmdef)

- **KyndeBlade.Core** — `KyndeBlade/Code/Core/`
- **KyndeBlade.Combat** — `KyndeBlade/Code/Combat/`
- **KyndeBlade.UI** — `KyndeBlade/Code/UI/`
- **KyndeBlade.EditorTools** — `Assets/Editor/` (Editor-only; name avoids Burst resolver issues)
- **KyndeBlade.Tests.PlayMode** — `Assets/Tests/PlayMode/`

## Conventions

- **Project root**: `KyndeBlade` (not `_Project`) so the top-level folder matches the product name.
- **No duplicate script trees**: Runtime code lives only under `KyndeBlade/Code/`.
- **Scenes**: Keep all `.unity` files under `Assets/Scenes/`.
