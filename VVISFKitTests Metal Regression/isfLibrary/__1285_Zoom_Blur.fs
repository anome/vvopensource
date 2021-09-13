/*{
	"CREDIT": "by Anomes",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"GENERATOR"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "strength",
			"TYPE": "float",
			"MIN": -10.0,
			"MAX": 10.0,
			"DEFAULT": 0.25
		},
		{
			"NAME": "center",
			"TYPE": "point2D",
			"DEFAULT": [0.5,0.5]
		}
	]
}*/




float random(vec3 scale,float seed)
{
	return fract(  sin(dot(gl_FragCoord.xyz+seed,scale))*43758.5453  +  seed  );
}


void main()
{
	const float iterations = 25.;//float(quality+1)*25.;
	vec2 pos = gl_FragCoord.xy / RENDERSIZE;
	vec4 color = vec4(0.);
	float total = 0.;
	vec2 toCenter = center - pos;
	float offset = random(  vec3(12.9898,78.233,151.7182), 0.  );
	for(float t=0.; t<=iterations; t++)
	{
		float percent = (t+offset)/iterations;
		float weight = 4.0*(percent-percent*percent);
		vec4 sample = IMG_NORM_PIXEL(inputImage, pos+toCenter*percent*strength);
		sample.rgb *= sample.a;
		color += sample*weight;
		total += weight;
	}
	gl_FragColor = color/total;
	gl_FragColor.rgb /= max(gl_FragColor.a,0.00001);
}

