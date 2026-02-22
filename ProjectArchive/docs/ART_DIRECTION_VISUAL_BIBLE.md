# Visual Bible: Prompts for Cursor (KyndeBlade)

Use this when asking Cursor to write **shaders**, **UI**, or **rendering** code so the aesthetic stays consistent.

---

## Primary Artistic References (Strong Guidance)

**Two references should strongly guide artistic direction:**

1. **The Salome image** — Pre-Raphaelite / Symbolist Salome (e.g. dramatic single figure, jewel tones, ornate costume, tragic stillness, symbolic objects). Use for: **character pose and pathos**, **jewel-like saturation** (crimson, emerald, ultramarine), **heavy fabric** (velvet, silk), **portrait-like focus**, and **emotional intensity** in key moments.
2. **Alan Lee art** — Tolkien / Faeries illustrator: ethereal atmosphere, detailed natural texture, medieval–fantasy mood, integrated character and environment. Use for: **environments**, **light and atmosphere**, **foliage and stone**, and **grounded-yet-mystical** tone.

When in doubt, ask: *Would this read as a Salome-like figure in a Lee-like world, inside a manuscript frame?*

---

## Master Prompt (Rendering / Shaders / UI)

> Implement a rendering style for KyndeBlade based on **Illuminated Manuscripts** and **Pre-Raphaelite** art. **Let the Salome image and Alan Lee art strongly guide the look**: Salome for character beauty, jewel tones, and pathos; Lee for environment, atmosphere, and natural detail.
>
> - **Manuscript elements:** Flat gold leaf textures, heavy ink-stroke outlines, and decorative "marginalia" borders for the UI.
> - **Pre-Raphaelite (Salome) elements:** Deep jewel tones (crimson, emerald, violet), naturalistic lighting on character faces, high-detail floral/organic motifs, and **dramatic stillness** (figures caught in moments of high drama or tragic beauty).
> - **Goal:** Characters should look like detailed oil paintings (Salome-like) trapped inside a flat, gold-embossed book page, in a world that feels drawn from Alan Lee.
> - **Visceral edge:** Where appropriate (Hunger, Green Knight, wounds, martyrdom), lean **sinewy and on the edge of gory**—suggested and stylized, not gratuitous.

---

## Shader Instructions

### Parchment overlay
> Create a shader that **multiplies** a grain/paper texture over the entire camera view (post-process or fullscreen quad). The result should look like the game is drawn on parchment.

### Gold leaf specular
> Write a shader so "Gold" materials respond to light with a **flat, metallic shine**—similar to real gold leaf on a manuscript page (not glossy 3D metal).

### Outline (ink stroke)
> Use an **inverted hull** or **Sobel filter** so characters get a **drawn ink outline** (heavy stroke, manuscript style).

---

## UI as Living Document

### Drop caps (boss names)
> Write a UI script that generates **Drop Caps** (large, decorative first letters) for boss names. When a boss speaks or is in focus, the first letter of their name should be a hand-drawn style element whose **color depends on their health** (e.g. from BossPalette, lerped by health ratio).

### Health bars (bleeding ink)
> Instead of a plain slider, implement a **"Bleeding Ink"** bar: as health drops, the ink **smudges** or **fades** into the parchment texture (soft edge, like ink soaking into paper).

---

## Animation (Human Experience)

### Long anticipation
> Adjust AnimationController / turn sequence so the Green Knight (and similar bosses) use **long anticipation**: hold a pose that looks like a **still painting for ~0.5 seconds** before the strike completes. Pre-Raphaelite pathos: frozen, dramatic pose before motion.
> - In KyndeBlade: set **TurnSequenceController.TelegraphDuration** to ~0.5 for boss encounters, and/or give boss actions **Cast Time** in their ScriptableObject so the wind-up holds before the hit.

### Parry slow-mo (dream-like)
> Create a **TimeManager** (or extend existing) that **slightly slows time** on **parry success**, and optionally apply a **Sepia or Golden Wash** filter to the screen to mimic an old manuscript moment.

---

## Vignette & Borders

### Decorative border overlay
> Write a script that applies a **Decorative Border Overlay** to the camera (or canvas). The border should change by context:
> - **Green Chapel:** border sprouts **vines** (Green Knight influence).
> - **Pride:** border becomes **rigid gold filigree**.
> Manuscripts are contained within borders; the frame is part of the art.

---

## Reference Files in Project

- **Salome image** + **Alan Lee art**: Primary artistic references (see above). Keep a Salome reference and Lee reference (e.g. LOTR/Hobbit/Faeries) visible when defining palettes, shaders, and character pose.
- **BossPalette** (ScriptableObject): Sin-specific colors for Pride, Hunger, Green Knight, etc.
- **ART_DIRECTION_GRIMDARK_MEDIEVAL.md**: Palette table, character notes, URP/post-processing.
- **VISUAL_DESIGN_ALAN_LEE.md**: Full Lee + Pre-Raphaelite + manuscript game-world guide.
- **ManuscriptUITheme** / **ManuscriptHealthBar**: Existing manuscript-style UI hooks.
