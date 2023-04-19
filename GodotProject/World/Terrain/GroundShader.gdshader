shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_back,diffuse_burley,specular_schlick_ggx;

const int HASHSIZE = 32;
const float renderDist = 4096.0f;
uniform vec3 points[1024];
uniform sampler2D riverGradient;
uniform float maxHeight;

uniform vec3 position;

uniform vec4 grassColor: source_color;
uniform vec4 sandColor : source_color;
uniform vec4 rockColor : source_color;

varying vec3 uv1_triplanar_pos;
uniform float uv1_blend_sharpness;
varying vec3 uv1_power_normal;
uniform vec3 uv1_scale;
uniform vec3 uv1_offset;


varying float HEIGHT;

varying vec2 texPos;

varying vec3 NRML;

//
// Description : Array and textureless GLSL 2D simplex noise function.
//      Author : Ian McEwan, Ashima Arts.
//  Maintainer : stegu
//     Lastmod : 20110822 (ijm)
//     License : Copyright (C) 2011 Ashima Arts. All rights reserved.
//               Distributed under the MIT License. See LICENSE file.
//               https://github.com/ashima/webgl-noise
//               https://github.com/stegu/webgl-noise
// 

vec3 mod289(vec3 x) {
  return x - floor(x * (1.0 / 289.0)) * 289.0;
}

vec2 mod2892(vec2 x) {
  return x - floor(x * (1.0 / 289.0)) * 289.0;
}

vec3 permute(vec3 x) {
  return mod289(((x*34.0)+10.0)*x);
}

