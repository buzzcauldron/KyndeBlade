# 16-Bit Spinoff Pipeline

Optional pipeline that renders the game at low resolution (SNES-style), point upscales to the screen, then applies the **same manuscript overlay** (parchment, grain, sepia) used in the main stylized 3D pipeline.

## Components

- **SixteenBitPipeline** (`KyndeBlade/Code/Core/Visual/SixteenBitPipeline.cs`)  
  Add to the **Main Camera**. When enabled:
  1. Camera renders the scene to a low-res `RenderTexture` (default **256×224**).
  2. That buffer is blitted to the screen with **point filtering** (pixel-perfect upscale).
  3. The **Manuscript Overlay** (earlier full-screen code) is applied on top: parchment multiply, grain, sepia (same params as `ManuscriptOverlayEffect` / `ManuscriptOverlayParams`).

- **KyndeBlade/Point Upscale (16-Bit)** shader  
  Used for the point-sampled blit so pixels stay sharp when upscaling.

- **SixteenBitConstants**  
  Static values: `NativeWidth`/`NativeHeight` (256×224), `HdWidth`/`HdHeight` (512×448), `MaxColorsOnScreen` (256). Use for UI reference resolution or palette logic when 16-bit mode is on.

## Usage

1. Add **SixteenBitPipeline** to the Main Camera (e.g. the one created by `KyndeBladeGameManager.EnsureCombatCamera()`).
2. Optionally disable **ManuscriptOverlayEffect** on that camera when using 16-bit (the pipeline applies overlay itself).
3. Set **Native Size** (e.g. 256×224 or 512×448). Toggle **Apply Manuscript Overlay** and tune parchment/sepia/grain as needed.

## Relation to main pipeline

- **Main pipeline**: Full-res 3D + manuscript overlay (and toon/satin/gold/DoF as implemented).
- **16-bit spinoff**: Same game scene, but rendered at low res → point upscale → same manuscript overlay. Reuses `ManuscriptOverlayParams` and the manuscript full-screen shader; no separate overlay component needed on the camera when this pipeline is active.

## Art direction

See **VISUAL_DESIGN_ALAN_LEE.md** (§ 16-Bit Technical Framework) for resolution, palette, and sprite constraints when targeting this pipeline.
