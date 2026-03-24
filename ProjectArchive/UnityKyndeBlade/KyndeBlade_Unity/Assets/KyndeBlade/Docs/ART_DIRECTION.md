# Art direction (two lanes)

KyndeBlade uses **two visual references** that apply in different contexts. This doc is the single source for contrast, scale, and where assets go.

## Lane A — Vista / hub (cinematic mood)

- **Intent:** Lush exterior, soft light, readable silhouettes (reference: high-fidelity vista / party looking outward).
- **Use for:** Main menu backdrop, story beats, optional map interstitials—not orthographic combat.
- **Implementation:** Full-screen `RawImage` or static art behind narrative UI; optional light post-processing on **non-combat** views only (see `ARCHITECTURE.md`).
- **Contrast:** Keep body text WCAG-oriented vs backdrop (darken overlay behind parchment panel if the art is bright).

## Lane B — Combat (SNES / Super Metroid)

- **Intent:** Side-view, **dark void** behind actors, **high contrast** sprites, **large boss vs small party**, clear platform/hazard read.
- **Use for:** `Main` combat view, `EncounterConfig` layouts, sorting layers for foreground hazards.
- **Defaults:** Combat backdrop color is configurable on [`GameSettings`](../../Code/Core/Game/GameSettings.cs) (`CombatBackdropColor`); default skews **near-black / deep cool** so colored sprites pop.
- **Pixel art:** When using true pixel sprites, document **PPU** per asset and avoid non-integer scale in world space.

## Unity packages (art / presentation)

Authoring in **this** repo uses the **Built-in Render Pipeline** (not URP/HDRP). Relevant UPM dependencies are declared in [`Packages/manifest.json`](../../../Packages/manifest.json), including:

- **`com.unity.feature.2d`** — 2D workflow
- **`com.unity.ugui`** — uGUI
- **`com.unity.inputsystem`** — input (see `ARCHITECTURE.md` for Active Input = Both)

Optional **Built-in** post stack: add **`com.unity.postprocessing`** from Package Manager only if you confirm Unity 6 compatibility; vista/hub-only per Lane A. Do not document URP Volume workflows as “the” project default without a pipeline migration.

## Reference files (project)

Place approved mood boards under:

- `Assets/KyndeBlade/Docs/References/` (optional folder; add `.meta` via Unity)

Do not put large PNGs in `Resources` unless they are loaded intentionally.

## Boss scale

- Prefer **larger enemy root scale** or **forward spawn offset** over changing AI stats.
- Tune per encounter in `EncounterConfig` / prefab scale; document notable bosses here when added.
