/*{
	"CREDIT": "INKA",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"INKA"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "vignette",
			"TYPE": "float",
			"DEFAULT": 0.5
		},
		{
			"NAME": "stronger",
			"TYPE": "bool"
		},
		{
			"NAME": "colorDark",
			"TYPE": "color",
			"DEFAULT": [
				0.5,
				0.07,
				0.1,
				0.8
			]
		},
		{
			"NAME": "colorLight",
			"TYPE": "color",
			"DEFAULT": [
				1.0,
				1.0,
				0.77,
				0.08
			]
		}
	]
}*/

vec4 hardMix(vec4 top, vec4 bottom) {
	float topAlpha = 1.0;
	float bottomAlpha = 1.0;
	vec4 darkenedBottom = vec4(bottomAlpha) * bottom;
	vec4 returnMe;
	
	float mixx = 0.5;
	
	returnMe.r = (top.r > mixx)
		? (1.0 - (1.0 - darkenedBottom.r) * (1.0 - 2.0 * (top.r - mixx)))
		: (darkenedBottom.r * (2.0 * top.r));
	returnMe.g = (top.g > mixx)
		? (1.0 - (1.0 - darkenedBottom.g) * (1.0 - 2.0 * (top.g - mixx)))
		: (darkenedBottom.g * (2.0 * top.g));
	returnMe.b = (top.b > mixx)
		? (1.0 - (1.0 - darkenedBottom.b) * (1.0 - 2.0 * (top.b - mixx)))
		: (darkenedBottom.b * (2.0 * top.b));
	
	returnMe.a = top.a;
	returnMe = mix(darkenedBottom, returnMe, topAlpha*top.a);
	return returnMe;
}

void main() {
	vec4 color = IMG_NORM_PIXEL(inputImage, vv_FragNormCoord.xy);
	if(vignette > 0.0) {
	    vec2 coord = (vv_FragNormCoord.xy - 0.5) * (RENDERSIZE.x / RENDERSIZE.y) * 1.;
	    float rf = sqrt(dot(coord, coord)) * vignette;
	    float rf2_1 = rf * rf + 1.0;
	    float e = 1.0 / (rf2_1 * rf2_1);
	    
    	vec4 vignettecol = mix(colorDark, colorLight, e);
	    vignettecol.a *= vignette;
	    
	    color = hardMix(vignettecol, color);
	    
	    if(stronger) {
		    vec4 _colorLight = colorLight;
		    _colorLight.a = 0.;
		    
	    	vignettecol = mix(colorDark, _colorLight, e);
		    vignettecol.a *= vignette;
		    
		    color = hardMix(vignettecol, color);
		    color = hardMix(vignettecol, color);
	    }
	    
	}
	gl_FragColor = color;
}