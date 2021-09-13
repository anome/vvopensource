/*{
	"CREDIT": "by VJ RYO",
	"ISFVSN": "2",
	"CATEGORIES": [
		"Distortion Effect", "Geometry Adjustment"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "inputAudio",
			"TYPE": "audio"
		},
		{
			"NAME": "level",
			"TYPE": "float",
			"MIN": 0.01,
			"MAX": 10.0,
			"DEFAULT": 1.0
		},
		{
			"NAME": "cutoff",
			"TYPE": "float",
			"MIN": 0.01,
			"MAX": 10.0,
			"DEFAULT": 1.0
		},
		{
			"NAME": "amp",
			"TYPE": "float",
			"MIN": 0.01,
			"MAX": 10.0,
			"DEFAULT": 1.0
		},
		{
			"NAME": "center",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				0.5
			]
		}

	]

}*/
float levelres;
float t = level;
float p = 10.0 / 3.14;
float d = inputAudio;
float smoother = 5.0 - 5.0 *cos(t/p);



void main() {
	vec2		loc;
	vec2		modifiedCenter;
	
	if(level*amp < cutoff){
	levelres = 0.001 ;	
	}
	else
	{
	levelres = smoother*amp*d;
	}
	
	loc = isf_FragNormCoord;
	modifiedCenter = center / RENDERSIZE;
	loc.x = (loc.x - modifiedCenter.x)*(1.0/levelres) + modifiedCenter.x;
	loc.y = (loc.y - modifiedCenter.y)*(1.0/levelres) + modifiedCenter.y;
	if ((loc.x < 0.0)||(loc.y < 0.0)||(loc.x > 1.0)||(loc.y > 1.0))	{
		gl_FragColor = vec4(0.0);
	}
	else	{
		gl_FragColor = IMG_NORM_PIXEL(inputImage,loc);
	}
}
