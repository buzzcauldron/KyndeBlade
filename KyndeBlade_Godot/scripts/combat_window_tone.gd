extends RefCounted
class_name CombatWindowTone
## Procedural combat SFX (no binary assets). Policy: [`docs/GODOT_AUDIO_DESIGN.md`](../docs/GODOT_AUDIO_DESIGN.md) + repo `docs/ASSET_LICENSES.md`.

const MIX_RATE := 22050


static func make_tone_hz(freq_hz: float, duration_sec: float, peak_linear: float = 0.22) -> AudioStreamWAV:
	var sample_count: int = maxi(1, int(MIX_RATE * duration_sec))
	var data := PackedByteArray()
	data.resize(sample_count * 2)
	var phase := 0.0
	var phase_inc: float = TAU * freq_hz / float(MIX_RATE)
	for i in sample_count:
		var env: float = (
				1.0 - abs((float(i) / float(sample_count - 1)) * 2.0 - 1.0)
				if sample_count > 1 else 1.0
		)
		var s: float = sin(phase) * env * peak_linear
		phase += phase_inc
		var s16: int = int(clampf(s, -1.0, 1.0) * 32000.0)
		data[i * 2] = s16 & 0xFF
		data[i * 2 + 1] = (s16 >> 8) & 0xFF
	var stream := AudioStreamWAV.new()
	stream.format = AudioStreamWAV.FORMAT_16_BITS
	stream.mix_rate = MIX_RATE
	stream.stereo = false
	stream.data = data
	return stream


## Deep kill / wound impact: sub-heavy thump + short decay + noise (project-authored PCM; see `docs/GODOT_AUDIO_DESIGN.md`).
static func make_enemy_kill_impact() -> AudioStreamWAV:
	const RATE := 44100
	const DURATION := 0.58
	var sample_count: int = maxi(1, int(RATE * DURATION))
	var data := PackedByteArray()
	data.resize(sample_count * 2)
	var phase := 0.0
	var phase_inc: float = TAU * 52.0 / float(RATE)
	var rng_seed: int = 0xC0FFEE
	for i in sample_count:
		var t: float = float(i) / float(max(1, sample_count - 1))
		var boom_env: float = exp(-4.2 * t)
		var thump: float = sin(phase) * boom_env * 0.44
		phase += phase_inc * (1.0 - 0.38 * t)
		rng_seed = rng_seed * 1103515245 + 12345
		var noise: float = (float(rng_seed & 0xFFFF) / 32768.0 - 1.0) * boom_env * 0.11
		var ring: float = sin(phase * 3.71) * exp(-22.0 * t) * 0.07
		var s: float = clampf(thump + noise + ring, -1.0, 1.0)
		var s16: int = int(s * 30000.0)
		data[i * 2] = s16 & 0xFF
		data[i * 2 + 1] = (s16 >> 8) & 0xFF
	var stream := AudioStreamWAV.new()
	stream.format = AudioStreamWAV.FORMAT_16_BITS
	stream.mix_rate = RATE
	stream.stereo = false
	stream.data = data
	return stream
