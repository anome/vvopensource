/*
	{
	"DESCRIPTION": "Invisible Elliptical Mask",
	"CATEGORIES": 
		[
		"filter"
		],
	"ISFVSN": "2",
	"CREDIT": "Old Salt",
	"VSN": "1.0.3",
	"INPUTS": 
		[
			{
			"LABEL": "Center: ",
			"NAME": "uOffset",
			"TYPE": "point2D",
			"MAX": [1.0,1.0],
			"MIN": [-1.0,-1.0],
			"DEFAULT": [0.0,0.0]
			},
			{
			"LABEL": "Size ",
			"NAME": "uSize",
			"TYPE": "point2D",
			"MIN": [0.0,0.0],
			"MAX": [1.0,1.0],
			"DEFAULT": [0.5,0.5]
			},
			{
			"LABEL": "Feather: ",
			"NAME": "uFeather",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.5
			},
			{
			"LABEL": "Continuous Rotation? ",
			"NAME": "uContRot",
			"TYPE": "bool",
			"DEFAULT": 1
			},
			{
			"LABEL": "Mask Behaviour: ",
			"NAME": "masktype",
			"TYPE": "long",
			"LABELS":
				[
				"Static     (stay at set size) ",
				"Collapsing (set size -> point)",
				"Expanding  (point -> set size)"
				],
			"VALUES":[0,1,2],
			"DEFAULT": 0
			},
			{
			"LABEL": "Invert Mask:  ",
			"NAME": "invert",
			"TYPE": "bool",
			"DEFAULT": 0
			},
			{
			"LABEL": "Show Mask:    ",
			"NAME": "show",
			"TYPE": "bool",
			"DEFAULT": 1
			}
		]
	}
*/

#define PI 3.1415926535897932384626433832795

void main()
	{
	float a = RENDERSIZE.y * uSize.x;                // horizontal size of ellipse
	float b = RENDERSIZE.y * uSize.y;                // vertical size of ellipse
	float fsize = (RENDERSIZE.y * (uFeather/2.0));   // size of feathered area
	float tm = fract(TIME);                          // 5 second expand or collapse
	if (masktype == 1)                               // use xLights 'Time Speed: ' to adjust
		{															                 // collapsing ellipse
		a = a * (1.0 - tm);
		b = b * (1.0 - tm);
		}
	else if (masktype == 2)                          // expanding ellipse
		{
		a = a * tm;
		b = b * tm;
		}                                              // nothing needed for static mask
	if (a < fsize) fsize = a;
	if (b < fsize) fsize = b;
	a = a + fsize;
	b = b + fsize;

	float tmin, tmax, tsize;                         // variables for iterative feathering
	float c;                                         // color and opacity

	vec2 uv = (gl_FragCoord.xy/RENDERSIZE.xy - (uOffset+1.0)*0.5 + 0.5);
	uv *= RENDERSIZE.xy;
	float x = uv.x - RENDERSIZE.x/2.0;      // re-center
	float y = uv.y - RENDERSIZE.y/2.0;

	float ellipse = (pow(x,2.0)/pow(a-fsize,2.0) + pow(y,2.0)/pow(b-fsize,2.0));

	if(ellipse < 1.0)                                // set mask color & opacity
		{
		c = 1.0;
		}
	else                                             //transition zone
		{
		tmin = 0.0;
		tmax = fsize;
		for (int i=0;i<8;i++)                          // work around for missing do ~ while statement				
			{                                            // allow a maximum of 8 iterations 
			tsize = tmin + (tmax-tmin)/2.0;
			if ((pow(x,2.0)/pow(a-tsize,2.0) + pow(y,2.0)/pow(b-tsize,2.0)) > 1.0)
				{
				tmax = tsize;
				}
			else
				{
				tmin = tsize;
				}
			if ((tmax - tmin) < (0.01 * tsize))          // transition zone accuracy limited to 1%
				{
				c = sin(fsize/tsize * PI/2.0);
				break;
				}
			};
		}
	if (invert) c = 1.0 - c;
	if (show) gl_FragColor = vec4(c, c, c, c);
	  else gl_FragColor = vec4(0.0, 0.0, 0.0, c);
	}