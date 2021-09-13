/*{
  "DESCRIPTION": "",
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "generator",
    "equirectangular"
  ],
  "INPUTS": [
		{
    	  	"NAME": 	"rX",
    	  	"TYPE": 	"float",
    	  	"MIN": 		-3.1459,
    	  	"MAX": 		3.1459,
    	  	"DEFAULT": 	0.0
    	},
    	{
    		"NAME": 	"rY",
      		"TYPE": 	"float",
      		"MIN": 		-3.1459,
      		"MAX": 		3.1459,
    		 "DEFAULT": 0.0
    	},
    	{
      		"NAME": 	"rZ",
      		"TYPE": 	"float",
      		"MIN": 		-3.1459,
      		"MAX": 		3.1459,
      		"DEFAULT": 	0.0
    	},
    	{
      		"NAME": 	"zoom",
      		"TYPE": 	"float",
      		"MIN": 		1.0,
      		"MAX": 		6.0,
      		"DEFAULT": 	3.0
    	},
     	{
    		"NAME":   	"center",
      		"TYPE":   	"point2D",
      		"MAX":    	[ 1.0, 1.0 ],
      		"MIN":    	[ -1.0, -1.0 ],
      		"DEFAULT":	[ 0.0, 0.0 ]
    	},
		{
			"NAME": 	"mX",
			"TYPE": 	"float",
			"DEFAULT": 	0.67,
			"MIN": 		0.0,
			"MAX": 	2.0
		},
		{
			"NAME": 	"mY",
			"TYPE": 	"float",
			"DEFAULT": 	1.0,
			"MIN": 		0.0,
			"MAX": 		2.0
		},
		{
			"NAME": 	"rate",
			"TYPE": "float",
			"DEFAULT": 1.5,
			"MIN": 0.0,
			"MAX": 3.0
		},
		{
			"NAME": "e",
			"TYPE": "float",
			"DEFAULT": 0.025,
			"MIN": 0.0005,
			"MAX": 0.1
		},
		{
			"NAME": "depth",
			"TYPE": "float",
			"DEFAULT": 12.0,
			"MIN": 9.0,
			"MAX": 24.0
		},
		{
			"NAME": "saturation",
			"TYPE": "float",
			"DEFAULT": 9.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "glow",
			"TYPE": "float",
			"DEFAULT": 8.0,
			"MIN": 2.0,
			"MAX": 12.0
		},
		{
   		"NAME" : 		"flip",
     	"TYPE" : 		"bool",
     	"DEFAULT" : 	 false
   	},	
	{
   		"NAME" : 		"flop",
     	"TYPE" : 		"bool",
     	"DEFAULT" : 	false
	}
	]
}*/

////////////////////////////////////////////////////////////
// Equirec_z33d+  by mojovideotech
//
// spherical mod of :
// interactiveshaderformat.com/\2129
// based on :
// shadertoy.com/\XlXcWj
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

#define 	twpi  	6.283185307179586  	// two pi, 2*pi
#define 	pi   	3.141592653589793 	// pi

void main()
{  
	float k = 0.0, T = TIME*rate*0.1;
	vec2 R = RENDERSIZE.xy;
  	vec2 M = vec2(mX,mY)*R.xy;
  	vec2 sph = (gl_FragCoord.xy / RENDERSIZE.xy) * vec2(twpi, pi);
    vec3 rd = vec3(sin(sph.y) * sin(sph.x), cos(sph.y), sin(sph.y) * cos(sph.x)); 
    vec3 uv;
    if (flip) {
  			uv = rd.zyx;
  			if (flop) uv = rd.zxy;
  		} 
  		else if (flop) uv = rd.yzx;
  		else uv = rd.xyz;
  	for (float i = 0.0; i < 24.0; i++) {
    	if (i > depth) { break; }
    	vec3 p = vec3(uv.xy + center.xy, k-1.0);
    	float a = T;
    	p.zy *= mat2(cos(rY+a), -sin(rY+a), sin(rY+a), cos(rY+a));
	 	a /= 2.0;
    	p.yx *= mat2(cos(rX+a), -sin(rX+a), sin(rX+a), cos(rX+a));
    	a /= 2.0;
    	p.zx *= mat2(cos(rZ+a), -sin(rZ+a), sin(rZ+a), cos(rZ+a));
    	vec3 z = p;
    	float c = 2.0;
    	for (float j = 0.0; j < 9.0; j++) {
      		float r = length(z);
        	if (r > 6.0) { 
        		k += log(r) * r / c / pi;
				break;
        	}
      		float d = acos(z.z / r) * (zoom + (zoom+6.0) * M.x / R.x);
      		float b = atan(z.y, z.x) * (zoom + (zoom+6.0)  * M.y / R.y);
      		c = pow(r, 7.0) * 5.0 * c / r + 1.0;
      		z = pow(r, 7.0) * vec3(sin(d) * cos(b), -sin(d) * sin(b), -cos(d)) + p;
    	}
    	gl_FragColor = vec4(1.0 - i / (3.0*glow) - k + p / (12.0-saturation), 1.0);
      	if (log(length(z)) * length(z) / c < e) {
       		break;
    	}
  	}
}
