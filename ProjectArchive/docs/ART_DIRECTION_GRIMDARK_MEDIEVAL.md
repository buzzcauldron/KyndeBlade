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

## Engine (URP + Post-Processing)

- **Vignette:** Claustrophobic focus on center (combat).
- **Film grain:** Old poem / ancient record feel.
- **Bloom:** Green Knight magic, Pride’s gold armor.

## Concept Art

- **Green Knight:** See `GreenKnight_GrimdarkConcept.png` (eco-brutalist, vines, moss, copper, rectangular silhouette).
