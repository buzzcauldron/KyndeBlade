extends RefCounted
class_name CrawlParallax
## **Crawl / exploration** parallax: slow multi-layer drift for hub and future route maps,
## plus Lane B combat backdrop. Sinusoidal motion (no tile wrap) — reads as living vista.

enum Layer {
	SKY,
	FAR_SILHOUETTE,
	MID_SILHOUETTE,
	RUIN_MASS,
	MIST_WASH,
	GROUND,
	TOWER,
	FOREGROUND,
	DITHER,
	FOLK_DUST,
}

const _FREQ_X := [0.055, 0.092, 0.118, 0.152, 0.088, 0.195, 0.125, 0.228, 0.065, 0.168]
const _AMP_X := [2.8, 9.0, 15.5, 24.0, 6.0, 32.0, 13.0, 38.0, 4.0, 20.0]
const _FREQ_Y := [0.032, 0.048, 0.055, 0.062, 0.04, 0.072, 0.05, 0.085, 0.028, 0.06]
const _AMP_Y := [1.2, 2.5, 3.2, 4.0, 1.8, 5.5, 2.8, 6.5, 0.9, 3.5]


## Horizontal + vertical offset (pixels) for this logical depth at `time_sec`.
static func offset(time_sec: float, layer: Layer, speed_scale: float = 1.0) -> Vector2:
	var i: int = int(layer)
	if i < 0 or i >= _FREQ_X.size():
		return Vector2.ZERO
	var sc: float = maxf(0.0, speed_scale)
	var ox: float = sin(time_sec * _FREQ_X[i] * sc) * _AMP_X[i]
	var oy: float = cos(time_sec * _FREQ_Y[i] * sc) * _AMP_Y[i]
	return Vector2(ox, oy)


## Hub crawl uses fewer layers; map enum subset to similar depths.
static func hub_band_offset(time_sec: float, band_index: int, speed_scale: float = 1.0) -> Vector2:
	var layer: Layer = Layer.SKY
	match band_index:
		0:
			layer = Layer.SKY
		1:
			layer = Layer.FAR_SILHOUETTE
		2:
			layer = Layer.MID_SILHOUETTE
		3:
			layer = Layer.MIST_WASH
		_:
			layer = Layer.GROUND
	return offset(time_sec, layer, speed_scale)
