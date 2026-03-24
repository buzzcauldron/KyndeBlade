extends CanvasLayer
## Autoload: treat the game as **one manuscript** — scene moves are **turning leaves**;
## [method turn_page_to] with [param deep_folio] is for sinking into combat (ink / binding).

const _SLIDE_IN := 0.22
const _SLIDE_OUT := 0.2

var _busy: bool = false
var _blocker: Control
var _page: Control
var _sheet: ColorRect
var _spine: ColorRect


func _ready() -> void:
	layer = 120
	process_mode = Node.PROCESS_MODE_ALWAYS
	_build_overlay()
	_hide_overlay()


func _build_overlay() -> void:
	_blocker = Control.new()
	_blocker.set_anchors_preset(Control.PRESET_FULL_RECT)
	_blocker.mouse_filter = Control.MOUSE_FILTER_STOP
	_blocker.visible = false
	add_child(_blocker)

	_page = Control.new()
	_page.mouse_filter = Control.MOUSE_FILTER_IGNORE
	_blocker.add_child(_page)

	_sheet = ColorRect.new()
	_sheet.mouse_filter = Control.MOUSE_FILTER_IGNORE
	_page.add_child(_sheet)

	_spine = ColorRect.new()
	_spine.mouse_filter = Control.MOUSE_FILTER_IGNORE
	_page.add_child(_spine)


func _hide_overlay() -> void:
	_blocker.visible = false


func turn_page_to(scene_path: String, deep_folio: bool = false) -> void:
	if scene_path.is_empty():
		push_warning("ManuscriptNav: empty scene path")
		return
	if not ResourceLoader.exists(scene_path):
		push_error("ManuscriptNav: scene not found: %s" % scene_path)
		return
	if _busy:
		return
	_busy = true
	var vp: Vector2 = get_viewport().get_visible_rect().size
	if vp.x < 1.0 or vp.y < 1.0:
		vp = Vector2(960, 540)

	_page.position = Vector2(vp.x, 0)
	_page.size = vp
	_sheet.position = Vector2.ZERO
	_sheet.size = vp
	var ink: Color = (
			KyndeBladeArtPalette.PARCHMENT.lerp(KyndeBladeArtPalette.COMBAT_VOID, 0.42)
			if deep_folio
			else KyndeBladeArtPalette.PARCHMENT_LIGHT
	)
	_sheet.color = ink
	_spine.position = Vector2.ZERO
	_spine.size = Vector2(14, vp.y)
	_spine.color = KyndeBladeArtPalette.BORDER_DARK.lerp(ink, 0.45)
	_spine.visible = true

	_blocker.visible = true
	var tw_in := create_tween()
	tw_in.tween_property(_page, "position", Vector2.ZERO, _SLIDE_IN).set_trans(Tween.TRANS_QUINT).set_ease(
			Tween.EASE_OUT
	)
	await tw_in.finished

	get_tree().change_scene_to_file(scene_path)
	await get_tree().process_frame

	vp = get_viewport().get_visible_rect().size
	if vp.x < 1.0 or vp.y < 1.0:
		vp = Vector2(960, 540)
	_page.position = Vector2.ZERO
	_page.size = vp
	_sheet.size = vp
	_spine.size = Vector2(14, vp.y)

	var tw_out := create_tween()
	tw_out.tween_property(_page, "position", Vector2(-vp.x, 0), _SLIDE_OUT).set_trans(Tween.TRANS_QUINT).set_ease(
			Tween.EASE_IN
	)
	await tw_out.finished

	_hide_overlay()
	_busy = false


## Reload the current scene file (e.g. combat retry) as another leaf over the same quire.
func turn_page_reopen_current() -> void:
	var cur: Node = get_tree().current_scene
	if cur == null:
		_busy = false
		return
	var p: String = cur.scene_file_path
	if p.is_empty():
		_busy = false
		return
	await turn_page_to(p)
