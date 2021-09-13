/*{
	"CREDIT": "by mojovideotech",
   	"CATEGORIES" : [
    "blending",
    "photoshop"
  ],
  "DESCRIPTION" : "GLSL port of Photoshop layer blending modes",
  "INPUTS" : [
   	{
      	"NAME": 	"mode",
      	"TYPE": 	"long",
      	"VALUES": [	0,
    	    		1,
    	    		2,
    	    		3,
     	   			4,
     	   			5,
        			6,
        			7,
        			8,
        			9,
        			10,
        			11,
					12,
					13,
     	   			14,
     	   			15,
        			16,
        			17,
        			18,
        			19,
        			20,
        			21,
					22,
					23,
        			24,
        			25
      		      ],
      	"LABELS": [	"crossfade",
      				"darken",
					"multiply",
					"colorBurn",
					"linearBurn",
					"darkerColor",
					"lighten",
					"screen",
					"colorDodge",
					"linearDodge",
					"lighterColor",
					"overlay",
					"softLight",
					"hardLight",
					"vividLight",
					"linearLight",
					"pinLight",
					"hardMix",
					"difference",
					"exclusion",
					"subtract",
					"divide",
					"hue",
					"color",
					"saturation",
					"luminosity"
      		      ],
		"DEFAULT": 	0
	},
	{
      "NAME" : "inputImageA",
      "TYPE" : "image"
    },
    {
      "NAME" : "inputImageB",
      "TYPE" : "image"
    },
    {
		"NAME" : 		"crossfader",
		"TYPE" : 		"float",
		"DEFAULT" : 	1.0,
		"MIN" : 		0.0,
		"MAX" : 		2.0
	},
   	{
   		"NAME" : 		"swapAB",
     	"TYPE" : 		"bool",
     	"DEFAULT" : 	false
   	}
  ]
}
*/


////////////////////////////////////////////////////////////
// PSBlendModeExplorer  by mojovideotech
//
// based on :
// shadertoy.com/XdS3RW
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


vec3 darken( vec3 s, vec3 d ) { return min(s,d); }

vec3 multiply( vec3 s, vec3 d ) { return s*d; }

vec3 colorBurn( vec3 s, vec3 d ) { return 1.0 - (1.0 - d) / s; }

vec3 linearBurn( vec3 s, vec3 d ) { return s + d - 1.0; }

vec3 darkerColor( vec3 s, vec3 d ) { return (s.x + s.y + s.z < d.x + d.y + d.z) ? s : d; }

vec3 lighten( vec3 s, vec3 d ) { return max(s,d); }

vec3 screen( vec3 s, vec3 d ) { return s + d - s * d; }

vec3 colorDodge( vec3 s, vec3 d ) { return d / (1.0 - s); }

vec3 linearDodge( vec3 s, vec3 d ) { return s + d; }

vec3 lighterColor( vec3 s, vec3 d ) { return (s.x + s.y + s.z > d.x + d.y + d.z) ? s : d; }

float overlay( float s, float d ) { return (d < 0.5) ? 2.0 * s * d : 1.0 - 2.0 * (1.0 - s) * (1.0 - d); }
vec3 overlay( vec3 s, vec3 d ) { return vec3 (overlay(s.x,d.x), overlay(s.y,d.y), overlay(s.z,d.z)); }

float softLight( float s, float d ) {
	return (s < 0.5) ? d - (1.0 - 2.0 * s) * d * (1.0 - d) 
		: (d < 0.25) ? d + (2.0 * s - 1.0) * d * ((16.0 * d - 12.0) * d + 3.0) 
					 : d + (2.0 * s - 1.0) * (sqrt(d) - d);
}
vec3 softLight( vec3 s, vec3 d ) { return vec3 (softLight(s.x,d.x), softLight(s.y,d.y), softLight(s.z,d.z)); }

float hardLight( float s, float d ) { return (s < 0.5) ? 2.0 * s * d : 1.0 - 2.0 * (1.0 - s) * (1.0 - d); }
vec3 hardLight( vec3 s, vec3 d ) { return vec3 (hardLight(s.x,d.x), hardLight(s.y,d.y), hardLight(s.z,d.z)); }

