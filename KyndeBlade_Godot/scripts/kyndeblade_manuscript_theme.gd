class_name KyndeBladeManuscriptTheme
## Builds a [Theme] from manuscript UI colors in [KyndeBladeArtPalette].
## See [UI_MANUSCRIPT_THEME.md](../../ProjectArchive/docs/UI_MANUSCRIPT_THEME.md).


static func _flat(bg: Color, border: Color, width: int = 2) -> StyleBoxFlat:
	var s := StyleBoxFlat.new()
	s.bg_color = bg
	s.border_color = border
	s.set_border_width_all(width)
	s.set_corner_radius_all(3)
	return s


static func build_theme() -> Theme:
	var th := Theme.new()
	var n := _flat(KyndeBladeArtPalette.PARCHMENT, KyndeBladeArtPalette.BORDER_SEPIA)
	## Hover / focus: jewel-tinged borders (Salome / Pre-Raphaelite layer); keep fills manuscript.
	var h := _flat(
			KyndeBladeArtPalette.PARCHMENT_DARK,
			KyndeBladeArtPalette.JEWEL_CRIMSON.lerp(KyndeBladeArtPalette.GOLD_DARK, 0.35),
			2
	)
	var p := _flat(KyndeBladeArtPalette.PARCHMENT_AGED, KyndeBladeArtPalette.BORDER_DARK)
	var focus_border: Color = KyndeBladeArtPalette.JEWEL_ULTRAMARINE.lerp(KyndeBladeArtPalette.JEWEL_VIOLET_SHADOW, 0.4)
	focus_border = focus_border.lerp(KyndeBladeArtPalette.SICKLY_HIGHLIGHT, 0.1)
	var focus_b := _flat(KyndeBladeArtPalette.PARCHMENT, focus_border, 2)
	th.set_stylebox("normal", "Button", n)
	th.set_stylebox("hover", "Button", h)
	th.set_stylebox("pressed", "Button", p)
	th.set_stylebox("focus", "Button", focus_b)
	th.set_stylebox("disabled", "Button", _flat(KyndeBladeArtPalette.PARCHMENT_AGED, KyndeBladeArtPalette.INK_LIGHT, 1))
	th.set_color("font_color", "Button", KyndeBladeArtPalette.INK_PRIMARY)
	th.set_color("font_hover_color", "Button", KyndeBladeArtPalette.INK_PRIMARY)
	th.set_color("font_pressed_color", "Button", KyndeBladeArtPalette.INK_PRIMARY)
	th.set_color("font_disabled_color", "Button", KyndeBladeArtPalette.INK_LIGHT)

	# Default labels: ink on transparent (panels provide parchment where needed)
	th.set_color("font_color", "Label", KyndeBladeArtPalette.INK_PRIMARY)
	th.set_color("font_shadow_color", "Label", Color(0, 0, 0, 0.25))
	th.set_constant("shadow_offset_x", "Label", 1)
	th.set_constant("shadow_offset_y", "Label", 1)

	# Progress bars: aged parchment track, fills per role (overridden in combat_root for per-bar tints)
	var track := _flat(KyndeBladeArtPalette.PARCHMENT_AGED, KyndeBladeArtPalette.BORDER_SEPIA, 1)
	track.set_corner_radius_all(2)
	var fill_player := _flat(KyndeBladeArtPalette.GOLD, KyndeBladeArtPalette.GOLD_DARK, 1)
	fill_player.set_corner_radius_all(2)
	th.set_stylebox("background", "ProgressBar", track)
	th.set_stylebox("fill", "ProgressBar", fill_player)

	# Panels (hub flavor, settings): lapis with a violet undertone for manuscript “frame” read.
	var panel_border: Color = KyndeBladeArtPalette.BORDER_BLUE.lerp(KyndeBladeArtPalette.JEWEL_VIOLET_SHADOW, 0.25)
	var panel := _flat(KyndeBladeArtPalette.PARCHMENT_LIGHT, panel_border)
	th.set_stylebox("panel", "PanelContainer", panel)

	return th


static func style_progress_bar(bar: ProgressBar, fill: Color, border: Color) -> void:
	var track := _flat(KyndeBladeArtPalette.PARCHMENT_AGED, KyndeBladeArtPalette.BORDER_SEPIA, 1)
	track.set_corner_radius_all(2)
	var f := _flat(fill, border, 1)
	f.set_corner_radius_all(2)
	bar.add_theme_stylebox_override("background", track)
	bar.add_theme_stylebox_override("fill", f)
