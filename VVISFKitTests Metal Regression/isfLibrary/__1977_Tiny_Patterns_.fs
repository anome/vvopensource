/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
		{
			"NAME": "SCALE",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "ShiftX",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "ShiftY",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "TAN",
			"TYPE": "bool",
			"DEFAULT": 0.0
		},
		{
			"NAME": "MIX",
			"TYPE": "bool",
			"DEFAULT": 0.0
		}
	]
}*/

// Based on "Tiny Patterns" by akohdr: https://www.shadertoy.com/view/XsKSRy

void mainImage( out vec4 k,  vec2 p)
{

	p.x += RENDERSIZE.x*(.5-ShiftX*2.);
	p.y += RENDERSIZE.y*(.5-ShiftY*2.);
	
	k += sin( dot(p,p)/(SCALE*RENDERSIZE.x) + TIME) -k;
	
    if (TAN) { k += tan( dot(p,p)/(SCALE*RENDERSIZE.x) + TIME) -k; }
    if (MIX) { k+= mix( sin( dot(p,p)/(SCALE*RENDERSIZE.x) + TIME) -k, tan( dot(p,p)/(SCALE*1000.) + TIME) -k, 0.5 ); }
 
}


void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}