shader_type spatial;

uniform vec3 points[250];
uniform sampler2D riverGradient;

uniform float maxHeight;

varying float HEIGHT;

void fragment() {
	ALBEDO = vec3(HEIGHT);
}

void vertex() {
	HEIGHT = 1.0f;
	for (int i = 0; i < points.length(); i++) {
		float newHeight = 1.0f - texture(riverGradient, vec2(clamp(distance(points[i].xz, VERTEX.xz) / points[i].y, 0.01f, 1.0f), 0.5f)).r;
		if (newHeight < HEIGHT) {HEIGHT = newHeight;}
	}
	HEIGHT = clamp(HEIGHT, 0.0f, 1.0f);

	//HEIGHT = texture(heightmap, (VERTEX.xz / 512.0f) + vec2(0.5f, 0.5f)).x * maxHeight;
	VERTEX.y = VERTEX.y + HEIGHT * maxHeight;
}
