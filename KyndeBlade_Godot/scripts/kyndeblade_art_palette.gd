class_name KyndeBladeArtPalette
## Documented colors for KyndeBlade Godot UI / backdrops.
##
## **Primary visual reference:** [`assets/hi_bit_ruin_vista/reference_style_target.png`](../assets/hi_bit_ruin_vista/reference_style_target.png) — hi-bit ruin vista (misty teal sky, coral/peach clouds, lime foliage, terracotta stone, slate shadows).
## This palette is a **high-contrast, higher-saturation** interpretation of that image so Lane A manuscript UI and Lane B combat read clearly at 960×540.
##
## Also aligned with repo docs:
## - [ART_DIRECTION.md](../../KyndeBlade_Unity/Assets/KyndeBlade/Docs/ART_DIRECTION.md)
## - [UI_MANUSCRIPT_THEME.md](../../ProjectArchive/docs/UI_MANUSCRIPT_THEME.md)
## - [ART_DIRECTION_GRIMDARK_MEDIEVAL.md](../../ProjectArchive/docs/ART_DIRECTION_GRIMDARK_MEDIEVAL.md)

# --- Lane B — combat (void anchors to slate-shadows in reference; stronger than old muted navy) ---
const COMBAT_VOID := Color("#050910")
const COMBAT_VOID_COOL := Color("#0a1420")
const COMBAT_UI_SCRIM := Color(0.02, 0.04, 0.08, 0.58)

# --- Lane A — hub / vista (mist ramp: cool teal-grey atmosphere from midground mist) ---
const HUB_TWILIGHT := Color("#0e1618")
const HUB_MIST := Color("#1a2830")
const VISTA_GOLD_TITLE := Color("#f4b82e")
const VISTA_BODY := Color("#d2c4a8")

# --- Manuscript — backgrounds (warm vellum lit by peach sky; higher key for contrast with ink) ---
const PARCHMENT_LIGHT := Color("#fff4dc")
const PARCHMENT := Color("#f2e0bc")
const PARCHMENT_AGED := Color("#deb882")
const PARCHMENT_DARK := Color("#c99858")

# --- Manuscript — ink (near-black brown; WCAG-friendly against parchment ramp) ---
const INK_PRIMARY := Color("#0a0806")
const INK_SECONDARY := Color("#2a2018")
const INK_LIGHT := Color("#5c4838")

# --- Manuscript — illumination & accents (saturated gold / vermillion / lapis) ---
const GOLD := Color("#e8a010")
const GOLD_DARK := Color("#a06820")
const RUBRICATION := Color("#d42010")
const LAPIS := Color("#1858c8")

# --- Manuscript — borders (stronger chroma for panel edges) ---
const BORDER_SEPIA := Color("#5c4030")
const BORDER_BLUE := Color("#2050a0")
const BORDER_RED := Color("#a02018")
const BORDER_DARK := Color("#2a1810")

# --- Boss tints (grimdark doc) — UI chips / future bars; sat bumped ---
const BOSS_PRIDE := Color("#602878")
const BOSS_HUNGER := Color("#d8d0c8")
const BOSS_GREEN_KNIGHT := Color("#228040")

# --- Hi-bit ruin vista (from reference PNG — sunset / overgrowth / stone / mist ramps, chroma boosted) ---
## Sky: saturated teal → mint → coral → apricot glow (matches cloud band warmth in reference).
const HI_BIT_SKY_TEAL := Color("#42c4b0")
const HI_BIT_SKY_MIST := Color("#7adcc8")
const HI_BIT_SKY_PEACH := Color("#ff8c58")
const HI_BIT_SKY_GLOW := Color("#ffc090")
## Distance: darker teal-greens for atmospheric contrast against bright sky.
const HI_BIT_SILHOUETTE_FAR := Color("#3d7068")
const HI_BIT_SILHOUETTE_MID := Color("#255850")
## Ground / moss: forest → lime foliage (foreground vines in reference).
const HI_BIT_SAGE := Color("#4a8840")
const HI_BIT_SAGE_DEEP := Color("#143228")
const HI_BIT_TEAL_SHADOW := Color("#0c2420")
## Masonry: warm terracotta brick.
const HI_BIT_TERRACOTTA := Color("#c85838")
const HI_BIT_TERRACOTTA_DEEP := Color("#702818")
const HI_BIT_FOLIAGE := Color("#2fa060")
## Soft dither speckle; warm tint from sky glow.
const HI_BIT_DITHER := Color(1.0, 0.88, 0.72, 0.14)
