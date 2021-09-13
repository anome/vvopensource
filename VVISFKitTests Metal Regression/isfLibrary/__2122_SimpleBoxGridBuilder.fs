/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
  {
            "NAME": "columns",
            "TYPE": "float",
           "DEFAULT": 16,
            "MIN": 2,
            "MAX": 64
        },
         {
            "NAME": "rows",
            "TYPE": "float",
           "DEFAULT": 9,
            "MIN": 2,
            "MAX": 64
        },
          {
			"NAME": "color1",
			"TYPE": "color",
			"DEFAULT": [
				0.8,
				0.0,
				0.0,
				1.0
			]
		},
		 {
			"NAME": "color2",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				0.8,
				1.0
			]
		}
	]
}*/

// SimpleBoxGridBuilder by mojovideotech
// simple checkerboard pattern with whole number ratios 
// useful for multiple output configuration / image alignment / geometry correction / mapping


void main(void) {
	vec2 p = gl_FragCoord.xy / RENDERSIZE.xy;
	vec2 q = vec2(floor(columns),floor(rows));

	vec3 color = vec3(color1);
	if (mod(floor(p.x * q.x), 2.0 ) + mod(floor(p.y * q.y), 2.0) == 1.0) {
		color = vec3(color2);
	}
	
	gl_FragColor = vec4(color, 1.0);
}