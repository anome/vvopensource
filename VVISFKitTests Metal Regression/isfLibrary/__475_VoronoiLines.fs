/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "Automatically Converted"
  ],
  "INPUTS": [],
  "DESCRIPTION": "Automatically converted from http://glslsandbox.com/e#15588.0"
}*/


// Shader by Nicolas Robert [NRX]
// Latest version: http://glsl.heroku.com/e#15513
// Concept from: http://www.iquilezles.org/www/articles/voronoilines/voronoilines.htm

#ifdef GL_ES
precision mediump float;
#endif


float iGlobalTime = TIME*.25;
vec3 iResolution = vec3 (RENDERSIZE, 0.0);

#define SIZE 		8
#define SQRT2		1.41421356237
#define HASH_MAGNITUDE	(2.0 * SQRT2 - 1.0) // Ok if: HASH_MAGNITUDE <= KERNEL * SQRT (2) - 1
#define KERNEL		2 // Ok if: KERNEL >= (HASH_MAGNITUDE + 1) / SQRT (2)
#define BORDER

float hash (in int index) {
	float x = float (index);
	return HASH_MAGNITUDE * 0.5 * sin (sin (x) * x + sin (x * x) * iGlobalTime);
}

vec2 pointInCell (in ivec2 cell) {
	int index = cell.x + cell.y * SIZE;
	return vec2 (cell) + vec2 (hash (index), hash (index + 1));
}

void main () {
	vec2 p = float (SIZE) * (gl_FragCoord.xy - 0.5 * iResolution.xy) / iResolution.y;
	ivec2 pCell = ivec2 (floor (p + 0.5));

	float dMin = 2.0 * HASH_MAGNITUDE;
	vec2 pqMin;
	ivec2 minCell;
	for (int y = -KERNEL; y <= KERNEL; ++y) {
		for (int x = -KERNEL; x <= KERNEL; ++x) {
			ivec2 qCell = pCell + ivec2 (x, y);
			vec2 pq = pointInCell (qCell) - p;
			float d = dot (pq, pq);
			if (d < dMin) {
				dMin = d;
				pqMin = pq;
				minCell = qCell;
			}
		}
	}
	dMin = sqrt (dMin)+5000.0;
	int col = minCell.x + minCell.y * SIZE;
	vec4 color = 0.6 + vec4 (hash (col), hash (col + 1), hash (col + 2), 0.0) * 0.8 / HASH_MAGNITUDE;

	#ifdef BORDER
	for (int y = -KERNEL; y <= KERNEL; ++y) {
		for (int x = -KERNEL; x <= KERNEL; ++x) {
			ivec2 qCell = pCell + ivec2 (x, y);
			if (qCell != minCell) {
				vec2 pq = pointInCell (qCell) - p;
				dMin = min (dMin, dot (0.5 * (pqMin + pq), normalize (pq - pqMin)));
			}
		}
	}
	#endif

	gl_FragColor = color * min(dMin* 8.0, 1.0) * ( .5 + .5*sin (TIME*-10.+dMin * .5*iResolution.y / float (SIZE)));
}