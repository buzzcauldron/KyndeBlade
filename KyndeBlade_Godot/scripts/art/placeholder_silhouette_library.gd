class_name PlaceholderSilhouetteLibrary
## Procedural **manuscript / Lane B** silhouettes derived from [`ART_DIRECTION_GODOT.md`](../../docs/ART_DIRECTION_GODOT.md) + Unity [`ART_DIRECTION.md`](../../../ProjectArchive/UnityKyndeBlade/KyndeBlade_Unity/Assets/KyndeBlade/Docs/ART_DIRECTION.md). Used by [`PlaceholderActor2D`](placeholder_actor_2d.gd) and combat presentation.


static func clear_and_build(character_id: String, root: Node2D) -> void:
	for c in root.get_children():
		c.queue_free()
	var row: Dictionary = PlaceholderArtRegistry.character_entry(character_id)
	var key := str(row.get("silhouette", "wille_laborer"))
	match key:
		"wille_laborer":
			_build_wille(root)
		"margin_beast_false":
			_build_false(root)
		"green_knight_boss":
			_build_green_knight(root)
		"lady_mede_robes":
			_build_mede(root)
		"wrath_spike_burst":
			_build_wrath(root)
		"hunger_tall_thin":
			_build_hunger(root)
		"piers_field_labourer":
			_build_piers(root)
		"boundary_wanderer":
			_build_orfeo_wanderer(root)
		"field_of_folk_dots":
			_build_crowd(root)
		"still_kneeler":
			_build_grace_kneeler(root)
		_:
			_build_wille(root)


static func _poly(parent: Node2D, z: int, col: Color, pts: PackedVector2Array) -> Polygon2D:
	var p := Polygon2D.new()
	p.z_index = z
	p.color = col
	p.polygon = pts
	parent.add_child(p)
	return p


static func _build_wille(root: Node2D) -> void:
	_poly(root, 2, PlaceholderArtRegistry.color_from_token("cowl_dusk"), PackedVector2Array([
		Vector2(-8, -58), Vector2(14, -62), Vector2(18, -38), Vector2(-4, -32)
	]))
	_poly(root, 1, PlaceholderArtRegistry.color_from_token("skin_umber"), PackedVector2Array([
		Vector2(-22, -32), Vector2(24, -28), Vector2(28, 38), Vector2(-26, 42), Vector2(-30, 8)
	]))
	var lap := KyndeBladeArtPalette.LAPIS.lerp(Color.BLACK, 0.12)
	_poly(root, 0, lap, PackedVector2Array([
		Vector2(26, -8), Vector2(62, 4), Vector2(60, 10), Vector2(24, -2)
	]))


static func _build_false(root: Node2D) -> void:
	var eb: Color = KyndeBladeArtPalette.RUBRICATION.darkened(0.1)
	eb = eb.lerp(KyndeBladeArtPalette.JEWEL_CRIMSON, KyndeBladeArtPalette.ENEMY_BODY_JEWEL_MIX)
	_poly(root, 1, eb, PackedVector2Array([
		Vector2(-32, -92), Vector2(8, -98), Vector2(44, -72), Vector2(38, -20),
		Vector2(48, 28), Vector2(12, 84), Vector2(-36, 76), Vector2(-44, 20), Vector2(-40, -40)
	]))
	_poly(root, 2, KyndeBladeArtPalette.BORDER_RED.lerp(Color.BLACK, 0.18), PackedVector2Array([
		Vector2(-10, -30), Vector2(24, -34), Vector2(20, 6), Vector2(-14, 2)
	]))
	# False image language: forked tongue + stained coat (Hawkin echo) + seed marks.
	_poly(root, 3, KyndeBladeArtPalette.JEWEL_CRIMSON, PackedVector2Array([
		Vector2(6, -10), Vector2(16, -6), Vector2(10, 22), Vector2(4, 26),
		Vector2(8, 10), Vector2(2, 26), Vector2(-4, 22), Vector2(0, -8)
	]))
	_poly(root, 0, KyndeBladeArtPalette.PARCHMENT_AGED.lerp(KyndeBladeArtPalette.INK_LIGHT, 0.2), PackedVector2Array([
		Vector2(-38, 14), Vector2(34, 6), Vector2(38, 26), Vector2(-42, 34)
	]))
	_poly(root, 1, KyndeBladeArtPalette.RUBRICATION.lerp(KyndeBladeArtPalette.BORDER_RED, 0.25), PackedVector2Array([
		Vector2(-28, 14), Vector2(-16, 12), Vector2(-10, 18), Vector2(-22, 20),
		Vector2(-4, 14), Vector2(10, 12), Vector2(16, 18), Vector2(2, 20),
		Vector2(20, 10), Vector2(30, 8), Vector2(34, 14), Vector2(24, 16)
	]))


static func _build_green_knight(root: Node2D) -> void:
	var g := KyndeBladeArtPalette.BOSS_GREEN_KNIGHT
	_poly(root, 1, g.darkened(0.15), PackedVector2Array([
		Vector2(-48, -140), Vector2(52, -138), Vector2(44, 120), Vector2(-56, 118), Vector2(-60, -20)
	]))
	_poly(root, 2, KyndeBladeArtPalette.GOLD_DARK, PackedVector2Array([
		Vector2(40, -100), Vector2(120, -40), Vector2(112, -20), Vector2(36, -72)
	]))
	_poly(root, 0, g.lerp(Color.BLACK, 0.35), PackedVector2Array([
		Vector2(-20, -40), Vector2(20, -38), Vector2(18, 40), Vector2(-22, 42)
	]))


