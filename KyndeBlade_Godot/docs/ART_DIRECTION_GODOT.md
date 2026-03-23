# Art direction — Godot Steam build

Godot mirrors the **markdown art bibles** in this repo using **code-driven colors and themes** (no new raster art in this pass).

## Source documents

| Doc | Role |
|-----|------|
| [`KyndeBlade_Unity/Assets/KyndeBlade/Docs/ART_DIRECTION.md`](../../KyndeBlade_Unity/Assets/KyndeBlade/Docs/ART_DIRECTION.md) | **Lane A** — vista / hub mood. **Lane B** — combat void, SNES-style contrast. |
| [`ProjectArchive/docs/UI_MANUSCRIPT_THEME.md`](../../ProjectArchive/docs/UI_MANUSCRIPT_THEME.md) | Manuscript UI: parchment, ink, gold, rubrication, lapis, borders. |
| [`ProjectArchive/docs/ART_DIRECTION_GRIMDARK_MEDIEVAL.md`](../../ProjectArchive/docs/ART_DIRECTION_GRIMDARK_MEDIEVAL.md) | Boss palette hints (exposed as constants for future bars / chips). |
| [`ProjectArchive/docs/VISUAL_DESIGN_ALAN_LEE.md`](../../ProjectArchive/docs/VISUAL_DESIGN_ALAN_LEE.md) | High-level Salome + Lee pairing (narrative direction; Godot uses Lane + manuscript first). |
| **Hi-bit ruin vista** (Lane B / exploration mood) | [`assets/hi_bit_ruin_vista/README.md`](../assets/hi_bit_ruin_vista/README.md) + [`reference_style_target.png`](../assets/hi_bit_ruin_vista/reference_style_target.png) |

### Hi-bit style (in addition to manuscript + SNES Lane B)

Target: **high-fidelity pixel** with **sage / dusty teal / terracotta** stone, **peach–teal dawn sky**, soft **dither**, **4–5 parallax layers**, overgrown **ruins at heroic scale** (viaduct, piers), tiny figure read. Fits dream-vision, *Piers* / Green Knight decay, and ink combat in the foreground.

- **Palette constants:** `KyndeBladeArtPalette` in [`scripts/kyndeblade_art_palette.gd`](../scripts/kyndeblade_art_palette.gd) — **HI_BIT_*** from [`reference_style_target.png`](../assets/hi_bit_ruin_vista/reference_style_target.png) with **boosted saturation/contrast**; manuscript **GOLD / RUBRICATION / LAPIS / PARCHMENT** tuned to match that illumination read.
- **Combat backdrop:** [`scripts/combat_manuscript_backdrop.gd`](../scripts/combat_manuscript_backdrop.gd) — procedural approximation; swap in a full `TextureRect` using the PNG when ship license is cleared.

## Implementation (this project)

| Asset | File |
|-------|------|
| Hex → `Color` constants | [`scripts/kyndeblade_art_palette.gd`](../scripts/kyndeblade_art_palette.gd) (`class_name KyndeBladeArtPalette`) |
| Manuscript `Theme` + progress bar fills | [`scripts/kyndeblade_manuscript_theme.gd`](../scripts/kyndeblade_manuscript_theme.gd) (`class_name KyndeBladeManuscriptTheme`) |

**Applied in:**

- **Main menu** — manuscript buttons + settings parchment scrim; Lane A vignette; title gold / subtitle body (`main_menu.gd`).
- **Hub** — twilight backdrop, manuscript theme, vista title colors (`hub_map.gd`).
- **Tower intro / arrival** — mist backdrop, gold title, lapis speaker, ink body (`story_arrival_screen.gd`).
- **Combat** — Lane B **hi-bit-style** procedural backdrop with **crawl parallax** (`combat.tscn` `BackdropLayer` → `ManuscriptBackdrop`, [`crawl_parallax.gd`](../scripts/crawl_parallax.gd)); manuscript UI, gold/rubric/lapis bars (`combat_root.gd`).
- **Hub (crawl)** — [`hub_crawl_parallax.gd`](../scripts/hub_crawl_parallax.gd) on `hub_map.tscn` (`CrawlParallaxBackdrop`).

**High-bit bonus** (`hi_bit_bonus_level.tscn`) keeps its own pixel-room palette; align void/sky to `COMBAT_VOID` / `LAPIS` later if desired.

## Next art steps (not automated here)

- Drop Lane A vista PNGs under a Godot `assets/` folder (see Unity `ART_DIRECTION.md` reference paths).
- Optional **pixel** combat sprites with documented **PPU** and integer scale (Lane B).
- Blackletter font asset + license row in [`docs/ASSET_LICENSES.md`](../../docs/ASSET_LICENSES.md).
