/*
	{
	"DESCRIPTION": "Petal Expansion",
	"CATEGORIES": 
		[
		"generator"
		],
	"ISFVSN": "2",
	"CREDIT": "Imported and Modified by: Old Salt",
	"VSN": "1.0",
  "INPUTS":
		[
			{
			"NAME": "uC1",
			"TYPE": "color",
			"DEFAULT":[0.0,1.0,0.0,1.0]
			},
			{
			"NAME": "uC2",
			"TYPE": "color",
			"DEFAULT":[0.0,0.0,0.0,1.0]
			},
			{
			"LABEL": "Offset: ",
			"NAME": "uOffset",
			"TYPE": "point2D",
			"MAX": [1.0,1.0],
			"MIN": [-1.0,-1.0],
			"DEFAULT": [0.0,0.0]
			},
			{
			"LABEL": "Zoom: ",
			"NAME": "uZoom",
			"TYPE": "float",
			"MAX": 10.0,
			"MIN": 0.0,
			"DEFAULT": 1.0
			},
			{
			"LABEL": "Rotation(or R Speed):",
			"NAME": "uRotate",
			"TYPE": "float",
			"MAX": 180.0,
			"MIN": -180.0,
			"DEFAULT": 0.0
			},
			{
			"LABEL": "Continuous Rotation? ",
			"NAME": "uContRot",
			"TYPE": "bool",
			"DEFAULT": 1
			},
			{
			"LABEL": "No. of Petals: ",
			"NAME": "uPetals",
			"TYPE": "float",
			"MAX": 20.0,
			"MIN": 1.0,
			"DEFAULT": 6.0
			},
			{
			"LABEL": "No. of Layers: ",
			"NAME": "uLayers",
			"TYPE": "float",
			"MAX": 10.0,
			"MIN": 1.0,
			"DEFAULT": 3.0
			}
		]
	}
*/
// based on: http://glslsandbox.com/e#72285

#define PI 3.141592653589
#define PI2 PI*2.0
#define PI2_inv 1.0/PI2

#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))


vec2 polar(vec2 p) {
	return vec2(length(p),-atan(p.y,-p.x)+PI);
}

float world(vec2 pp, float i) {
	return abs(cos((pp.r*pow(2.0,i))*PI2*4.0)+cos((pp.g*pow(2.0,i))*floor(uPetals+0.49)))/2.0;
}

void main()
	{
#ifdef XL_SHADER  // This shader must be copied to another
  discard;        // ISF account, and these three lines removed
#endif            // to allow it to function in xLights.
	vec2 uv = gl_FragCoord.xy/RENDERSIZE - 0.5; // normalize coordinates
	uv.x *= RENDERSIZE.x/RENDERSIZE.y;          // correct aspect ratio
	uv = (uv-uOffset) * 1.0/uZoom;              // offset and zoom functions
	uv = uContRot ? uv*rotate2D(TIME*uRotate/36.0) : uv*rotate2D(uRotate*PI/180.0); // rotation

	vec2 pp = polar(uv);
	float c = 0.0;
		
	c = world(pp,0.0);
	float Layers = -1.0;
	for (int i=0; i<10; i++)
		{
		Layers+=1.0;
		if (abs(c) < sin(TIME)*0.5+0.5) c = world(pp,Layers);
		if (Layers > floor(uLayers)) break;
		}
	vec3 col = uC1.rgb*c + uC2.rgb * (1.0-c);
	gl_FragColor = vec4(col, 1.0);
	}