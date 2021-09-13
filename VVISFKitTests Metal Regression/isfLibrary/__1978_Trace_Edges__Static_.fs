/*{
	"CREDIT": "by VIDVOX",
	"CATEGORIES": [
		"Stylize"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "intensity",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 50.0,
			"DEFAULT": 4.2
		},
		{
			"NAME": "threshold",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.0
		},
		{
			"NAME": "sobel",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "opaque",
			"TYPE": "bool",
			"DEFAULT": true
		},
		{
			"NAME": "fade",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 0.05,
			"DEFAULT": 0.01
		}
	]
	,
	"PERSISTENT_BUFFERS": [
		"bufferVariableNameA"
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA"
		},
		{
		
		}
	]
}*/


varying vec2 left_coord;
varying vec2 right_coord;
varying vec2 above_coord;
varying vec2 below_coord;

varying vec2 lefta_coord;
varying vec2 righta_coord;
varying vec2 leftb_coord;
varying vec2 rightb_coord;

float gray(vec4 n)
{
	return (n.r + n.g + n.b)/3.0;
}

void main()
{

	//	do this junk so that the ripple starts from nothing
	vec2 uv = vv_FragNormCoord.xy;
	vec2 texCoord = uv;
	vec4 color = IMG_PIXEL(inputImage, gl_FragCoord.xy);
	vec4 stalePixel = IMG_PIXEL(bufferVariableNameA, gl_FragCoord.xy);
	
	float dist = distance(uv, RENDERSIZE/2.);
	float pos = 0.3;
	float adjustedTime = (pos * RENDERSIZE.x/RENDERSIZE.y - 1.0)/(1.0 - 1.0);

	//if ( (dist <= (adjustedTime + magnitude)) && (dist >= (adjustedTime - magnitude)) ) 	{
	if(true) {
		float diff = (3.);

		vec4 colorL = IMG_NORM_PIXEL(inputImage, left_coord);
		vec4 colorR = IMG_NORM_PIXEL(inputImage, right_coord);
		vec4 colorA = IMG_NORM_PIXEL(inputImage, above_coord);
		vec4 colorB = IMG_NORM_PIXEL(inputImage, below_coord);
	
		vec4 colorLA = IMG_NORM_PIXEL(inputImage, lefta_coord);
		vec4 colorRA = IMG_NORM_PIXEL(inputImage, righta_coord);
		vec4 colorLB = IMG_NORM_PIXEL(inputImage, leftb_coord);
		vec4 colorRB = IMG_NORM_PIXEL(inputImage, rightb_coord);
	
		float gx = (0.0);
		float gy = (0.0);
		if (sobel)	{
			gx = (-1.0 * gray(colorLA)) + (-2.0 * gray(colorL)) + (-1.0 * gray(colorLB)) + (1.0 * gray(colorRA)) + (2.0 * gray(colorR)) + (1.0 * gray(colorRB));
			gy = (1.0 * gray(colorLA)) + (2.0 * gray(colorA)) + (1.0 * gray(colorRA)) + (-1.0 * gray(colorRB)) + (-2.0 * gray(colorB)) + (-1.0 * gray(colorLB));
		}
		else	{
			gx = (-1.0 * gray(colorLA)) + (-1.0 * gray(colorL)) + (-1.0 * gray(colorLB)) + (1.0 * gray(colorRA)) + (1.0 * gray(colorR)) + (1.0 * gray(colorRB));
			gy = (1.0 * gray(colorLA)) + (1.0 * gray(colorA)) + (1.0 * gray(colorRA)) + (-1.0 * gray(colorRB)) + (-1.0 * gray(colorB)) + (-1.0 * gray(colorLB));
		}
	
		float bright = pow(gx*gx + gy*gy,0.5);
		vec4 final = color * bright;
		
		//	if the brightness is below the threshold draw black
		if (bright < threshold)	{
			if (opaque)
				final = vec4(0.0, 0.0, 0.0, 1.0);
			else
				final = vec4(0.0, 0.0, 0.0, 0.0);
		}
		else	{
			final = (final * (intensity)) * diff;
			if (opaque)
				final.a = 1.0;
		}
		color = final;
		//color = vec4(diff);
	} else {
		if (opaque)
				color = vec4(0.0, 0.0, 0.0, 1.0);
			else
				color = vec4(0.0, 0.0, 0.0, 0.0);
	}
	
	float brightLevel = (color.r + color.b + color.g) / 3.0;
	if (brightLevel < 0.5)
		brightLevel = 1.0;
	else
		brightLevel = 0.0;
		

	color = mix(color, stalePixel*(1.-fade), brightLevel);
	
	
	gl_FragColor = color;
}