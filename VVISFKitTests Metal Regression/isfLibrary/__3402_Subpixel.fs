/*{
	"CREDIT": "by joris_dejong",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "SamplePoint",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				0.5
			]
		},
		{"NAME" : "Offset",
		"TYPE" : "float",
		"DEFAULT": "0.5"
		},
		{"NAME" : "Size",
		"TYPE" : "float",
		"DEFAULT": "0.05"
		},
		{"NAME" : "RandomX",
		"TYPE" : "float",
		"DEFAULT": "0.05"
		},
		{"NAME" : "RandomY",
		"TYPE" : "float",
		"DEFAULT": "0.05"
		}
	]
}*/

float random (vec2 st) {
    return fract(sin(dot(st.xy,
                         vec2(12.9898,78.233)))*
        43758.5453123);
}

void main() {
	vec2 st = isf_FragNormCoord.xy;

    float offsetAdjust = Offset * Size;
	//magic mumbo jumbo to get the X offset
	vec2 xrb = vec2(st.x,1.0);
	vec2 xrs = vec2(st.x, st.x) * 230.0 + sin(TIME/ 170.0) * 51.0;
	float xr = random(floor(xrb)) + random(floor(xrs * RandomX));
	float w = 0.34 * xr * Size;
	float x = mod ( st.x, w ) + st.x * offsetAdjust;
	
	//magic mumbo jumbo to get the Y offset
	vec2 yrb = vec2(st.y,1.0);
	vec2 yrs = vec2(st.y, st.y) * 170.0 + sin(TIME/ 230.0) * 34.0;
	float yr = random (floor (yrb) ) + random( floor(yrs * RandomY));
	float h = 0.34 * yr * Size;
	float y = mod ( st.y, h ) + st.y * offsetAdjust;
	
	vec2 coord = mod( vec2(x,y) + SamplePoint, 1.0);
    gl_FragColor = IMG_NORM_PIXEL(inputImage, coord);


}