float vividLight( float s, float d ) { return (s < 0.5) ? 1.0 - (1.0 - d) / (2.0 * s) : d / (2.0 * (1.0 - s)); }
vec3 vividLight( vec3 s, vec3 d ) { return vec3 (vividLight(s.x,d.x), vividLight(s.y,d.y), vividLight(s.z,d.z)); }

vec3 linearLight( vec3 s, vec3 d ) { return 2.0 * s + d - 1.0; }

float pinLight( float s, float d ) { return (2.0 * s - 1.0 > d) ? 2.0 * s - 1.0 : (s < 0.5 * d) ? 2.0 * s : d; }
vec3 pinLight( vec3 s, vec3 d ) { return vec3 (pinLight(s.x,d.x), pinLight(s.y,d.y), pinLight(s.z,d.z)); }

vec3 hardMix( vec3 s, vec3 d ) { return floor(s + d); }

vec3 difference( vec3 s, vec3 d ) { return abs(d - s); }

vec3 exclusion( vec3 s, vec3 d ) { return s + d - 2.0 * s * d; }

vec3 subtract( vec3 s, vec3 d ) { return s - d; }

vec3 divide( vec3 s, vec3 d ) { return s / d; }

vec3 rgb2hsv(vec3 c) {
	vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
	vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}
vec3 hsv2rgb(vec3 c) {
	vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}
vec3 hue( vec3 s, vec3 d ) {
	d = rgb2hsv(d);
	d.x = rgb2hsv(s).x;
	return hsv2rgb(d);
}
vec3 color( vec3 s, vec3 d ) {
	s = rgb2hsv(s);
	s.z = rgb2hsv(d).z;
	return hsv2rgb(s);
}
vec3 saturation( vec3 s, vec3 d ) {
	d = rgb2hsv(d);
	d.y = rgb2hsv(s).y;
	return hsv2rgb(d);
}

vec3 luminosity( vec3 s, vec3 d ) {
	float dLum = dot(d, vec3(0.3, 0.59, 0.11));
	float sLum = dot(s, vec3(0.3, 0.59, 0.11));
	float lum = sLum - dLum;
	vec3 c = d + lum;
	float minC = min(min(c.x, c.y), c.z);
	float maxC = max(max(c.x, c.y), c.z);
	if(minC < 0.0) return sLum + ((c - sLum) * sLum) / (sLum - minC);
	else if(maxC > 1.0) return sLum + ((c - sLum) * (1.0 - sLum)) / (maxC - sLum);
	else return c;
}

vec3 blend( vec3 s, vec3 d, int id ) {
	if(id==1)	return darken(s,d);
	if(id==2)	return multiply(s,d);
	if(id==3)	return colorBurn(s,d);
	if(id==4)	return linearBurn(s,d);
	if(id==5)	return darkerColor(s,d);
	if(id==6)	return lighten(s,d);
	if(id==7)	return screen(s,d);
	if(id==8)	return colorDodge(s,d);
	if(id==9)	return linearDodge(s,d);
	if(id==10)	return lighterColor(s,d);
	if(id==11)	return overlay(s,d);
	if(id==12)	return softLight(s,d);
	if(id==13)	return hardLight(s,d);
	if(id==14)	return vividLight(s,d);
	if(id==15)	return linearLight(s,d);
	if(id==16)	return pinLight(s,d);
	if(id==17)	return hardMix(s,d);
	if(id==18)	return difference(s,d);
	if(id==19)	return exclusion(s,d);
	if(id==20)	return subtract(s,d);
	if(id==21)	return divide(s,d);
	if(id==22)	return hue(s,d);
	if(id==23)	return color(s,d);
	if(id==24)	return saturation(s,d);
	if(id==25)	return luminosity(s,d);
    return vec3(0.0);
}

void main() 
{
	int id = int(mode);
	vec4 aa = IMG_PIXEL(inputImageA, gl_FragCoord.xy);
	vec4 bb = IMG_PIXEL(inputImageB, gl_FragCoord.xy);
	vec4 a, b;
	if(swapAB) { a = bb; b = aa; }
	else { a = aa; b = bb; }
	vec3 col;
	if(id==0) { col = mix(a,b,crossfader*0.5).rgb; }
	else { col = clamp(blend(a.xyz*clamp(2.0-crossfader,0.0,1.0),b.xyz*clamp(crossfader,0.0,1.0),id),0.0,1.0); }
	
	gl_FragColor = vec4(col,1.0);
}
