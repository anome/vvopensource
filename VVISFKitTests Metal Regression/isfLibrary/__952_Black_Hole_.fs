/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
		{
			"NAME": "iChannel0",
			"TYPE": "image"
		},
      		{
			"LABEL": "Mouse X",
			"NAME": "mX",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": -0.5,
			"MAX": 1.5
		},
		{
			"LABEL": "Mouse Y",
			"NAME": "mY",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": -0.5,
			"MAX": 1.5
		},
		{
			"LABEL": "WARP",
			"NAME": "WARP",
			"TYPE": "float",
			"DEFAULT": 0.2,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "WARPFACTOR",
			"NAME": "WARPFACTOR",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "HOLE",
			"NAME": "HOLE",
			"TYPE": "float",
			"DEFAULT": 0.2,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
}*/

// Ported from "Black Hole Shader" by robobo1221: https://www.shadertoy.com/view/Xt3GWj

vec3 iResolution = vec3(RENDERSIZE, 1.);
vec2 iMouse = vec2(mX*RENDERSIZE.x, mY*RENDERSIZE.y);

vec3 getTexure(vec2 coord){
	return IMG_NORM_PIXEL(iChannel0,coord.st).rgb;    
}


void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	vec2 uv = fragCoord.xy / iResolution.xy;
    
    vec2 texC = uv;
    
    vec2 lpos = iMouse.xy / iResolution.x;
    
    vec2 texC2 = fragCoord.xy / iResolution.x;
    
    texC = mix(texC2, (texC * 2.0 - lpos * 2.0) * 0.0 * 0.5 + lpos, (WARPFACTOR*2. / (distance((texC2 * 2.0 - lpos * 2.0) * WARP*100. * 0.5 + lpos, lpos) - 1.0))); //Black hole shader
    
	vec3 getColor = getTexure(texC);
    
    getColor *= clamp(pow(distance(texC2, vec2(lpos)),HOLE*40.) * 300000000.0,0.0,1.0);
    
	fragColor = vec4(getColor,1.0);
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}