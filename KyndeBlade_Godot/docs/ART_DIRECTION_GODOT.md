# Art direction — Godot Steam build

Godot mirrors the **markdown art bibles** in this repo using **code-driven colors and themes** (no new raster art in this pass).

## Source documents

| Doc | Role |
|-----|------|
| [`ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets/KyndeBlade/Docs/ART_DIRECTION.md`](../../ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets/KyndeBlade/Docs/ART_DIRECTION.md) | **Lane A** — vista / hub mood. **Lane B** — combat void, SNES-style contrast. |
| [`ProjectArchive/docs/UI_MANUSCRIPT_THEME.md`](../../ProjectArchive/docs/UI_MANUSCRIPT_THEME.md) | Manuscript UI: parchment, ink, gold, rubrication, lapis, borders. |
| [`ProjectArchive/docs/ART_DIRECTION_GRIMDARK_MEDIEVAL.md`](../../ProjectArchive/docs/ART_DIRECTION_GRIMDARK_MEDIEVAL.md) | Boss palette hints (exposed as constants for future bars / chips). |
| [`ProjectArchive/docs/VISUAL_DESIGN_ALAN_LEE.md`](../../ProjectArchive/docs/VISUAL_DESIGN_ALAN_LEE.md) | High-level Salome + Lee pairing (narrative direction; Godot uses Lane + manuscript first). |
| [`ProjectArchive/docs/ART_DIRECTION_VISUAL_BIBLE.md`](../../ProjectArchive/docs/ART_DIRECTION_VISUAL_BIBLE.md) | Pre-Raphaelite / Salome prompts (jewel tones, dramatic stillness, visceral edge). |
| **Hi-bit ruin vista** (Lane B / exploration mood) | [`assets/hi_bit_ruin_vista/README.md`](../assets/hi_bit_ruin_vista/README.md) + [`reference_style_target.png`](../assets/hi_bit_ruin_vista/reference_style_target.png) |
| **Salome / jewel refs (optional PNGs)** | [`assets/reference_preraphaelite/README.md`](../assets/reference_preraphaelite/README.md) |
| **Photographic mood (external, no assets committed)** | [sethcoastisthebestcoast on Instagram](https://www.instagram.com/sethcoastisthebestcoast/) — see subsection below |

### Photographic mood — coastal / atmosphere (optional)

Use [this Instagram feed](https://www.instagram.com/sethcoastisthebestcoast/) as **loose** inspiration for **light, weather, horizon, and texture** — not as assets to paste into the build.

**How to steal like an art director (not like a lawyer):**

- **Hub / Lane A twilight** — flat or bruised skies, sea haze, low sun behind cloud: nudge `hub_map` mist, crawl parallax opacity, and title gold vs body ink contrast.
- **Hi-bit ruin vista (Lane B)** — cool grey–teal vs warm stone, wet sheen, distant weather: inform `HI_BIT_*` bands and parallax layer weights in [`combat_manuscript_backdrop.gd`](../scripts/ui/combat_manuscript_backdrop.gd) (still procedural).
- **Jewel contamination** — when a shot has a **single sick saturated accent** in an otherwise drained palette, that maps to `JEWEL_*` / `jewel_wash_strength`: one wrong note, not rainbow noise.

**Rights:** Instagram photos are **not** cleared for redistribution. Do **not** commit screenshots or traced rasters unless you have explicit permission and a row in [`docs/ASSET_LICENSES.md`](../../docs/ASSET_LICENSES.md). Derive **palette and mood** only, or negotiate use with the photographer.

### Salome / jewel layer (Lane B figures + UI contamination)

Godot encodes a **second palette pass** on top of hi-bit sky + manuscript UI:

- **Constants:** `JEWEL_CRIMSON`, `JEWEL_EMERALD`, `JEWEL_ULTRAMARINE`, `JEWEL_VIOLET_SHADOW`, `SICKLY_HIGHLIGHT` in [`kyndeblade_art_palette.gd`](../scripts/ui/kyndeblade_art_palette.gd); `RUBRICATION` / `BORDER_RED` nudged deeper for margin-beast read.
- **Combat figures:** [`combat_presentation.gd`](../scripts/combat/combat_presentation.gd) — real swing: crimson→violet modulate + **reduced idle motion** (tableau); feint: **emerald** fringe; enemy hull mixed with `JEWEL_CRIMSON`. On defensive window open, a short **telegraph hold** (~50 ms) freezes bob/breath so the read snaps in before motion resumes.
- **Backdrop:** [`combat_manuscript_backdrop.gd`](../scripts/ui/combat_manuscript_backdrop.gd) — `@export jewel_wash_strength`: low-alpha crimson→violet strips over sky (tunable / can set 0). `@export jewel_wash_ultramarine_mix` pulls the wash end toward **ultramarine** for a cooler contaminated sky.
- **Theme:** [`kyndeblade_manuscript_theme.gd`](../scripts/ui/kyndeblade_manuscript_theme.gd) — button **hover** and **focus** borders + panel border pick up jewel/ultramarine/violet.

`HI_BIT_*` stays the **ruin vista** read; `JEWEL_*` adds Pre-Raphaelite **pathos / wrong** without replacing the exploration sky.

### Hi-bit style (in addition to manuscript + SNES Lane B)

Target: **high-fidelity pixel** with **sage / dusty teal / terracotta** stone, **peach–teal dawn sky**, soft **dither**, **4–5 parallax layers**, overgrown **ruins at heroic scale** (viaduct, piers), tiny figure read. Fits dream-vision, *Piers* / Green Knight decay, and ink combat in the foreground.

- **Palette constants:** `KyndeBladeArtPalette` in [`scripts/ui/kyndeblade_art_palette.gd`](../scripts/ui/kyndeblade_art_palette.gd) — **HI_BIT_*** from [`reference_style_target.png`](../assets/hi_bit_ruin_vista/reference_style_target.png) with **boosted saturation/contrast**; manuscript **GOLD / RUBRICATION / LAPIS / PARCHMENT** tuned to match that illumination read.
- **Combat backdrop:** [`scripts/ui/combat_manuscript_backdrop.gd`](../scripts/ui/combat_manuscript_backdrop.gd) — procedural approximation; swap in a full `TextureRect` using the PNG when ship license is cleared.

## Placeholder actors & level backdrops (art bible → data → Godot)

Procedural **silhouettes** and **location backdrops** mirror Lane A / Lane B / jewel / hi-bit language from the markdown bibles above (no new raster art).

| Piece | Role |
|-------|------|
| [`data/art/placeholder_registry.json`](../data/art/placeholder_registry.json) | Per **location**: `preset`, `jewel_wash`, `preview_character`. Per **character** id: `silhouette` + bible lane note. |
| [`scripts/art/placeholder_art_registry.gd`](../scripts/art/placeholder_art_registry.gd) | Loads JSON, resolves color tokens, **`validate_coverage()`** vs `LocationRegistry`. |
| [`scripts/art/placeholder_silhouette_library.gd`](../scripts/art/placeholder_silhouette_library.gd) | `Polygon2D` recipes: Wille, False, Green Knight, Mede, Wrath, Hunger, Piers, Orfeo, crowd, grace kneeler. |
| [`scripts/art/placeholder_actor_2d.gd`](../scripts/art/placeholder_actor_2d.gd) | `character_id` → rebuild figure. |
| [`scripts/art/placeholder_location_backdrop.gd`](../scripts/art/placeholder_location_backdrop.gd) | `_draw` presets (mist, hi-bit field, dungeon void, Orfeo violet, etc.). |
| [`addons/kyndeblade_placeholders/`](../addons/kyndeblade_placeholders/) | **Editor plugin** — *Project → Tools → KyndeBlade: Validate art placeholders*. |
| [`scenes/placeholders/character_placeholder.tscn`](../scenes/placeholders/character_placeholder.tscn) | Drop-in instance for authoring. |

[`scenes/world/location_shell.tscn`](../scenes/world/location_shell.tscn) composes backdrop + bottom **preview** silhouette per location.

## Implementation (this project)

| Asset | File |
|-------|------|
| Hex → `Color` constants | [`scripts/ui/kyndeblade_art_palette.gd`](../scripts/ui/kyndeblade_art_palette.gd) (`class_name KyndeBladeArtPalette`) |
| Manuscript `Theme` + progress bar fills | [`scripts/ui/kyndeblade_manuscript_theme.gd`](../scripts/ui/kyndeblade_manuscript_theme.gd) (`class_name KyndeBladeManuscriptTheme`) |

**Applied in:**

- **Main menu** — full **manuscript page** (`main_menu.tscn`): parchment field, framed `PanelContainer` sheet, historiated margin motif, rubric rule, ink subtitle; settings on parchment dialog over scrim; slider/check styling in `KyndeBladeManuscriptTheme` (`main_menu.gd`).
- **Hub** — twilight backdrop, manuscript theme, vista title colors (`hub_map.gd`).
- **Tower intro / arrival** — mist backdrop, gold title, lapis speaker, ink body (`story_arrival_screen.gd`).
- **Combat** — Lane B **hi-bit-style** procedural backdrop with **crawl parallax** (`combat.tscn` `BackdropLayer` → `ManuscriptBackdrop`, [`crawl_parallax.gd`](../scripts/hub/crawl_parallax.gd)); manuscript UI, gold/rubric/lapis bars (`combat_root.gd`).
- **Hub (crawl)** — [`hub_crawl_parallax.gd`](../scripts/hub/hub_crawl_parallax.gd) on `hub_map.tscn` (`CrawlParallaxBackdrop`).

**High-bit bonus** (`hi_bit_bonus_level.tscn`) keeps its own pixel-room palette; align void/sky to `COMBAT_VOID` / `LAPIS` later if desired.

## Next art steps (not automated here)

- Drop Lane A vista PNGs under a Godot `assets/` folder (see Unity `ART_DIRECTION.md` reference paths).
- Optional **pixel** combat sprites with documented **PPU** and integer scale (Lane B).
- Blackletter font asset + license row in [`docs/ASSET_LICENSES.md`](../../docs/ASSET_LICENSES.md).
