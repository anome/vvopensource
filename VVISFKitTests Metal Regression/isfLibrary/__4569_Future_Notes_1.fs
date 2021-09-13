/*
	{
	"DESCRIPTION": "Play with Coordinate System",
	"CATEGORIES": 
		[
		"generator"
		],
	"ISFVSN": "2",
	"CREDIT": "Old Salt",
	"VSN": "1.0",
  "INPUTS":
		[
			{
			"LABEL": "Coordinate Play? ",
			"NAME": "uActive",
			"TYPE": "bool",
			"DEFAULT": 0
			},
			{
			"LABEL": "Origin Location: ",
			"LABELS":
				[
				"Center of Viewport ",
				"Bottom Left Corner "
				],
			"NAME": "uCenter",
			"TYPE": "long",
			"VALUES": [0,1],
			"DEFAULT": 0
			},
			{
			"LABEL": "Order of Transformation: ",
			"LABELS":
				[
				"Zoom then Offset ",
				"Offset then Zoom "
				],
			"NAME": "uOrder",
			"TYPE": "long",
			"VALUES": [0,1],
			"DEFAULT": 0
			},
			{
			"LABEL": "Offset: ",
			"NAME": "uOffset",
			"TYPE": "point2D",
			"MAX": [1.0,1.0],
			"MIN": [-1.0,-1.0],
			"DEFAULT": [-0.25,0.25]
			},
 			{
			"LABEL": "Zoom: ",
			"NAME": "uZoom",
			"TYPE": "float",
			"MAX": 10.0,
			"MIN": 0.0,
			"DEFAULT": 1.0
			}
		]
	}
*/


float Band(float t, float start, float end, float blur){
	float step1 = smoothstep(start-blur, start+blur, t);
	float step2 = smoothstep(end+blur, end-blur, t);
	return step1*step2;
}

float Rect(vec2 uv, float left, float right, float bottom, float top, float blur){
	float band1 = Band(uv.x, left, right, blur);
	float band2 = Band(uv.y, bottom, top, blur);
	return band1 * band2;
}
void main()
	{

/*****      manipulate the coordinate system      *****/
	vec2 p;
	if (uCenter==0)
		{
		p =(gl_FragCoord.xy-RENDERSIZE*0.5)/RENDERSIZE; // normalize coordinates (origin at center)
		}
	else
		{
		p = gl_FragCoord.xy / RENDERSIZE;               // normalize coordinates (origin at bottom left)
		}
	if (uOrder==0)
		{
		p /= uZoom;
		p -= uOffset;
		}
	else
		{
		p -= uOffset;
		p /= uZoom;
		}
	
/*****   generate a simple rectangle for testing  *****/
/***** Borrowed from Shader Toy Instruction Video *****/
	vec3 col = vec3(0.0);
	float mask = 0.0;
	mask = Rect(p, -0.1, 0.1, -0.1, 0.1, 0.0001);
	col = vec3(1.0 * mask);
	
	gl_FragColor = uActive ? vec4(col, 1.0) : vec4(0.6, 0.0, 1.0, 1.0);
	}
