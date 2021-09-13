/*{
	"CREDIT": "by msfeldstein",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
		{
			"TYPE": "float",
			"NAME": "edge",
			"MIN": 0,
			"MAX": 1,
			"DEFAULT": 0.5
		}, {
			"TYPE": "float",
			"NAME": "zoom",
			"MIN": 0,
			"MAX": 1
		}, {
			"TYPE": "float",
			"NAME": "xOffset",
			"MIN": -2.5,
			"MAX": 2.5,
			"DEFAULT": 0
		}, {
			"TYPE": "float",
			"NAME": "yOffset",
			"MIN": -1,
			"MAX": 1,
			"DEFAULT": 0.07
		}
	]
}*/

const int max_iteration = 1000;

float log10(float t) {
	return log(t) / 2.71828;
}

vec3 hsv2rgb(vec3 c)
{
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void main() {
	// Mandlebrot range from x[-2.5:1], y[-1:1]
	float halfXSize = 3.5 * (1.0 - zoom) / 2.0;
	float halfYSize = 1.0 * (1.0 - zoom);
	float x0 = mix(-0.75 - halfXSize, -0.75 + halfXSize, vv_FragNormCoord.x) + xOffset;
	float y0 = mix(-halfYSize, halfYSize, vv_FragNormCoord.y) + yOffset;
	float x = 0.0;
	float y = 0.0;
	
	int iterations;
	for (int iteration = 0; iteration < max_iteration; iteration++) {
		float newX = x * x - y * y + x0;
		float newY = 2.0 * x * y + y0;
		x = newX;
		y = newY;
		if (x * x + y * y > 2.0 * 2.0) break;
		iterations ++;
	}
	float v = float(iterations) / (15.0 + 400.0 * edge);
	vec3 hsv = vec3(v, 1.0, 0.5);
	vec3 rgb = hsv2rgb(hsv);
	gl_FragColor = vec4( v, 0.0, 0.0, 1.0);
}