float snoise(vec2 v)
  {
  const vec4 C = vec4(0.211324865405187,  // (3.0-sqrt(3.0))/6.0
                      0.366025403784439,  // 0.5*(sqrt(3.0)-1.0)
                     -0.577350269189626,  // -1.0 + 2.0 * C.x
                      0.024390243902439); // 1.0 / 41.0
// First corner
  vec2 i  = floor(v + dot(v, C.yy) );
  vec2 x0 = v -   i + dot(i, C.xx);

// Other corners
  vec2 i1;
  //i1.x = step( x0.y, x0.x ); // x0.x > x0.y ? 1.0 : 0.0
  //i1.y = 1.0 - i1.x;
  i1 = (x0.x > x0.y) ? vec2(1.0, 0.0) : vec2(0.0, 1.0);
  // x0 = x0 - 0.0 + 0.0 * C.xx ;
  // x1 = x0 - i1 + 1.0 * C.xx ;
  // x2 = x0 - 1.0 + 2.0 * C.xx ;
  vec4 x12 = x0.xyxy + C.xxzz;
  x12.xy -= i1;

// Permutations
  i = mod2892(i); // Avoid truncation effects in permutation
  vec3 p = permute( permute( i.y + vec3(0.0, i1.y, 1.0 ))
		+ i.x + vec3(0.0, i1.x, 1.0 ));

  vec3 m = max(0.5 - vec3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
  m = m*m ;
  m = m*m ;

// Gradients: 41 points uniformly over a line, mapped onto a diamond.
// The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)

  vec3 x = 2.0 * fract(p * C.www) - 1.0;
  vec3 h = abs(x) - 0.5;
  vec3 ox = floor(x + 0.5);
  vec3 a0 = x - ox;

// Normalise gradients implicitly by scaling m
// Approximation of: m *= inversesqrt( a0*a0 + h*h );
  m *= 1.79284291400159 - 0.85373472095314 * ( a0*a0 + h*h );

// Compute final noise value at P
  vec3 g;
  g.x  = a0.x  * x0.x  + h.x  * x0.y;
  g.yz = a0.yz * x12.xz + h.yz * x12.yw;
  return ((130.0 * dot(m, g)) + 1.0f) / 2.0f;
}



int cursedHash(vec2 point){
	int hashDivide = int(renderDist * 2.0f) / HASHSIZE;
	int x = int(floor((point.x + renderDist) / float(hashDivide)));
	int y = int(floor((point.y + renderDist) / float(hashDivide)));
	return x + y * HASHSIZE;
}

float getHeight(vec3 point) {
	float height = 1.0f;
	for (int i = cursedHash(point.xz) - HASHSIZE * 2; (i < points.length()) && (i < cursedHash(point.xz) + HASHSIZE * 2); i++) {
		float newHeight = 1.0f - texture(riverGradient, vec2(clamp(distance(points[i].xz, point.xz) / (points[i].y * 1.5f), 0.01f, 1.0f), 0.5f)).r;
		if (newHeight < height) {height = newHeight;}
	}
	vec3 pos = -position + point;
	height -= snoise(pos.xz / 8196.0f * 3.0f) * 0.3 + snoise(pos.xz / 8196.0f * 12.0f) * 0.1;
	//height = clamp(height, 0.0f, 1.0f);
	return height;
}

float fmod(float i, float m){
	if(abs(i) < m){
		return i;
	}else if(i < 0.0f){
		return i + (floor(abs(i) / m)) * m;
	}else{
		return i - (floor(abs(i) / m)) * m;
	}
}

float noiseHash(float val) {
	return fmod(((val * 2531.0f) / 1289.0f), 4.0f);
}

vec2 addNoise(vec3 point) {
	return (vec3(noiseHash(point.x), noiseHash(point.y), noiseHash(point.z))).xz;
}

void vertex() {
	
	HEIGHT = getHeight(VERTEX);
	texPos = VERTEX.xz + position.xz;
	//VERTEX.xz = VERTEX.xz + addNoise(VERTEX - position);
	NORMAL = normalize( vec3(
		(HEIGHT - getHeight(VERTEX + vec3(1.0f,0,0))) - (HEIGHT - getHeight(VERTEX - vec3(1.0f,0,0))) * 10.0f,
		0.05f,
		HEIGHT - getHeight(VERTEX + vec3(0,0,1.0f)) - (HEIGHT - getHeight(VERTEX - vec3(0,0,1.0f))) * 10.0f
		) );
	NRML = NORMAL;
	VERTEX.y = VERTEX.y + HEIGHT * maxHeight;
	
	uv1_power_normal=pow(abs(NORMAL),vec3(uv1_blend_sharpness));
	uv1_triplanar_pos = (VERTEX - position) * uv1_scale + uv1_offset;
	uv1_power_normal/=dot(uv1_power_normal,vec3(1.0));
	uv1_triplanar_pos *= vec3(1.0,-1.0, 1.0);
}


vec4 triplanar_texture(sampler2D p_sampler,vec3 p_weights,vec3 p_triplanar_pos) {
	vec4 samp=vec4(0.0);
	samp+= texture(p_sampler,p_triplanar_pos.xy) * p_weights.z;
	samp+= texture(p_sampler,p_triplanar_pos.xz) * p_weights.y;
	samp+= texture(p_sampler,p_triplanar_pos.zy * vec2(-1.0,1.0)) * p_weights.x;

	return samp;
}

float clampColor(float i, float clampVal) {
	if (i > clampVal) {
		return 0.5f;
	}else{
		return 0.1f;
	}
}

vec3 groundAlbedo(){
	vec3 color = mix(sandColor.rgb, mix(grassColor.rgb, rockColor.rgb, clamp(length(NRML.xz) * 10.0f, 0.0f, 1.0f)), clamp(ceil(HEIGHT), 0.0f, 1.0f));
	vec3 tri_pos = vec3(round(uv1_triplanar_pos.x * 5.0f), round(uv1_triplanar_pos.y * 5.0f), round(uv1_triplanar_pos.z * 5.0f));
	return (color)
	 * (vec3( clampColor( (snoise(tri_pos.xz * 1.1f) + snoise(tri_pos.xy * 1.7) + snoise(tri_pos.zy * 2.3f) ) / 3.0f, 0.4f) ) 
	 +  vec3( clampColor( (snoise(tri_pos.xz * 0.3f) + snoise(tri_pos.xy * 0.5) + snoise(tri_pos.zy * 0.7) ) / 3.0f, 0.4f) ) );
}

void fragment() {
	ALBEDO = groundAlbedo();
	METALLIC = 0.12;
	ROUGHNESS = 0.98;
	//ALPHA = ceil(HEIGHT);
}





