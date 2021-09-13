
/*{
	"DESCRIPTION": "Blur",
	"CREDIT": "SoleneMV",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "applyEffect",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "softness",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
	
}*/

void main()	{
    vec4 color = IMG_THIS_NORM_PIXEL(inputImage);
    vec4 outColor = color;
    
    vec2 texc = vec2(isf_FragNormCoord[0],isf_FragNormCoord[1]);
    vec2 d = 1.0/RENDERSIZE;

	if (applyEffect) {
	    vec2 left_coord = clamp(vec2(texc.xy + vec2(-d.x , 0)),0.0,1.0);
	    vec2 right_coord = clamp(vec2(texc.xy + vec2(d.x , 0)),0.0,1.0);
	    vec2 above_coord = clamp(vec2(texc.xy + vec2(0,d.y)),0.0,1.0);
	    vec2 below_coord = clamp(vec2(texc.xy + vec2(0,-d.y)),0.0,1.0);

	    vec2 lefta_coord = clamp(vec2(texc.xy + vec2(-d.x , d.x)),0.0,1.0);
	    vec2 righta_coord = clamp(vec2(texc.xy + vec2(d.x , d.x)),0.0,1.0);
	    vec2 leftb_coord = clamp(vec2(texc.xy + vec2(-d.x , -d.x)),0.0,1.0);
	    vec2 rightb_coord = clamp(vec2(texc.xy + vec2(d.x , -d.x)),0.0,1.0);
	    
    	vec4 colorL = IMG_NORM_PIXEL(inputImage, left_coord);
	    vec4 colorR = IMG_NORM_PIXEL(inputImage, right_coord);
	    vec4 colorA = IMG_NORM_PIXEL(inputImage, above_coord);
	    vec4 colorB = IMG_NORM_PIXEL(inputImage, below_coord);

	    vec4 colorLA = IMG_NORM_PIXEL(inputImage, lefta_coord);
	    vec4 colorRA = IMG_NORM_PIXEL(inputImage, righta_coord);
	    vec4 colorLB = IMG_NORM_PIXEL(inputImage, leftb_coord);
	    vec4 colorRB = IMG_NORM_PIXEL(inputImage, rightb_coord);
	    
	    vec4 avg = (color + colorL + colorR + colorA + colorB + colorLA + colorRA + colorLB + colorRB) / 9.0;
	    
	    avg = mix(color, avg, softness);
	    
	    outColor = avg;
	}
	
	gl_FragColor = outColor;
}
