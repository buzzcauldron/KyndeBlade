# Art Direction: Grimdark Medieval

*Dark Souls meets 14th-century illuminated manuscripts. Reference for palette, character, environment, and UI.*

**Primary artistic references:** Let **the Salome image** (Pre-Raphaelite character, jewel tones, pathos) and **Alan Lee art** (environment, atmosphere, natural detail) strongly guide the look. See [ART_DIRECTION_VISUAL_BIBLE.md](ART_DIRECTION_VISUAL_BIBLE.md) and [VISUAL_DESIGN_ALAN_LEE.md](VISUAL_DESIGN_ALAN_LEE.md).

## Palette (Sin-Specific)

| Boss           | Primary              | Secondary   | Material / texture notes                    |
|----------------|----------------------|------------|---------------------------------------------|
| **Pride**      | Imperial purple      | Gold       | Polished plate, mirror-like; velvet capes  |
| **Hunger**     | Pale grey, off-white | —          | Bone, stretched leather, sunken eyes        |
| **Green Knight** | Deep moss, copper  | Rust       | Overgrown iron, vines, decaying leaves     |

Use **BossPalette** asset (Create → KyndeBlade → Boss Palette) in code for health bars, VFX tints, and manuscript accents.

## Character Design (Human Experience)

- **Green Knight:** Eco-brutalist. Armor held together by vines; massive rectangular silhouette; immovable, heavy. Deep moss and copper. Sinewy where it shows (e.g. nature reclaiming flesh, or aftermath of the blow); beheading game is weighty and bodily, not sanitized.
- **Hunger:** “Endless void” — armor too big, wasting away inside; gaping dark where the face is. Not fat; hollow.
- **Pride:** Most human but uncanny. Taller than normal; mask with a fixed condescending smile. Visceral only if the design calls for it (e.g. pride as exposure, vulnerability under the armor).

## Environment

- **Atmospheric fog:** Low-lying volumetric fog at feet; movements feel ghostly and turn-based.
- **Medieval surrealism:** Background like a tapestry (flat distant mountains); 3D grit in foreground.

## UI (Illuminated Manuscript)

- **Health:** Melting wax candles or ink bleeding into parchment (not sci-fi bars).
- **Typography:** Blackletter / Gothic for boss names; clean serif for numbers (readable damage).
- **Status effects:** Hunger = e.g. ghostly teeth over character or grayscale shift as vitality drops; not just icons.

## Engine (Unity oracle: Built-in RP — not URP)

The **Unity oracle project** ([`ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity`](../../ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity)) uses the **Built-in Render Pipeline** (no `com.unity.render-pipelines.universal` in [`Packages/manifest.json`](../../ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Packages/manifest.json)). Do **not** assume URP/HDRP unless you migrate the project.

**Art-adjacent packages actually in use (UPM ids):**

| Package id | Role |
|------------|------|
| `com.unity.feature.2d` | 2D feature set (sprites, etc.) |
| `com.unity.ugui` | uGUI (Canvas, legacy Text; project also uses built-in fonts in places) |
| `com.unity.inputsystem` | New Input System (combat UI module on EventSystem) |
| `com.unity.modules.ui` / `uielements` | Core UI modules |

**Post-processing / mood (Built-in):**

- Combat presentation is driven by code paths such as **`SixteenBitPipeline`** / manuscript-style effects on the **Main Camera**, not URP volumes. See [`ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets/KyndeBlade/Docs/ART_DIRECTION.md`](../../ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets/KyndeBlade/Docs/ART_DIRECTION.md) (Lane A/B) and [`ARCHITECTURE.md`](../../ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets/KyndeBlade/ARCHITECTURE.md).
- If you add **stack-style** post FX for Built-in, the historical UPM package is **`com.unity.postprocessing`** (verify compatibility with your Unity 6.x version in Package Manager). **Vista / hub** only: keep combat readable (Lane B = dark void, high contrast; avoid fighting the 16-bit pass).
- **Vignette / grain / bloom** as *design intent* below still apply as **goals**—implement via Built-in-compatible stack, shader, or `RawImage` overlays, not URP Volume assets unless you port the pipeline.

- **Vignette:** Claustrophobic focus on center (combat).
- **Film grain:** Old poem / ancient record feel.
- **Bloom:** Green Knight magic, Pride’s gold armor.

## Concept Art

- **Green Knight:** See `GreenKnight_GrimdarkConcept.png` (eco-brutalist, vines, moss, copper, rectangular silhouette).
