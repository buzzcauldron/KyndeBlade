# Manuscript UI Theme
## Kynde Blade — Illuminated Manuscript Aesthetic

This document defines the **manuscript theme** for **all Kynde Blade UI only** (menus, HUD, dialogue, health bars, buttons). The game world visuals follow original medieval and Pre-Raphaelite style—see [VISUAL_DESIGN_ALAN_LEE.md](VISUAL_DESIGN_ALAN_LEE.md).

Inspired by medieval illuminated manuscripts such as Books of Hours.

**Primary reference**: `Assets/Art/Reference/manuscript_folio_reference.png` — richly decorated folio with Gothic script, miniature illustrations, and elaborate borders of vines, foliage, and floral motifs. Use as primary reference for mood, palette, and composition.

---

## Core Aesthetic Principles

**Manuscript Characteristics (from reference):**
- **Parchment**: Off-white/cream aged background; slightly textured, warm base
- **Ink**: Dense blackletter (Gothic) script; predominantly black ink
- **Illumination**: Gold for halos, miniature backgrounds, and border highlights
- **Rubrication**: Red ink for headings, initial letters, important sections
- **Blue accent**: Lapis/cerulean for secondary emphasis and miniature borders
- **Borders**: Intertwining vines, scrolling foliage, stylized flowers; gold dots and red accents; organic, flowing motifs
- **Miniatures**: Framed with gold backgrounds; thin blue and red borders
- **Texture**: Slightly uneven, handcrafted feel; subtle specks for authenticity

**Integration with 16-Bit:**
- Manuscript UI overlays the 16-bit sprite combat; the two aesthetics complement each other
- UI reads as "pages from a manuscript" while the game world remains pixel art
- Illumination moments (3D sphere) echo the "illuminated" quality of manuscript decoration

---

## Color Palette

*Derived from reference: aged parchment, black ink, gold illumination, red rubrication, lapis blue.*

### Backgrounds
| Name | Hex | Use |
|------|-----|-----|
| Parchment Light | #F5E6C8 | Primary panel background (off-white cream) |
| Parchment | #E8D4A8 | Secondary panels, cards |
| Parchment Aged | #D4C090 | Borders, dividers, vine tones |
| Parchment Dark | #C4A870 | Hover states, pressed |

### Text (Ink)
| Name | Hex | Use |
|------|-----|-----|
| Ink Primary | #1A1810 | Body text (blackletter black) |
| Ink Secondary | #3D2C1F | Muted text |
| Ink Light | #5C4A3A | Disabled, secondary text |

### Accents (Illumination)
| Name | Hex | Use |
|------|-----|-----|
| Gold | #D4A84B | Halos, highlights, current turn, emphasis |
| Gold Dark | #8B7355 | Vine work, decorative borders |
| Rubrication | #B8461A | Headings, important labels (vermillion) |
| Lapis Blue | #2E5A8C | Secondary emphasis, miniature borders |

### Borders & Decorative
| Name | Hex | Use |
|------|-----|-----|
| Border Sepia | #6B5344 | Panel borders |
| Border Blue | #3D6B9E | Miniature-style frames (with red) |
| Border Red | #8B3626 | Miniature-style frames (with blue) |
| Border Dark | #4A3A2E | Strong borders, eyelid tones |

---

## Typography

**Font Style (from reference):**
- **Primary**: Blackletter (Gothic) script—sharp angles, heavy strokes, decorative flourishes
- Fallback: Serif or Unity default
- **Body**: 14–16pt, Ink Primary (black)
- **Headings**: 18–24pt, Rubrication (red) or Gold
- **Labels**: 12–14pt, Ink Secondary or Lapis Blue
- **Emphasis**: Gold (illumination) or Rubrication (headings)

**Alignment:**
- Single column, generous margins (manuscript layout)
- Left-aligned for body; centered for titles

---

## Layout & Spacing

**Panel Structure:**
- Panels use a parchment background with a visible border (border color)
- Padding: 12–16px between content and edge
- Margins: 8–12px between sections

**Border Treatment:**
- 2–4px solid in Border Sepia or Parchment Aged
- Miniature-style: thin blue + red border (Border Blue, Border Red)
- Future: vine/floral sprite borders, gold dots, organic flourishes

---

## Component Guidelines

### Action Buttons
- Background: Parchment or Parchment Aged
- Border: Border Sepia
- Text: Ink Primary

### Turn Order Slots
- Background: Transparent or minimal
- Current turn: Gold text or Gold border
- Other: Ink Primary

### Health Bars (Manuscript Illustrations)
- **Track**: Parchment Aged — like manuscript margins
- **Fill (players)**: Verdigris (green-blue patina) — good/main characters; Gold when full
- **Fill (enemies)**: Vermillion (red) — bad/antagonist characters; Gold when full
- **Border**: Border Sepia — thin frame like miniature illustrations
- **Label**: Ink Primary, small numerals overlaid

### Victory/Defeat Panels
- Full-screen overlay with Parchment background
- Decorative border (optional vine/flourish)
- Title: Gold, large
- Body: Ink Primary

### Parry/Dodge Indicator
- Sclera: Parchment Light (instead of white)
- Pupil: Ink Primary
- Eyelids: Parchment Dark or Border Dark

### Goal Text / State Text
- Ink Primary

---

## Implementation

Use `ManuscriptUITheme` (static class) for:
- `ManuscriptUITheme.ParchmentLight`, `.InkPrimary`, etc.
- `ManuscriptUITheme.ApplyToText(Text t)`, `ApplyToButton(Button b)`, etc.

Apply theme on creation of UI elements in `CombatUI`, `GameStateManager`, and `ParryDodgeZoneIndicator`.

---

## Reference Assets (Future)

For full manuscript fidelity, consider:
- Parchment texture (subtle noise, slight discoloration) for panel backgrounds
- Decorative border sprites: vines, foliage, flowers, gold dots
- Custom font: Blackletter/Gothic (sharp, heavy strokes)
- Gold accent texture for highlights
- Optional: small insect/creature motifs (beetles, butterflies) as easter eggs
