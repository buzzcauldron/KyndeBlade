# Third-party visual and audio assets — licenses

Use this file (and per-pack `LICENSE.txt` under each folder) so **commercial use** and **attribution** stay provable. This is separate from literary credits in [`SOURCES_AND_CREDITS.md`](../ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets/KyndeBlade/Docs/SOURCES_AND_CREDITS.md).

## Guidance (read first)

- **Where art goes (lanes):** [`ART_DIRECTION.md`](../ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets/KyndeBlade/Docs/ART_DIRECTION.md) — Lane A (vista / menu / story) vs Lane B (combat). Mood: [`ART_DIRECTION_VISUAL_BIBLE.md`](../ProjectArchive/docs/ART_DIRECTION_VISUAL_BIBLE.md).
- **Music / SFX workflow:** [`MUSIC_SOURCES.md`](../ProjectArchive/docs/MUSIC_SOURCES.md) — CC0 / CC BY / CC BY-SA rules, checklist, credits pattern.
- **Unity mood boards (optional):** `ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets/KyndeBlade/Docs/References/` — see [`References/README.md`](../ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets/KyndeBlade/Docs/References/README.md).
- **Godot imports:** `KyndeBlade_Godot/assets/third_party/<pack_name>/` — each pack **must** include copied license text or link snapshot in `LICENSE.txt`.

## Prefer

- **CC0** — simplest for games; still document source URL.
- **CC BY** — store exact attribution string required by the author; add to in-game **Credits** and this table.

Avoid assets without a clear license file or page.

## Optional Godot mood refs (no row until you commit a file)

- **Pre-Raphaelite / Salome plates:** when you add cleared PNG/WebP under [`KyndeBlade_Godot/assets/reference_preraphaelite/`](../KyndeBlade_Godot/assets/reference_preraphaelite/), add an **inventory row** below (path, URL, license, attribution). See that folder’s README.

## Inventory (fill in as you add files)

| Asset / pack | Repo path | Source URL | License | Attribution (if required) |
|--------------|-----------|------------|---------|----------------------------|
| Menu Lane A gradient | `KyndeBlade_Godot/assets/lane_a_menu_backdrop.tres` | *(engine-authored)* | *(n/a)* | — |
| Manuscript / Lane A–B palette (code only) | `KyndeBlade_Godot/scripts/kyndeblade_art_palette.gd`, `kyndeblade_manuscript_theme.gd` | Colors from repo markdown ([`ART_DIRECTION.md`](../ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets/KyndeBlade/Docs/ART_DIRECTION.md), [`UI_MANUSCRIPT_THEME.md`](../ProjectArchive/docs/UI_MANUSCRIPT_THEME.md)) | *(n/a)* | — |
| Unity slice export (locations + encounters) | `KyndeBlade_Godot/data/exported_from_unity.json` | Derived from `Loc_tour`, `Loc_fayre_felde`, `FayreFeldeEncounter` in Unity `Assets/Resources/Data/Vision1/` | Same as game / SO data *(provenance only)* | Regenerate via Unity **KyndeBlade → Export Slice Data for Godot** |
| Combat defensive-window tones | `KyndeBlade_Godot/scripts/combat_window_tone.gd` (`CombatWindowTone.make_tone_hz`) | *(engine-generated sine bursts at runtime)* | *(n/a — project-authored PCM)* | No third-party sample; plays on **SFX** bus |
| Combat enemy-kill impact | `KyndeBlade_Godot/scripts/combat_window_tone.gd` (`CombatWindowTone.make_enemy_kill_impact`) | *(engine-generated thump + decay + noise at runtime)* | *(n/a — project-authored PCM)* | No third-party sample; plays on **SFX** bus during victory slow-mo; see [`GODOT_AUDIO_DESIGN.md`](../KyndeBlade_Godot/docs/GODOT_AUDIO_DESIGN.md) |
| Hi-bit ruin vista **reference** PNG | `KyndeBlade_Godot/assets/hi_bit_ruin_vista/reference_style_target.png` | *(see folder README — confirm author / license before Steam ship)* | **TBD** — treat as mood reference until cleared | Add URL + attribution when source confirmed; see [`hi_bit_ruin_vista/README.md`](../KyndeBlade_Godot/assets/hi_bit_ruin_vista/README.md) |
| Fair Field Lane B **concept** (optional combat backdrop) | `KyndeBlade_Godot/assets/concept/fair_field_lane_b_concept.png` | **AI-generated** mood board (image tool in dev workflow); not third-party scrape | **Project-generated — review before retail** | Replace or supplement with human-authored licensed art for ship; toggle via `combat.tscn` → `ManuscriptBackdrop` → **Paint mood texture only** |
| Historiated margin motif (pixel UI) | `KyndeBlade_Godot/assets/ui_manuscript/historiated_margin_crowned_figure.png` | Derivative: **crop** (right margin), **nearest-neighbor** resize, **palette quantize** to `KyndeBladeArtPalette` colors — from a **staging** manuscript-fragment photograph in project reference set (not a retail digitization record). | **Placeholder pipeline proof** — confirm with an **open-access** shelfmark + scan terms before Steam; see [`ui_manuscript/README.md`](../KyndeBlade_Godot/assets/ui_manuscript/README.md) | Replace or supplement with documented BL / institutional open-access source when locking ship art. |
| **Excluded (do not vendor)** | — | **Modern trade book covers** (e.g. copyrighted edition jackets), **third-party game screenshots**, **uncleared** `.cursor/.../assets/` drops | **N/A — not in repo** | Use **composition / mood only**; do not commit to `export_presets` / player builds. |

## Engine-authored assets

Gradients or textures authored only in Godot (no external file) need no third-party row; note in scene/resource comments if useful.

## Free packs (recommended pools, not vendored)

Use CC0 / clearly licensed art and audio per [`MUSIC_SOURCES.md`](MUSIC_SOURCES.md) and [`ART_DIRECTION.md`](../ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets/KyndeBlade/Docs/ART_DIRECTION.md). Common sources: **OpenGameArt** (filter by license), **Kenney.nl** CC0 packs, **itch.io** CC0-tagged assets — always keep `LICENSE.txt` in-repo.
