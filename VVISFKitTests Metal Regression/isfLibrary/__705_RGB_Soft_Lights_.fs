/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
	
	{
			"NAME": "Light1",
			"TYPE": "color",
			"DEFAULT": [
				1.0,
				0.0,
				0.0,
				1.0
			]
		},
		{
			"NAME": "Light1_Radius",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.5
		},
		{
			"NAME": "Light1_X",
			"TYPE": "float",
			"MIN": -1.0,
			"MAX": 1.0,
			"DEFAULT": 0.0
		},{
			"NAME": "Light1_Y",
			"TYPE": "float",
			"MIN": -1.0,
			"MAX": 1.0,
			"DEFAULT": 0.25
		},
		{
			"NAME": "Light2",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				1.0,
				0.0,
				1.0
			]
		},
		{
			"NAME": "Light2_Radius",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.5
		},
		{
			"NAME": "Light2_X",
			"TYPE": "float",
			"MIN": -1.0,
			"MAX": 1.0,
			"DEFAULT": -0.25
		},{
			"NAME": "Light2_Y",
			"TYPE": "float",
			"MIN": -1.0,
			"MAX": 1.0,
			"DEFAULT": -0.25
		},
		{
			"NAME": "Light3",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "Light3_Radius",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.5
		},
		{
			"NAME": "Light3_X",
			"TYPE": "float",
			"MIN": -1.0,
			"MAX": 1.0,
			"DEFAULT": 0.25
		},{
			"NAME": "Light3_Y",
			"TYPE": "float",
			"MIN": -1.0,
			"MAX": 1.0,
			"DEFAULT": -0.25
		},
		{
			"NAME": "CanvasColor",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				0.0,
				1.0
			]
		}
	
	]
}*/

// Ported/adapted from "RGB Soft Lights" by BitOfGold: https://www.shadertoy.com/view/llVGz1

vec3 iResolution = vec3(RENDERSIZE, 1.);

vec3 softLight(vec3 canvas, vec2 uv, vec2 center, float r, vec3 color) {
    float d = clamp(1.0-length(center-uv)/r,0.0,1.0);
    return(canvas + d*color);
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	vec2 uv = fragCoord.xy / RENDERSIZE.xy;
	float ASPECT = (RENDERSIZE.x/RENDERSIZE.y);
	uv.x *= ASPECT;
	
	vec3 canvas = vec3(CanvasColor.rgb);
    canvas = softLight(canvas, uv, vec2(Light1_X+.5*ASPECT, Light1_Y+.5), Light1_Radius*2., vec3(Light1.rgb));
    canvas = softLight(canvas, uv, vec2(Light2_X+.5*ASPECT, Light2_Y+.5), Light2_Radius*2., vec3(Light2.rgb));
    canvas = softLight(canvas, uv, vec2(Light3_X+.5*ASPECT, Light3_Y+.5), Light3_Radius*2., vec3(Light3.rgb));
	fragColor = vec4(canvas,1.0);
}


void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}