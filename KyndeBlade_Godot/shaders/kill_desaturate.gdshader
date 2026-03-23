shader_type canvas_item;

/// Full-screen pass: mix toward luminance for kill-moment read (pairs with `Engine.time_scale` dip).
uniform sampler2D screen_texture : hint_screen_texture, filter_linear_mipmap;
uniform float desat_strength : hint_range(0.0, 1.0) = 0.0;

void fragment() {
	vec4 c = textureLod(screen_texture, SCREEN_UV, 0.0);
	float g = dot(c.rgb, vec3(0.299, 0.587, 0.114));
	vec3 out_rgb = mix(c.rgb, vec3(g), desat_strength);
	COLOR = vec4(out_rgb, 1.0);
}