static func _build_mede(root: Node2D) -> void:
	var cr := KyndeBladeArtPalette.JEWEL_CRIMSON
	_poly(root, 1, cr.lerp(KyndeBladeArtPalette.RUBRICATION, 0.25), PackedVector2Array([
		Vector2(-70, 20), Vector2(-50, -100), Vector2(50, -105), Vector2(72, 18), Vector2(40, 110), Vector2(-42, 108)
	]))
	_poly(root, 2, KyndeBladeArtPalette.GOLD, PackedVector2Array([
		Vector2(-18, -108), Vector2(20, -112), Vector2(22, -78), Vector2(-20, -76)
	]))
	_poly(root, 0, cr.darkened(0.2), PackedVector2Array([
		Vector2(-8, -72), Vector2(10, -74), Vector2(6, -40), Vector2(-10, -38)
	]))


static func _build_wrath(root: Node2D) -> void:
	var c := KyndeBladeArtPalette.RUBRICATION.lerp(KyndeBladeArtPalette.SICKLY_HIGHLIGHT, 0.12)
	_poly(root, 1, c, PackedVector2Array([
		Vector2(0, -110), Vector2(38, -30), Vector2(110, 0), Vector2(42, 38), Vector2(0, 100),
		Vector2(-42, 36), Vector2(-108, 0), Vector2(-36, -32)
	]))
	_poly(root, 2, KyndeBladeArtPalette.BORDER_RED, PackedVector2Array([
		Vector2(-6, -24), Vector2(8, -28), Vector2(4, 8), Vector2(-8, 6)
	]))


static func _build_hunger(root: Node2D) -> void:
	var h := KyndeBladeArtPalette.BOSS_HUNGER
	_poly(root, 1, h.lerp(Color.BLACK, 0.25), PackedVector2Array([
		Vector2(-14, -130), Vector2(12, -132), Vector2(18, 40), Vector2(-20, 42), Vector2(-22, -40)
	]))
	_poly(root, 2, h.lerp(KyndeBladeArtPalette.JEWEL_VIOLET_SHADOW, 0.4), PackedVector2Array([
		Vector2(-22, -132), Vector2(24, -136), Vector2(18, -88), Vector2(-16, -86)
	]))


static func _build_piers(root: Node2D) -> void:
	var earth := KyndeBladeArtPalette.HI_BIT_TERRACOTTA.lerp(PlaceholderArtRegistry.color_from_token("skin_umber"), 0.5)
	_poly(root, 2, KyndeBladeArtPalette.HI_BIT_SILHOUETTE_MID, PackedVector2Array([
		Vector2(-10, -54), Vector2(12, -58), Vector2(14, -36), Vector2(-6, -32)
	]))
	_poly(root, 1, earth, PackedVector2Array([
		Vector2(-20, -30), Vector2(22, -26), Vector2(26, 36), Vector2(-24, 40)
	]))
	_poly(root, 0, KyndeBladeArtPalette.LAPIS.darkened(0.2), PackedVector2Array([
		Vector2(22, -6), Vector2(58, 6), Vector2(56, 12), Vector2(20, 0)
	]))


static func _build_orfeo_wanderer(root: Node2D) -> void:
	var mist := KyndeBladeArtPalette.JEWEL_VIOLET_SHADOW.lerp(KyndeBladeArtPalette.HI_BIT_SKY_TEAL, 0.35)
	_poly(root, 1, mist, PackedVector2Array([
		Vector2(-16, -44), Vector2(14, -48), Vector2(18, 32), Vector2(-20, 36)
	]))
	_poly(root, 2, KyndeBladeArtPalette.JEWEL_ULTRAMARINE.lerp(Color.BLACK, 0.2), PackedVector2Array([
		Vector2(-10, -52), Vector2(8, -56), Vector2(6, -28), Vector2(-8, -26)
	]))


static func _build_crowd(root: Node2D) -> void:
	var ink := KyndeBladeArtPalette.INK_PRIMARY
	ink.a = 0.55
	var seeds := [
		Vector2(-120, 20), Vector2(-70, 8), Vector2(-20, 24), Vector2(30, 6), Vector2(80, 22),
		Vector2(120, 12), Vector2(-40, 40), Vector2(50, 44)
	]
	for i in seeds.size():
		var s: float = 6.0 + float(i % 3) * 2.0
		var c: Vector2 = seeds[i]
		_poly(root, i % 3, ink, PackedVector2Array([
			c + Vector2(-s, 0), c + Vector2(0, -s * 0.8), c + Vector2(s, 0), c + Vector2(0, s * 0.6)
		]))


static func _build_grace_kneeler(root: Node2D) -> void:
	var p := KyndeBladeArtPalette.PARCHMENT_AGED.lerp(KyndeBladeArtPalette.INK_LIGHT, 0.35)
	_poly(root, 1, p, PackedVector2Array([
		Vector2(-80, 28), Vector2(88, 24), Vector2(92, 48), Vector2(-84, 52)
	]))
	_poly(root, 2, KyndeBladeArtPalette.VISTA_BODY, PackedVector2Array([
		Vector2(-14, -8), Vector2(14, -10), Vector2(12, 22), Vector2(-12, 24)
	]))
