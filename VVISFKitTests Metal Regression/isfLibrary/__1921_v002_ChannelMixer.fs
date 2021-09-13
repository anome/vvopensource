/*{
	"DESCRIPTION": "",
	"CREDIT": "shader by v002, ported by zoidberg",
	"CATEGORIES": [
		"Halftone Effect"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "amount",
			"TYPE": "float",
			"DEFAULT": 0.0
		},
		{
			"NAME": "redR",
			"TYPE": "float"
		},
		{
			"NAME": "redG",
			"TYPE": "float"
		},
		{
			"NAME": "redB",
			"TYPE": "float"
		},
		{
			"NAME": "greenR",
			"TYPE": "float"
		},
		{
			"NAME": "greenG",
			"TYPE": "float"
		},
		{
			"NAME": "greenB",
			"TYPE": "float"
		},
		{
			"NAME": "blueR",
			"TYPE": "float"
		},
		{
			"NAME": "blueG",
			"TYPE": "float"
		},
		{
			"NAME": "blueB",
			"TYPE": "float"
		}
	]
}*/

void main (void) 
{
	vec3	red = vec3(redR,redG,redB);
	vec3	green = vec3(greenR,greenG,greenB);
	vec3	blue = vec3(blueR,blueG,blueB);
	
	vec4	input0 = IMG_PIXEL(inputImage,gl_FragCoord.xy);
	
	vec3 redchannel = vec3(input0.r) * red;
	vec3 greenchannel = vec3(input0.g) * green;
	vec3 bluechannel = vec3(input0.b) * blue;
	
	vec3 result = redchannel + greenchannel + bluechannel;
	
	gl_FragColor = mix(input0, vec4(result, input0.a), amount);

} 
