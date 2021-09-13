/*{
	"DESCRIPTION": "",
	"CREDIT": "by mojovideotech",
	"ISFVSN": "2",
	"CATEGORIES": [
		"fractal"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "color",
			"TYPE": "color",
			"DEFAULT": [
				0.5,
				0.5,
				0.5,
				1.0
			]
		},
		{
			"NAME": "rate",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.001,
			"MAX": 3.0
		},
		{
			"NAME": "shift",
			"TYPE": "float",
			"DEFAULT": 0.33,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
      		"NAME": "depth",
      		"TYPE": "float",
      		"DEFAULT": 80,
      		"MIN": 36,
      		"MAX": 120
    	},
		{
			"NAME": "shade",
			"TYPE": "point2D",
			"DEFAULT":	[ 0, 0 ],
			"MAX" : 	[ 1.0, 1.0 ],
      		"MIN" : 	[ -1.0, -1.0 ]
		},
		{
			"NAME": "mixer",
			"TYPE": "float",
			"DEFAULT": 0.75,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
	}*/
	
	
///////////////////////////////////////////
// FractalShaderFilter  by mojovideotech
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
///////////////////////////////////////////

#define 	phi   	1.618033988749895 	// golden ratio

void main()	{
	vec4 g = IMG_THIS_PIXEL(inputImage);
	vec4 p = g;
	float b = -shade.x, c = 8.0 * shade.y - 4.0;
	vec4 h  = g + vec4(b, b, b, 0.0);
	g.rgb = ((vec3(2.0) * (h.rgb - vec3(0.5))) * vec3(c) / vec3(2.0)) + vec3(0.5);
	g.a = ((2.0 * (h.a - 0.5)) * abs(c) / 2.0) + 0.5;
    vec2 uv = 0.99 * gl_FragCoord.xy / RENDERSIZE.y;
    float t = 0.01*(TIME*rate), k = cos(t), l = sin(t);        
     vec2 vv = uv/-g.xy;
     uv += mix(g.zw,vv,shift);
    float s = atan(phi-dot(k,l));
    for(int i=0; i<120; ++i) {
    	float r = float (i)-floor(depth);
        uv  = abs(uv) - s;    
        uv *= mat2(k,-l,-l,-k); 
        s  *= 0.987;
        if (r == 0.0) break;
    }
    float x = .5 + .5*tan(6.28318*cos(69.*length(uv)));
	vec4 col = color+mix(color,g,1.0-shift);
	col *= vec4(vec3(length(uv)*2.34, 0.456*x, 0.567/x),0.8);
	gl_FragColor = mix(p,clamp(col,0.0,1.0),mixer);
}
	
