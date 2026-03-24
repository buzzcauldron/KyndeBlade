extends Control
## Unity `ParryDodgeZoneIndicator`–style phased eye + `React!` countdown (presentation only).

const IMMINENT_PHASE_START := 0.75
const PUPIL_START_SCALE := 1.0
const PUPIL_STRIKE_SCALE := 0.25
const EYELID_CLOSE_AMOUNT := 0.35

var _combat: CombatManager

@onready var pupil_root: Control = %PupilRoot
@onready var react_label: Label = %ReactLabel
@onready var eyelid_top: Control = %EyelidTop
@onready var eyelid_bottom: Control = %EyelidBottom


func setup(combat: CombatManager) -> void:
	_combat = combat
	visible = false


func _process(_delta: float) -> void:
	if _combat == null:
		visible = false
		return
	var active := _combat.state == CombatManager.State.REAL_TIME_WINDOW
	visible = active
	if not active:
		return
	var t: float = _combat.window_phase_t()
	var pupil_scale := PUPIL_START_SCALE
	var close_norm := 0.0
	if t >= IMMINENT_PHASE_START:
		var denom := 1.0 - IMMINENT_PHASE_START
		var imminent_t := smoothstep(0.0, 1.0, (t - IMMINENT_PHASE_START) / denom) if denom > 0.0001 else 1.0
		pupil_scale = lerpf(PUPIL_START_SCALE, PUPIL_STRIKE_SCALE, imminent_t)
		close_norm = imminent_t
	var close := close_norm * EYELID_CLOSE_AMOUNT
	pupil_root.scale = Vector2(pupil_scale, pupil_scale)
	var lid_px := close * 20.0
	eyelid_top.offset_bottom = 14.0 + lid_px
	eyelid_bottom.offset_top = -14.0 - lid_px
	react_label.text = "React! %.1fs" % maxf(0.0, _combat.window_remaining)
