/*
	{
	"DESCRIPTION": "Radial Replicate",
	"CATEGORIES": 
		[
		"filter"
		],
	"ISFVSN": "2",
	"CREDIT": "by: Old Salt",
	"VSN": "1.0",
	"INPUTS" : 
		[
			{
			"NAME": "inputImage",
			"TYPE": "image"
			},
			{
			"LABEL": "Number Of Divisions: ",
			"NAME": "uDivs",
			"TYPE": "float",
			"MAX": 360,
			"MIN": 1,
			"DEFAULT": 12
			},
			{
			"LABEL": "Start Position: ",
			"NAME": "uPos",
			"TYPE": "float",
			"MAX": 180,
			"MIN": -180,
			"DEFAULT": -9.4
			},
			{
			"LABEL": "Offset Output: ",
			"NAME": "uOffset",
			"TYPE": "point2D",
			"MAX": [1.0,1.0],
			"MIN": [-1.0,-1.0],
			"DEFAULT": [0.0,0.0]
			},
			{
			"LABEL": "Rotation: ",
			"NAME": "uRotate",
			"TYPE": "float",
			"MAX": 180,
			"MIN": -180,
			"DEFAULT": 0
			},
			{
			"LABEL": "Zoom: ",
			"NAME": "uZoom",
			"TYPE" : "float",
			"MAX" : 2,
			"MIN": 0,
			"DEFAULT" : 0.6
			},
			{
			"LABEL": "Intensity: ",
			"NAME": "uIntensity",
			"TYPE": "float",
			"MAX": 2,
			"MIN": 0,
			"DEFAULT": 1.0
			}
		]
	}
*/
// Based on "Radial Replicate" by: VIDVOX
// Original: https://editor.isf.video/shaders/5e7a7feb7c113618206de6f6

#define pi 3.1415926535897932384626433832795


void main()
	{
	vec2 iSize = IMG_SIZE(inputImage);          // xLights 2021.12 and later
	vec2 loc = iSize * (isf_FragNormCoord.xy - uOffset);

	//	'r' is the radius - distance from 'loc' to the center of the rendering space
	float r = distance(iSize/2.0, loc);

	//	'a' is the angle of the line segment from the center to loc is rotated
	float a = atan ((loc.y-iSize.y/2.0),(loc.x-iSize.x/2.0));
	float modAngle = 2.0 * pi / floor(uDivs);

	if(uZoom==0.0) r = r / 0.0001;             // prevent divide by 0 error
		else r =  r / uZoom;
	a = mod(a + pi * uRotate/360.0,modAngle);

	//	now modify 'a', convert the modified polar coords to cartesian coords
	loc.x = r * cos(a + 2.0 * pi * uPos / 360.0);
	loc.y = r * sin(a + 2.0 * pi * uPos / 360.0);
	loc = loc / iSize + vec2(0.5);

	vec4 col = vec4(0.0);
	if ((loc.x < 0.0)||(loc.y < 0.0)||(loc.x > 1.0)||(loc.y > 1.0))
		{
		col = vec4(0.0);
		}
	else
		{
		col = IMG_NORM_PIXEL(inputImage,loc);
		}
	gl_FragColor = vec4((col.rgb * uIntensity), 1.0);
	}