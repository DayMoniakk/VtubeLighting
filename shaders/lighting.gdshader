shader_type canvas_item;

uniform sampler2D light_tex : hint_default_black;

uniform bool use_alternative_light_blending = false;
uniform float light_strength : hint_range(0.0, 2.0) = 1.0;

void fragment() {
    vec2 uv = UV;

    // Sample textures
    vec4 sprite = texture(TEXTURE, uv);
    vec3 light_col = texture(light_tex, uv).rgb;

    // Multiply lighting
	vec3 lit;
	
	if (use_alternative_light_blending) {
		lit = sprite.rgb * (1.0 + (light_col - 0.5) * light_strength * 2.0);
	}
	else {
		lit = sprite.rgb * mix(vec3(1.0), light_col, light_strength);
	}

    // Preserve sprite alpha
    COLOR = vec4(lit, sprite.a);
}