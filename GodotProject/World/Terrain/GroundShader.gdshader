shader_type spatial;

uniform sampler2D heightmap;
uniform float maxHeight;

varying float HEIGHT;

void fragment() {
	ALBEDO = vec3(HEIGHT) / maxHeight;
}

void vertex() {
	HEIGHT = texture(heightmap, (VERTEX.xz / 512.0f) + vec2(0.5f, 0.5f)).x * maxHeight;
	VERTEX.y = VERTEX.y + HEIGHT;
}
